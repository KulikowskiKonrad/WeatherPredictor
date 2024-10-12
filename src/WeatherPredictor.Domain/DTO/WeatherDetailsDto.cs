namespace WeatherPredictor.Domain.DTO;

public class WeatherDetailsDto
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double? GenerationTimeMs { get; set; }
    public int? UtcOffsetSeconds { get; set; }
    public string Timezone { get; set; }
    public string TimezoneAbbreviation { get; set; }
    public double? Elevation { get; set; }
    public int? LocationId { get; set; }
    public CurrentWeatherUnitsDto CurrentWeatherUnitsDto { get; set; }
    public CurrentWeatherDto CurrentWeatherDto { get; set; }
}

public class CurrentWeatherUnitsDto
{
    public string Time { get; set; }
    public string Interval { get; set; }
    public string Temperature { get; set; }
    public string Windspeed { get; set; }
    public string Winddirection { get; set; }
    public string IsDay { get; set; }
    public string WeatherCode { get; set; }
}

public class CurrentWeatherDto
{
    public string Time { get; set; }
    public int? Interval { get; set; }
    public double? Temperature { get; set; }
    public double? Windspeed { get; set; }
    public int? Winddirection { get; set; }
    public int? IsDay { get; set; }
    public int? WeatherCode { get; set; }
}