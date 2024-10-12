using WeatherPredictor.Application.Abstractions.Queries;
using WeatherPredictor.Domain.DTO;
using WeatherPredictor.Domain.Repositories;
using WeatherPredictor.Infrastructure.Utils;

namespace WeatherPredictor.Application.Queries;

public sealed class GetCoordinatesQueryHandler : IQueryHandler<GetCoordinatesQuery, List<CoordinatesDto>>
{
    private readonly IWeatherRepository _weatherRepository;

    public GetCoordinatesQueryHandler(IWeatherRepository weatherRepository)
    {
        _weatherRepository = weatherRepository;
    }

    public async Task<List<CoordinatesDto>> Handle(GetCoordinatesQuery request, CancellationToken cancellationToken)
    {
        return await RetryPolicy.ExecuteWithRetryAsync(async () => await _weatherRepository.GetUsedCoordinatesAsync());
    }
}