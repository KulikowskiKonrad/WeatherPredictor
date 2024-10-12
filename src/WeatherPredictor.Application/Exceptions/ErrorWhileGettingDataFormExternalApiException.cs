using WeatherPredictor.Domain.Exceptions;

namespace WeatherPredictor.Application.Exceptions;

public class ErrorWhileGettingDataFormExternalApiException : WeatherPredictorException
{
    internal ErrorWhileGettingDataFormExternalApiException() : base("Error while getting data from eternal API.")
    {
    }
}