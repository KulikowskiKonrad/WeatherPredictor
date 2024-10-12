using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using WeatherPredictor.Application.Abstractions.Queries;
using WeatherPredictor.Application.Services.Abstractions;
using WeatherPredictor.Domain.DTO;
using WeatherPredictor.Domain.Entities;
using WeatherPredictor.Domain.Repositories;
using WeatherPredictor.Infrastructure.Utils;

namespace WeatherPredictor.Application.Queries;

public sealed class GetWeatherDetailsQueryHandler : IQueryHandler<GetWeatherDetailsQuery, WeatherDetailsDto>
{
    private readonly IWeatherRepository _weatherRepository;
    private readonly IWeatherApiService _weatherApiService;
    private readonly IMemoryCache _cache;

    public GetWeatherDetailsQueryHandler(IWeatherRepository weatherRepository, IWeatherApiService weatherApiService,
        IMemoryCache cache)
    {
        _weatherRepository = weatherRepository;
        _weatherApiService = weatherApiService;
        _cache = cache;
    }

    public async Task<WeatherDetailsDto> Handle(GetWeatherDetailsQuery request, CancellationToken cancellationToken)
    {
        var memoryCacheKey = $"{request.Latitude}_{request.Longitude}";

        if (_cache.TryGetValue(memoryCacheKey, out WeatherDetailsDto? weatherDetailsDto))
        {
            return weatherDetailsDto;
        }

        return await RetryPolicy.ExecuteWithRetryAsync(async () =>
        {
            var weatherDetailsListsFromDb =
                await _weatherRepository.GetByCoordinatesAsync(request.Latitude, request.Longitude);

            if (weatherDetailsListsFromDb is not null)
            {
                return JsonSerializer.Deserialize<WeatherDetailsDto>(weatherDetailsListsFromDb.Forecast);
            }
            else
            {
                var weatherDetailsListsFromApi =
                    await _weatherApiService.GetDetailsAsync<WeatherDetailsExternalApiDto>(request.Latitude,
                        request.Longitude);

                var mappedDetails = MapFormExternalApiDtoIntoLocalDto(weatherDetailsListsFromApi);

                var weather = new Weather()
                {
                    Longitude = Math.Round(weatherDetailsListsFromApi.Longitude, 2),
                    Latitude = Math.Round(weatherDetailsListsFromApi.Latitude, 2),
                    Forecast = JsonSerializer.Serialize(mappedDetails),
                    CreateDate = DateTime.UtcNow
                };

                _cache.Set(memoryCacheKey, mappedDetails, TimeSpan.FromSeconds(3));

                await _weatherRepository.AddAsync(weather);

                return mappedDetails;
            }
        });
    }

    private WeatherDetailsDto MapFormExternalApiDtoIntoLocalDto(WeatherDetailsExternalApiDto externalDto)
    {
        return new WeatherDetailsDto()
        {
            Latitude = externalDto.Latitude,
            Longitude = externalDto.Longitude,
            GenerationTimeMs = externalDto.GenerationTimeMs,
            UtcOffsetSeconds = externalDto.UtcOffsetSeconds,
            Timezone = externalDto.Timezone,
            TimezoneAbbreviation = externalDto.TimezoneAbbreviation,
            Elevation = externalDto.Elevation,
            LocationId = externalDto.LocationId,
            CurrentWeatherUnitsDto = externalDto.CurrentWeatherUnits != null
                ? new CurrentWeatherUnitsDto
                {
                    Time = externalDto.CurrentWeatherUnits.Time,
                    Interval = externalDto.CurrentWeatherUnits.Interval,
                    Temperature = externalDto.CurrentWeatherUnits.Temperature,
                    Windspeed = externalDto.CurrentWeatherUnits.Windspeed,
                    Winddirection = externalDto.CurrentWeatherUnits.Winddirection,
                    IsDay = externalDto.CurrentWeatherUnits.IsDay,
                    WeatherCode = externalDto.CurrentWeatherUnits.WeatherCode
                }
                : null,
            CurrentWeatherDto = externalDto.CurrentWeather != null
                ? new CurrentWeatherDto
                {
                    Time = externalDto.CurrentWeather.Time,
                    Interval = externalDto.CurrentWeather.Interval,
                    Temperature = externalDto.CurrentWeather.Temperature,
                    Windspeed = externalDto.CurrentWeather.Windspeed,
                    Winddirection = externalDto.CurrentWeather.Winddirection,
                    IsDay = externalDto.CurrentWeather.IsDay,
                    // WeatherCode = externalDto.CurrentWeather.WeatherCode
                }
                : null
        };
    }
}