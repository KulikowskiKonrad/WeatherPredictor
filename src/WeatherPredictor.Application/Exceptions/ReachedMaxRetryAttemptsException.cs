using WeatherPredictor.Domain.Exceptions;

namespace WeatherPredictor.Application.Exceptions;

public class ReachedMaxRetryAttemptsException : WeatherPredictorException
{
    internal ReachedMaxRetryAttemptsException() : base("Reached max count of retry attempts.")
    {
    }
}