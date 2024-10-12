namespace WeatherPredictor.Domain.Entities;

public class Weather
{
    public int Id { get; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string Forecast { get; set; }
    public DateTime CreateDate { get; set; }
}