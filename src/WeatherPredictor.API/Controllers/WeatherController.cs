using MediatR;
using Microsoft.AspNetCore.Mvc;
using WeatherPredictor.API.Requests;
using WeatherPredictor.Application.Commands;
using WeatherPredictor.Application.Queries;
using WeatherPredictor.Domain.DTO;

namespace WeatherPredictor.API.Controllers;

[ApiController]
[Route("api/weather")]
public class WeatherController : ControllerBase
{
    private readonly IMediator _mediator;

    public WeatherController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("details/{latitude:double}/{longitude:double}")]
    public async Task<ActionResult<WeatherDetailsDto>> GetWeatherDetails(double latitude, double longitude)
    {
        var query = new GetWeatherDetailsQuery(latitude, longitude);

        return Ok(await _mediator.Send(query));
    }

    [HttpGet("coordinates")]
    public async Task<ActionResult<List<CoordinatesDto>>> GetCoordinatesList()
    {
        var query = new GetCoordinatesQuery();
    
        return Ok(await _mediator.Send(query));
    }

    [HttpDelete]
    public async Task<ActionResult> DeleteWeatherDetails([FromBody] RemoveWeatherRequest request)
    {
        var command = new DeleteWeatherCommand(request.Latitude, request.Longitude);

        await _mediator.Send(command);
        return Ok();
    }

    [HttpPut]
    public async Task<ActionResult> AddWeather([FromBody] AddWeatherRequest request)
    {
        var command = new SaveWeatherCommand(
            new SaveWeatherCommand.WeatherDetailsCommand
            {
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                GenerationTimeMs = request.GenerationTimeMs,
                UtcOffsetSeconds = request.UtcOffsetSeconds,
                Timezone = request.Timezone,
                TimezoneAbbreviation = request.TimezoneAbbreviation,
                Elevation = request.Elevation,
                CurrentWeatherUnits = new SaveWeatherCommand.CurrentWeatherUnitsCommand
                {
                    Time = request.CurrentWeatherUnitsDto.Time,
                    Interval = request.CurrentWeatherUnitsDto.Interval,
                    Temperature = request.CurrentWeatherUnitsDto.Temperature,
                    Windspeed = request.CurrentWeatherUnitsDto.Windspeed,
                    Winddirection = request.CurrentWeatherUnitsDto.Winddirection,
                    IsDay = request.CurrentWeatherUnitsDto.IsDay,
                    WeatherCode = request.CurrentWeatherUnitsDto.WeatherCode
                },
                CurrentWeather = new SaveWeatherCommand.CurrentWeatherCommand
                {
                    Time = request.CurrentWeatherDto.Time,
                    Interval = request.CurrentWeatherDto.Interval,
                    Temperature = request.CurrentWeatherDto.Temperature,
                    Windspeed = request.CurrentWeatherDto.Windspeed,
                    Winddirection = request.CurrentWeatherDto.Winddirection,
                    IsDay = request.CurrentWeatherDto.IsDay,
                    WeatherCode = request.CurrentWeatherDto.WeatherCode
                }
            });

        await _mediator.Send(command);
        return Ok();
    }
}