using System.Text.Json.Serialization;

namespace WeatherPredictor.Domain.DTO;

public class WeatherDetailsExternalApiDto
{
    [JsonPropertyName("latitude")]
    public double Latitude { get; set; }

    [JsonPropertyName("longitude")]
    public double Longitude { get; set; }

    [JsonPropertyName("generationtime_ms")]
    public double? GenerationTimeMs { get; set; }

    [JsonPropertyName("utc_offset_seconds")]
    public int? UtcOffsetSeconds { get; set; }

    [JsonPropertyName("timezone")]
    public string Timezone { get; set; }

    [JsonPropertyName("timezone_abbreviation")]
    public string TimezoneAbbreviation { get; set; }

    [JsonPropertyName("elevation")]
    public double? Elevation { get; set; }

    [JsonPropertyName("location_id")]
    public int? LocationId { get; set; }

    [JsonPropertyName("current_weather_units")]
    public CurrentWeatherUnitsExternalApiDto CurrentWeatherUnitsExternalApiDto { get; set; }

    [JsonPropertyName("current_weather")]
    public CurrentWeatherExternalApiDto CurrentWeatherExternalApiDto { get; set; }
}

public class CurrentWeatherUnitsExternalApiDto
{
    [JsonPropertyName("time")]
    public string Time { get; set; }

    [JsonPropertyName("interval")]
    public string Interval { get; set; }

    [JsonPropertyName("temperature")]
    public string Temperature { get; set; }

    [JsonPropertyName("windspeed")]
    public string Windspeed { get; set; }

    [JsonPropertyName("winddirection")]
    public string Winddirection { get; set; }

    [JsonPropertyName("is_day")]
    public string IsDay { get; set; }

    [JsonPropertyName("weathercode")]
    public string WeatherCode { get; set; }
}

public class CurrentWeatherExternalApiDto
{
    [JsonPropertyName("time")]
    public string Time { get; set; }

    [JsonPropertyName("interval")]
    public int? Interval { get; set; }

    [JsonPropertyName("temperature")]
    public double? Temperature { get; set; }

    [JsonPropertyName("windspeed")]
    public double? Windspeed { get; set; }

    [JsonPropertyName("winddirection")]
    public int? Winddirection { get; set; }

    [JsonPropertyName("is_day")]
    public int? IsDay { get; set; }

    [JsonPropertyName("weathercode")]
    public int? WeatherCode { get; set; }
}
