using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using WeatherPredictor.Application.Exceptions;
using WeatherPredictor.Application.OtionsSettings;
using WeatherPredictor.Application.Services.Abstractions;
using JsonSerializer = System.Text.Json.JsonSerializer;

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
        T? result = default;

        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("Getting weather details finished successfully");

            await response.Content.ReadAsStringAsync().ContinueWith(x =>
            {
                if (!x.IsFaulted)
                {
                    var options = new JsonSerializerSettings
                    {
                        MissingMemberHandling = MissingMemberHandling.Ignore,
                        NullValueHandling = NullValueHandling.Ignore,
                        DefaultValueHandling = DefaultValueHandling.Include
                    };

                    _logger.LogInformation("Response: {response}", x.Result);

                    try
                    {
                        // Check if x.Result is a JSON array
                        if (x.Result.TrimStart().StartsWith("["))
                        {
                           var results =  JsonConvert.DeserializeObject<T[]>(x.Result, options);
                           result = results.FirstOrDefault();
                        }
                        else
                        {
                            // If not an array, deserialize as a single object
                            result = JsonConvert.DeserializeObject<T>(x.Result, options);
                        }
                        
                    } catch (JsonSerializationException ex)
                    {
                        _logger.LogError(ex, "Error during JSON deserialization");
                        throw;
                    }
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

        _logger.LogError("Http Error: {statusCode}, {responseBody}", response.StatusCode, responseBody);
        throw new ErrorWhileGettingDataFormExternalApiException();
    }
}