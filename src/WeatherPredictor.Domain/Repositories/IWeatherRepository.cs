using WeatherPredictor.Domain.DTO;
using WeatherPredictor.Domain.Entities;

namespace WeatherPredictor.Domain.Repositories;

public interface IWeatherRepository
{
    Task AddAsync(Weather weather);
    Task<Weather> GetByCoordinatesAsync(double latitude, double longitude);
    Task DeleteAsync(double latitude, double longitude);
    Task<List<CoordinatesDto>> GetUsedCoordinatesAsync();
    Task<bool> IsExistAsync(double latitude, double longitude);
}