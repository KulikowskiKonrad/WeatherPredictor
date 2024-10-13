using Microsoft.Extensions.Caching.Memory;
using Moq;
using WeatherPredictor.Application.Commands;
using WeatherPredictor.Domain.Entities;
using WeatherPredictor.Domain.Repositories;
using Xunit;

namespace WeatherPredictor.Tests.Application;

public class SaveWeatherCommandHandlerTests
{
    private readonly Mock<IWeatherRepository> _weatherRepositoryMock;
    private readonly Mock<IMemoryCache> _memoryCacheMock;
    private readonly SaveWeatherCommandHandler _handler;

    public SaveWeatherCommandHandlerTests()
    {
        // Arrange 
        _weatherRepositoryMock = new Mock<IWeatherRepository>();
        _memoryCacheMock = new Mock<IMemoryCache>();

        _handler = new SaveWeatherCommandHandler(_weatherRepositoryMock.Object, _memoryCacheMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldAddToRepositoryAndCache_WhenWeatherDoesNotExist()
    {
        // Arrange
        var weatherDetails = new SaveWeatherCommand.WeatherDetailsCommand
        {
            Latitude = 52.52,
            Longitude = 13.41
        };

        var command = new SaveWeatherCommand(weatherDetails);

        _weatherRepositoryMock
            .Setup(x => x.IsExistAsync(command.WeatherDetails.Latitude, command.WeatherDetails.Longitude))
            .ReturnsAsync(false);

        _memoryCacheMock
            .Setup(x => x.CreateEntry(It.IsAny<object>()))
            .Returns(Mock.Of<ICacheEntry>);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _weatherRepositoryMock.Verify(
            x => x.AddAsync(It.Is<Weather>(c =>
                c.Latitude == command.WeatherDetails.Latitude && c.Longitude == command.WeatherDetails.Longitude)),
            Times.Once);

        _memoryCacheMock.Verify(
            x => x.CreateEntry(It.IsAny<object>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldNotAddToRepository_WhenWeatherAlreadyExists()
    {
        // Arrange
        var weatherDetails = new SaveWeatherCommand.WeatherDetailsCommand
        {
            Latitude = 52.52,
            Longitude = 13.41
        };

        var command = new SaveWeatherCommand(weatherDetails);

        _weatherRepositoryMock
            .Setup(x => x.IsExistAsync(command.WeatherDetails.Latitude, command.WeatherDetails.Longitude))
            .ReturnsAsync(true);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _weatherRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Weather>()), Times.Never);
        _memoryCacheMock.Verify(x => x.CreateEntry(It.IsAny<object>()), Times.Never);
    }
}