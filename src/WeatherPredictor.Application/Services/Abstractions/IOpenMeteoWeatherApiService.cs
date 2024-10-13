namespace WeatherPredictor.Application.Services.Abstractions;

public interface IOpenMeteoWeatherApiService
{
    Task<T> GetDetailsAsync<T>(double latitude, double longitude);
}