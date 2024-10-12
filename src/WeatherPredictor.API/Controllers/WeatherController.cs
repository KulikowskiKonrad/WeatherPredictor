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

        var result = await _mediator.Send(query);

        if (result == null)
        {
            return NotFound();
        }

        return Ok();
    }

    [HttpGet("coordinates")]
    public async Task<ActionResult<List<CoordinatesDto>>> GetCoordinatesList()
    {
        var query = new GetCoordinatesQuery();

        var result = await _mediator.Send(query);
        
        if(result == null)
        {
            return NotFound();
        }
        
        return Ok();
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
                    Time = request.CurrentWeatherUnits.Time,
                    Interval = request.CurrentWeatherUnits.Interval,
                    Temperature = request.CurrentWeatherUnits.Temperature,
                    Windspeed = request.CurrentWeatherUnits.Windspeed,
                    Winddirection = request.CurrentWeatherUnits.Winddirection,
                    IsDay = request.CurrentWeatherUnits.IsDay,
                    WeatherCode = request.CurrentWeatherUnits.WeatherCode
                },
                CurrentWeather = new SaveWeatherCommand.CurrentWeatherCommand
                {
                    Time = request.CurrentWeather.Time,
                    Interval = request.CurrentWeather.Interval,
                    Temperature = request.CurrentWeather.Temperature,
                    Windspeed = request.CurrentWeather.Windspeed,
                    Winddirection = request.CurrentWeather.Winddirection,
                    IsDay = request.CurrentWeather.IsDay,
                    WeatherCode = request.CurrentWeather.WeatherCode
                }
            });

        await _mediator.Send(command);
        return Ok();
    }
}