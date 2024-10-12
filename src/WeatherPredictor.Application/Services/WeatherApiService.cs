using System.Text.Json;
using Microsoft.Extensions.Logging;
using WeatherPredictor.Application.Exceptions;
using WeatherPredictor.Application.Services.Abstractions;

namespace WeatherPredictor.Application.Services;

public class WeatherApiService : IWeatherApiService
{
    private readonly ILogger<WeatherApiService> _logger;
    private readonly HttpClient _httpClient;

    public WeatherApiService(ILogger<WeatherApiService> logger, HttpClient httpClient)
    {
        _logger = logger;
        _httpClient = httpClient;
    }

    public async Task<T> GetDetailsAsync<T>(double latitude, double longitude)
    {
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri =
                new Uri(
                    $"https://api.open-meteo.com/v1/forecast?latitude={latitude}&longitude={longitude}&current_weather=true")
        };

        _logger.LogInformation("Get weather details.");

        var response = await _httpClient.SendAsync(requestMessage);

        return await GetResultAsync<T>(response);
    }

    private async Task<T> GetResultAsync<T>(HttpResponseMessage? response)
    {
        T? result = default;

        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("Getting weather details finished successfully");

            await response.Content.ReadAsStringAsync().ContinueWith(x =>
            {
                if (!x.IsFaulted)
                {
                    _logger.LogInformation($"Response: {x.Result}");
                    result = JsonSerializer.Deserialize<T>(x.Result);
                }
            });

            return result;
        }

        _logger.LogInformation(
            "Getting weather details with BODY: {body}. Failed with code: {code} and details: {details}",
            response.StatusCode, response.ReasonPhrase);

        var responseBody = string.Empty;
        await response.Content.ReadAsStringAsync().ContinueWith(x =>
        {
            if (!x.IsFaulted && !string.IsNullOrEmpty(x.Result))
            {
                _logger.LogInformation($"SetHttpRequestAsync response: {x.Result}");
                var data = JsonSerializer.Deserialize<string>(x.Result);
                responseBody = data;
            }
        });

        _logger.LogError($"Http Error: {response.StatusCode}, {responseBody}");
        throw new ErrorWhileGettingDataFormExternalApiException();
    }
}