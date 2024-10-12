using MediatR;

namespace WeatherPredictor.Application.Abstractions.Commands;

public interface ICommand : IRequest
{
}

public interface ICommand<out TResult> : IRequest<TResult>
{
}