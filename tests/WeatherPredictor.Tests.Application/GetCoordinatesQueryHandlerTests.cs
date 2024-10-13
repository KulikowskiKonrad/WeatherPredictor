using Moq;
using WeatherPredictor.Application.Queries;
using WeatherPredictor.Domain.DTO;
using WeatherPredictor.Domain.Repositories;
using Xunit;

namespace WeatherPredictor.Tests.Application;

public class GetCoordinatesQueryHandlerTests
{
    private readonly Mock<IWeatherRepository> _weatherRepositoryMock;
    private readonly GetCoordinatesQueryHandler _handler;

    public GetCoordinatesQueryHandlerTests()
    {
        // Arrange
        _weatherRepositoryMock = new Mock<IWeatherRepository>();


        _handler = new GetCoordinatesQueryHandler(_weatherRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenSkipIsGreaterThanOrEqualToTotalItems()
    {
        // Arrange
        var pageNumber = 3;
        var pageSize = 10;

        var request = new GetCoordinatesQuery(pageSize, pageNumber);

        _weatherRepositoryMock.Setup(x => x.GetTotalCoordinatesCountAsync()).ReturnsAsync(15);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task Handle_ShouldReturnCoordinates_WhenSkipIsLessThanTotalItems()
    {
        // Arrange
        var pageNumber = 3;
        var pageSize = 10;

        var request = new GetCoordinatesQuery(pageSize, pageNumber);

        _weatherRepositoryMock.Setup(x => x.GetTotalCoordinatesCountAsync()).ReturnsAsync(20);
        _weatherRepositoryMock.Setup(x => x.GetUsedCoordinatesAsync(0, 10)).ReturnsAsync(new List<CoordinatesDto>
        {
            new CoordinatesDto {Latitude = 52.52, Longitude = 13.41},
            new CoordinatesDto {Latitude = 40.71, Longitude = -74.01}
        });

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotEmpty(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task Handle_ShouldCalculateSkipCorrectly()
    {
        // Arrange
        var pageNumber = 2;
        var pageSize = 10;

        var request = new GetCoordinatesQuery(pageSize, pageNumber);

        _weatherRepositoryMock.Setup(x => x.GetTotalCoordinatesCountAsync()).ReturnsAsync(30);
        _weatherRepositoryMock.Setup(x => x.GetUsedCoordinatesAsync(10, 10)).ReturnsAsync(new List<CoordinatesDto>
        {
            new CoordinatesDto {Latitude = 48.85, Longitude = 2.35}
        });

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Single(result);
        Assert.Equal(48.85, result.First().Latitude);
        Assert.Equal(2.35, result.First().Longitude);
    }
}