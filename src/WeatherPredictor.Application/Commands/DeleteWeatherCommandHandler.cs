using WeatherPredictor.Application.Abstractions.Commands;
using WeatherPredictor.Domain.Repositories;
using WeatherPredictor.Infrastructure.Utils;

namespace WeatherPredictor.Application.Commands;

public sealed class DeleteWeatherCommandHandler : ICommandHandler<DeleteWeatherCommand>
{
    private readonly IWeatherRepository _weatherRepository;

    public DeleteWeatherCommandHandler(IWeatherRepository weatherRepository)
    {
        _weatherRepository = weatherRepository;
    }

    public async Task Handle(DeleteWeatherCommand request, CancellationToken cancellationToken)
    {
        await RetryPolicy.ExecuteWithRetryAsync(async () =>
        {
            await _weatherRepository.DeleteAsync(request.Latitude, request.Longitude);
        });
    }
}