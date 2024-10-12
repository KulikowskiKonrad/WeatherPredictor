﻿using MediatR;

namespace WeatherPredictor.Application.Abstractions.Queries;

public interface IQuery<out TResult> : IRequest<TResult>
{
}