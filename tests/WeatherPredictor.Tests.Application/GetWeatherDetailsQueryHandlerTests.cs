using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using WeatherPredictor.Application.Exceptions;
using WeatherPredictor.Application.Queries;
using WeatherPredictor.Application.Services.Abstractions;
using WeatherPredictor.Domain.DTO;
using WeatherPredictor.Domain.Entities;
using WeatherPredictor.Domain.Repositories;
using Xunit;

namespace WeatherPredictor.Tests.Application;

public class GetWeatherDetailsQueryHandlerTests
{
    private readonly Mock<IWeatherRepository> _weatherRepositoryMock;
    private readonly Mock<IOpenMeteoWeatherApiService> _weatherApiServiceMock;
    private readonly Mock<IMemoryCache> _memoryCacheMock;
    private readonly GetWeatherDetailsQueryHandler _handler;

    public GetWeatherDetailsQueryHandlerTests()
    {
        // Arrange
        _weatherRepositoryMock = new Mock<IWeatherRepository>();
        _weatherApiServiceMock = new Mock<IOpenMeteoWeatherApiService>();
        _memoryCacheMock = new Mock<IMemoryCache>();

        _handler = new GetWeatherDetailsQueryHandler(_weatherRepositoryMock.Object, _weatherApiServiceMock.Object,
            _memoryCacheMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnCachedData_WhenDataExistsInCache()
    {
        // Arrange
        var query = new GetWeatherDetailsQuery(52.52, 13.41);
        var cachedWeatherDetails = new WeatherDetailsDto
            {Latitude = 52.52, Longitude = 13.41, CurrentWeatherDto = new CurrentWeatherDto {Temperature = 25}};

        _memoryCacheMock.Setup(cache => cache.TryGetValue(It.IsAny<object>(), out cachedWeatherDetails)).Returns(true);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(25, result.CurrentWeatherDto.Temperature);
    }

    [Fact]
    public async Task Handle_ShouldReturnDataFromRepository_WhenDataExistsInRepository()
    {
        // Arrange
        var query = new GetWeatherDetailsQuery(52.52, 13.41);
        var weatherFromDb = new Weather
        {
            Latitude = 52.52, Longitude = 13.41,
            Forecast = JsonSerializer.Serialize(new WeatherDetailsDto
                {CurrentWeatherDto = new CurrentWeatherDto {Temperature = 20}})
        };

        _weatherRepositoryMock.Setup(x => x.GetByCoordinatesAsync(query.Latitude, query.Longitude))
            .ReturnsAsync(weatherFromDb);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(20, result.CurrentWeatherDto.Temperature);
    }

    [Fact]
    public async Task Handle_ShouldFetchDataFromApiAndAddToCache_WhenDataNotInCacheOrRepository()
    {
        // Arrange
        var query = new GetWeatherDetailsQuery(52.52, 13.41);
        var apiWeatherDetails = new WeatherDetailsExternalApiDto
            {Latitude = 52.52, Longitude = 13.41, CurrentWeather = new CurrentWeather {Temperature = 18}};

        _weatherRepositoryMock.Setup(x => x.GetByCoordinatesAsync(query.Latitude, query.Longitude))
            .ReturnsAsync((Weather) null);
        
        _weatherApiServiceMock
            .Setup(api => api.GetDetailsAsync<WeatherDetailsExternalApiDto>(query.Latitude, query.Longitude))
            .ReturnsAsync(apiWeatherDetails);

        _memoryCacheMock.Setup(cache => cache.CreateEntry(It.IsAny<object>())).Returns(Mock.Of<ICacheEntry>);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(18, result.CurrentWeatherDto.Temperature);
        
        _weatherRepositoryMock.Verify(
            x => x.AddAsync(It.Is<Weather>(w => w.Latitude == query.Latitude && w.Longitude == query.Longitude)),
            Times.Once);

        _memoryCacheMock.Verify(cache => cache.CreateEntry(It.IsAny<object>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenApiReturnsNull()
    {
        // Arrange
        var query = new GetWeatherDetailsQuery(52.52, 13.41);

        _weatherRepositoryMock.Setup(x => x.GetByCoordinatesAsync(query.Latitude, query.Longitude))
            .ReturnsAsync((Weather) null);
        
        _weatherApiServiceMock
            .Setup(api => api.GetDetailsAsync<WeatherDetailsExternalApiDto>(query.Latitude, query.Longitude))
            .ReturnsAsync((WeatherDetailsExternalApiDto) null);

        // Act & Assert
        await Assert.ThrowsAsync<ErrorWhileGettingDataFormExternalApiException>(() =>
            _handler.Handle(query, CancellationToken.None));
    }
}