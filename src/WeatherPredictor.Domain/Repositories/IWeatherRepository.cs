using WeatherPredictor.Domain.DTO;
using WeatherPredictor.Domain.Entities;

namespace WeatherPredictor.Domain.Repositories;

public interface IWeatherRepository
{
    Task AddAsync(Weather weather);
    Task<Weather> GetByCoordinatesAsync(double latitude, double longitude);
    Task DeleteAsync(double latitude, double longitude);
    Task<IEnumerable<CoordinatesDto>> GetUsedCoordinatesAsync(int skip, int take);
    Task<bool> IsExistAsync(double latitude, double longitude);
    Task<int> GetTotalCoordinatesCountAsync();
}