using WeatherPredictor.Application.Abstractions.Commands;

namespace WeatherPredictor.Application.Commands;

public class DeleteWeatherCommand : ICommand
{
    public double Latitude { get; }
    public double Longitude { get; }

    public DeleteWeatherCommand(double latitude, double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }
}