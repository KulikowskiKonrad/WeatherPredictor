using WeatherPredictor.Application.Abstractions.Commands;

namespace WeatherPredictor.Application.Commands;

public class SaveWeatherCommand : ICommand
{
    public WeatherDetailsCommand WeatherDetails { get; } = new();

    public class WeatherDetailsCommand
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double? GenerationTimeMs { get; set; }
        public int? UtcOffsetSeconds { get; set; }
        public string? Timezone { get; set; }
        public string? TimezoneAbbreviation { get; set; }
        public double? Elevation { get; set; }
        public CurrentWeatherUnitsCommand CurrentWeatherUnits { get; set; } = new();
        public CurrentWeatherCommand CurrentWeather { get; set; } = new();
    }

    public class CurrentWeatherUnitsCommand
    {
        public string? Time { get; set; }
        public string? Interval { get; set; }
        public string? Temperature { get; set; }
        public string? Windspeed { get; set; }
        public string? Winddirection { get; set; }
        public bool? IsDay { get; set; }
        public string? WeatherCode { get; set; }
    }

    public class CurrentWeatherCommand
    {
        public string? Time { get; set; }
        public int? Interval { get; set; }
        public double? Temperature { get; set; }
        public double? Windspeed { get; set; }
        public int? Winddirection { get; set; }
        public bool? IsDay { get; set; }
        public int? WeatherCode { get; set; }
    }

    public SaveWeatherCommand(WeatherDetailsCommand weatherDetails)
    {
        WeatherDetails = weatherDetails;
    }
}