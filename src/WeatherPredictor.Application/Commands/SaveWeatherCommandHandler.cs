using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using WeatherPredictor.Application.Abstractions.Commands;
using WeatherPredictor.Application.Utils;
using WeatherPredictor.Domain.Entities;
using WeatherPredictor.Domain.Repositories;

namespace WeatherPredictor.Application.Commands;

public sealed class SaveWeatherCommandHandler : ICommandHandler<SaveWeatherCommand>
{
    private readonly IWeatherRepository _weatherRepository;
    private readonly IMemoryCache _cache;

    public SaveWeatherCommandHandler(IWeatherRepository weatherRepository, IMemoryCache cache)
    {
        _weatherRepository = weatherRepository;
        _cache = cache;
    }

    public async Task Handle(SaveWeatherCommand request, CancellationToken cancellationToken)
    {
        var exist = await _weatherRepository.IsExistAsync(request.WeatherDetails.Latitude,
            request.WeatherDetails.Longitude);

        if (!exist)
        {
            var memoryCacheKey = $"{request.WeatherDetails.Latitude}_{request.WeatherDetails.Longitude}";

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            };

            var weather = new Weather()
            {
                Longitude = Math.Round(request.WeatherDetails.Longitude, 2),
                Latitude = Math.Round(request.WeatherDetails.Latitude, 2),
                CreateDate = DateTime.UtcNow,
                Forecast = JsonSerializer.Serialize(request.WeatherDetails, options)
            };

            _cache.Set(memoryCacheKey, request.WeatherDetails, TimeSpan.FromSeconds(3));

            try
            {
                await _weatherRepository.AddAsync(weather);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message, ex.InnerException);
            }
        }
    }
}