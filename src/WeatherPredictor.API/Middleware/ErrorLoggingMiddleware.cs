using WeatherPredictor.Domain.Exceptions;

namespace WeatherPredictor.API.Middleware;

public class ErrorLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, ILogger<ErrorLoggingMiddleware> logger,
        IWebHostEnvironment environment)
    {
        try
        {
            await _next(context);
        }
        catch (WeatherPredictorException e)
        {
            // Our internal exceptions
            await ProcessException(context, StatusCodes.Status400BadRequest, e.Message);
        }
        catch (OperationCanceledException)
        {
            // Operation canceled exceptions. We don't want to log those anywhere
            await ProcessException(context, StatusCodes.Status500InternalServerError, "Operation canceled.");
        }
        catch (Exception ex)
        {
            if (environment.IsDevelopment())
            {
                throw;
            }

            logger.LogError(ex, ex.Message);

            await ProcessException(context, StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
        }
    }

    private static async Task ProcessException(HttpContext context, int statusCode, string message)
    {
        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsync(message);
    }
}