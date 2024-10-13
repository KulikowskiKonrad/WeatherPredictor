using WeatherPredictor.Application.Abstractions.Commands;
using WeatherPredictor.Domain.Repositories;

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
        await _weatherRepository.DeleteAsync(request.Latitude, request.Longitude);
    }
}