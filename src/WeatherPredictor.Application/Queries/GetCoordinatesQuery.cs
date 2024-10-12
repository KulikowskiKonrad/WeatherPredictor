using WeatherPredictor.Application.Abstractions.Queries;
using WeatherPredictor.Domain.DTO;

namespace WeatherPredictor.Application.Queries;

public class GetCoordinatesQuery : IQuery<List<CoordinatesDto>>
{
}