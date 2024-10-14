using Microsoft.EntityFrameworkCore;
using WeatherPredictor.Domain.DTO;
using WeatherPredictor.Domain.Entities;
using WeatherPredictor.Domain.Repositories;
using WeatherPredictor.Infrastructure.Exceptions;

namespace WeatherPredictor.Infrastructure.DAL.Repositories;

public class WeatherRepository : IRepository, IWeatherRepository
{
    private readonly WeatherPredictorDbContext _context;

    public WeatherRepository(WeatherPredictorDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Weather weather)
    {
        await _context.Weathers.AddAsync(weather);
        await _context.SaveChangesAsync();
    }

    public async Task<Weather> GetByCoordinatesAsync(double latitude, double longitude)
    {
        return await _context.Weathers.FirstOrDefaultAsync(x => x.Latitude == latitude && x.Longitude == longitude);
    }

    public async Task DeleteAsync(double latitude, double longitude)
    {
        var weatherToDelete = await GetByCoordinatesAsync(latitude, longitude);

        if (weatherToDelete is null)
        {
            throw new CouldNotGetWeatherDetailsException();
        }

        _context.Weathers.Remove(weatherToDelete);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<CoordinatesDto>> GetUsedCoordinatesAsync(int skip, int take)
    {
        return await _context.Weathers
            .Select(x => new CoordinatesDto() {Longitude = x.Longitude, Latitude = x.Latitude})
            .Skip(skip)
            .Take(take)
            .Distinct()
            .ToListAsync();
    }

    public async Task<bool> IsExistAsync(double latitude, double longitude)
    {
        return await _context.Weathers.AnyAsync(x => x.Latitude == latitude && x.Longitude == longitude);
    }

    public async Task<int> GetTotalCoordinatesCountAsync()
    {
        return await _context.Weathers.CountAsync();
    }
}