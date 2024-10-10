namespace WeatherPredictor.Domain.Exceptions;

public class WeatherPredictorException : Exception
{
    protected WeatherPredictorException(string message) : base(message)
    {
    }
}