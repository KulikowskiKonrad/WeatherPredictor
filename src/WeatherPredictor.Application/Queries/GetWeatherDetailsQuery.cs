using WeatherPredictor.Application.Abstractions.Queries;
using WeatherPredictor.Domain.DTO;

namespace WeatherPredictor.Application.Queries;

public class GetWeatherDetailsQuery : IQuery<WeatherDetailsDto>
{
    public double Latitude { get; }
    public double Longitude { get; }

    public GetWeatherDetailsQuery(double latitude, double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }
}