namespace WeatherPredictor.Application.Services.Abstractions;

public interface IWeatherApiService
{
    Task<T> GetDetailsAsync<T>(double latitude, double longitude);
}