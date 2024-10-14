using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Polly;
using WeatherPredictor.Application.Abstractions.Queries;
using WeatherPredictor.Application.Exceptions;
using WeatherPredictor.Application.Services.Abstractions;
using WeatherPredictor.Application.Utils;
using WeatherPredictor.Domain.DTO;
using WeatherPredictor.Domain.Entities;
using WeatherPredictor.Domain.Repositories;

namespace WeatherPredictor.Application.Queries;

public sealed class GetWeatherDetailsQueryHandler : IQueryHandler<GetWeatherDetailsQuery, WeatherDetailsDto>
{
    private readonly IWeatherRepository _weatherRepository;
    private readonly IOpenMeteoWeatherApiService _openMeteoWeatherApiService;
    private readonly IMemoryCache _cache;
    private readonly IAsyncPolicy<HttpResponseMessage> _retryPolicy;
    private readonly ILogger<GetWeatherDetailsQueryHandler> _logger;

    public GetWeatherDetailsQueryHandler(IWeatherRepository weatherRepository,
        IOpenMeteoWeatherApiService openMeteoWeatherApiService,
        IMemoryCache cache, ILogger<GetWeatherDetailsQueryHandler> logger)
    {
        _weatherRepository = weatherRepository;
        _openMeteoWeatherApiService = openMeteoWeatherApiService;
        _cache = cache;
        _logger = logger;
        _retryPolicy = RetryPolicy.GetRetryPolicy();
    }

    public async Task<WeatherDetailsDto> Handle(GetWeatherDetailsQuery request, CancellationToken cancellationToken)
    {
        var memoryCacheKey = $"{request.Latitude}_{request.Longitude}";

        if (_cache.TryGetValue(memoryCacheKey, out WeatherDetailsDto? weatherDetailsDto))
        {
            return weatherDetailsDto;
        }

        var weatherDetailsListsFromDb =
            await _weatherRepository.GetByCoordinatesAsync(request.Latitude, request.Longitude);

        if (weatherDetailsListsFromDb is not null)
        {
            _logger.LogInformation("Checking is coordinates exist in database");

            return JsonSerializer.Deserialize<WeatherDetailsDto>(weatherDetailsListsFromDb.Forecast);
        }

        var weatherDetailsListsFromApi = await _retryPolicy.ExecuteAsync(async () =>
        {
            _logger.LogInformation("Getting data form external api");

            var response =
                await _openMeteoWeatherApiService.GetDetailsAsync<WeatherDetailsExternalApiDto>(request.Latitude,
                    request.Longitude);

            _logger.LogInformation("Response from external client {response}", response);

            if (response == null)
            {
                throw new ErrorWhileGettingDataFormExternalApiException();
            }

            return new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                {Content = new StringContent(JsonSerializer.Serialize(response))};
        });

        var responseContent = await weatherDetailsListsFromApi.Content.ReadAsStringAsync(cancellationToken);
        var weatherDetailsExternal = JsonSerializer.Deserialize<WeatherDetailsExternalApiDto>(responseContent);

        var mappedDetails = MapFormExternalApiDtoIntoLocalDto(weatherDetailsExternal);

        var weather = new Weather()
        {
            Longitude = weatherDetailsExternal.Longitude,
            Latitude = weatherDetailsExternal.Latitude,
            Forecast = JsonSerializer.Serialize(mappedDetails),
            CreateDate = DateTime.UtcNow
        };

        var exist = await _weatherRepository.IsExistAsync(weatherDetailsExternal.Latitude,
            weatherDetailsExternal.Longitude);

        _cache.Set(memoryCacheKey, mappedDetails, TimeSpan.FromSeconds(3));

        if (!exist)
        {
            try
            {
                await _weatherRepository.AddAsync(weather);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message, ex.InnerException);
            }
        }

        return mappedDetails;
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
                    WeatherCode = externalDto.CurrentWeather.WeatherCode
                }
                : null
        };
    }
}