using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace WeatherPredictor.Domain.DTO;

public class WeatherDetailsExternalApiDto
{
    [JsonProperty("latitude")]
    public double Latitude { get; set; }

    [JsonProperty("longitude")]
    public double Longitude { get; set; }

    [JsonProperty("generationtime_ms")]
    public double? GenerationTimeMs { get; set; }

    [JsonProperty("utc_offset_seconds")]
    public int? UtcOffsetSeconds { get; set; }

    [JsonProperty("timezone")]
    public string? Timezone { get; set; }

    [JsonProperty("timezone_abbreviation")]
    public string? TimezoneAbbreviation { get; set; }

    [JsonProperty("elevation")]
    public double? Elevation { get; set; }

    [JsonProperty("location_id")]
    public int? LocationId { get; set; }

    [JsonProperty("current_weather_units")]
    public CurrentWeatherUnits CurrentWeatherUnits { get; set; } = new();

    [JsonProperty("current_weather")] 
    public CurrentWeather CurrentWeather { get; set; } = new();
}

public class CurrentWeatherUnits
{
    [JsonProperty("time")]
    public string? Time { get; set; }

    [JsonProperty("interval")]
    public string? Interval { get; set; }

    [JsonProperty("temperature")]
    public string? Temperature { get; set; }

    [JsonProperty("windspeed")]
    public string? Windspeed { get; set; }

    [JsonProperty("winddirection")]
    public string? Winddirection { get; set; }

    [JsonProperty("is_day")]
    public string? IsDay { get; set; }

    [JsonProperty("weathercode")]
    public string? WeatherCode { get; set; }
}

public class CurrentWeather
{
    [JsonProperty("time")]
    public string? Time { get; set; }

    [JsonProperty("interval")]
    public int? Interval { get; set; }

    [JsonProperty("temperature")]
    public double? Temperature { get; set; }

    [JsonProperty("windspeed")]
    public double? Windspeed { get; set; }

    [JsonProperty("winddirection")]
    public int? Winddirection { get; set; }

    [JsonProperty("is_day")]
    public int? IsDay { get; set; }

    [JsonProperty("weathercode")]
    public int? WeatherCode { get; set; }
}
