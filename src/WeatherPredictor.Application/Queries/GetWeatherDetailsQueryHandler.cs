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
            CurrentWeatherUnitsDto = externalDto.CurrentWeatherUnitsExternalApiDto != null
                ? new CurrentWeatherUnitsDto
                {
                    Time = externalDto.CurrentWeatherUnitsExternalApiDto.Time,
                    Interval = externalDto.CurrentWeatherUnitsExternalApiDto.Interval,
                    Temperature = externalDto.CurrentWeatherUnitsExternalApiDto.Temperature,
                    Windspeed = externalDto.CurrentWeatherUnitsExternalApiDto.Windspeed,
                    Winddirection = externalDto.CurrentWeatherUnitsExternalApiDto.Winddirection,
                    IsDay = externalDto.CurrentWeatherUnitsExternalApiDto.IsDay,
                    WeatherCode = externalDto.CurrentWeatherUnitsExternalApiDto.WeatherCode
                }
                : null,
            CurrentWeatherDto = externalDto.CurrentWeatherExternalApiDto != null
                ? new CurrentWeatherDto
                {
                    Time = externalDto.CurrentWeatherExternalApiDto.Time,
                    Interval = externalDto.CurrentWeatherExternalApiDto.Interval,
                    Temperature = externalDto.CurrentWeatherExternalApiDto.Temperature,
                    Windspeed = externalDto.CurrentWeatherExternalApiDto.Windspeed,
                    Winddirection = externalDto.CurrentWeatherExternalApiDto.Winddirection,
                    IsDay = externalDto.CurrentWeatherExternalApiDto.IsDay,
                    WeatherCode = externalDto.CurrentWeatherExternalApiDto.WeatherCode
                }
                : null
        };
    }
}