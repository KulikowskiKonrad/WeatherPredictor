﻿using MediatR;

namespace WeatherPredictor.Application.Abstractions.Queries;

public interface IQueryHandler<in TQuery, TResult> :
    IRequestHandler<TQuery, TResult> where TQuery : IQuery<TResult>
{
}