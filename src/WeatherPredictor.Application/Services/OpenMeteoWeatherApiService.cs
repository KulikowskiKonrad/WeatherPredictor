using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WeatherPredictor.Application.Exceptions;
using WeatherPredictor.Application.OtionsSettings;
using WeatherPredictor.Application.Services.Abstractions;

namespace WeatherPredictor.Application.Services;

public class OpenMeteoWeatherApiService : IOpenMeteoWeatherApiService
{
    private readonly ILogger<OpenMeteoWeatherApiService> _logger;
    private readonly OpenMeteoOptions _openMeteoOptions;
    private readonly HttpClient _httpClient;

    public OpenMeteoWeatherApiService(ILogger<OpenMeteoWeatherApiService> logger, HttpClient httpClient,
        IOptions<OpenMeteoOptions> openMeteo)
    {
        _logger = logger;
        _httpClient = httpClient;
        _openMeteoOptions = openMeteo.Value;
    }

    public async Task<T> GetDetailsAsync<T>(double latitude, double longitude)
    {
        _logger.LogInformation("Get weather details.");

        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri =
                new Uri(
                    $"{_openMeteoOptions.ApiUrl}/v1/forecast?latitude={Math.Round(latitude, 2)}&longitude={Math.Round(longitude, 2)}&current_weather=true")
        };

        var response = await _httpClient.SendAsync(requestMessage);

        return await GetResultAsync<T>(response);
    }

    private async Task<T> GetResultAsync<T>(HttpResponseMessage? response)
    {
        T[]? result = default;

        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("Getting weather details finished successfully");

            await response.Content.ReadAsStringAsync().ContinueWith(x =>
            {
                if (!x.IsFaulted)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                    };

                    _logger.LogInformation("Response: {response}", x.Result);
                    result = JsonSerializer.Deserialize<T[]>(x.Result, options);
                }
            });

            return result.FirstOrDefault();
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

        _logger.LogError("Http Error: {statusCode}, {responseBody}", response.StatusCode, responseBody);
        throw new ErrorWhileGettingDataFormExternalApiException();
    }
}