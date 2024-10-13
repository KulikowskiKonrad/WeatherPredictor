namespace WeatherPredictor.Application.Clients;

public class OpenMeteoApiClient
{
    private readonly HttpClient _httpClient;

    public OpenMeteoApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
}