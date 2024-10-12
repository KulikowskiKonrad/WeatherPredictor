namespace WeatherPredictor.API.Requests;

public class AddWeatherRequest
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double? GenerationTimeMs { get; set; }
    public int? UtcOffsetSeconds { get; set; }
    public string? Timezone { get; set; }
    public string? TimezoneAbbreviation { get; set; }
    public double? Elevation { get; set; }
    public CurrentWeatherUnits CurrentWeatherUnits { get; set; } = new();
    public CurrentWeather CurrentWeather { get; set; } = new();
}

public class CurrentWeatherUnits
{
    public string? Time { get; set; }
    public string? Interval { get; set; }
    public string? Temperature { get; set; }
    public string? Windspeed { get; set; }
    public string? Winddirection { get; set; }
    public string? IsDay { get; set; }
    public string? WeatherCode { get; set; }
}

public class CurrentWeather
{
    public string? Time { get; set; }
    public int? Interval { get; set; }
    public double? Temperature { get; set; }
    public double? Windspeed { get; set; }
    public int? Winddirection { get; set; }
    public int? IsDay { get; set; }
    public int? WeatherCode { get; set; }
}