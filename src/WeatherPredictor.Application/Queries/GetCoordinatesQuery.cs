using WeatherPredictor.Application.Abstractions.Queries;
using WeatherPredictor.Domain.DTO;

namespace WeatherPredictor.Application.Queries;

public class GetCoordinatesQuery : IQuery<IEnumerable<CoordinatesDto>>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }

    public GetCoordinatesQuery(int pageSize, int pageNumber)
    {
        PageSize = pageSize;
        PageNumber = pageNumber;
    }
}