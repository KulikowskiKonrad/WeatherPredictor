using Moq;
using WeatherPredictor.Application.Commands;
using WeatherPredictor.Domain.Repositories;
using Xunit;

namespace WeatherPredictor.Tests.Application;

public class DeleteWeatherCommandHandlerTests
{
    private readonly Mock<IWeatherRepository> _weatherRepositoryMock;
    private readonly DeleteWeatherCommandHandler _handler;

    public DeleteWeatherCommandHandlerTests()
    {
        // Arrange 
        _weatherRepositoryMock = new Mock<IWeatherRepository>();

        _handler = new DeleteWeatherCommandHandler(_weatherRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCallDeleteAsync_WhenCommandIsValid()
    {
        // Arrange
        var latitude = 52.52;
        var longitude = 13.41;
        var command = new DeleteWeatherCommand(latitude, longitude);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert 
        _weatherRepositoryMock.Verify(
            x => x.DeleteAsync(command.Latitude, command.Longitude),
            Times.Once);
    }
}