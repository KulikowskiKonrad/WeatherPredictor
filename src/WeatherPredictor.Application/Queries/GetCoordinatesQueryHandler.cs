using WeatherPredictor.Application.Abstractions.Queries;
using WeatherPredictor.Domain.DTO;
using WeatherPredictor.Domain.Repositories;

namespace WeatherPredictor.Application.Queries;

public sealed class GetCoordinatesQueryHandler : IQueryHandler<GetCoordinatesQuery, IEnumerable<CoordinatesDto>>
{
    private readonly IWeatherRepository _weatherRepository;

    public GetCoordinatesQueryHandler(IWeatherRepository weatherRepository)
    {
        _weatherRepository = weatherRepository;
    }

    public async Task<IEnumerable<CoordinatesDto>> Handle(GetCoordinatesQuery request,
        CancellationToken cancellationToken)
    {
        var totalItems = await _weatherRepository.GetTotalCoordinatesCountAsync();

        var skip = (request.PageNumber - 1) * request.PageSize;

        if (skip >= totalItems)
        {
            return new List<CoordinatesDto>();
        }

        return await _weatherRepository.GetUsedCoordinatesAsync(skip, request.PageSize);
    }
}