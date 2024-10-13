using WeatherPredictor.API.Middleware;
using WeatherPredictor.Application.OtionsSettings;
using WeatherPredictor.Application.Services;
using WeatherPredictor.Application.Services.Abstractions;
using WeatherPredictor.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddLogging();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<OpenMeteoOptions>(
    builder.Configuration.GetSection(OpenMeteoOptions.OpenMeteo));

builder.Services.AddHttpClient<IOpenMeteoWeatherApiService, OpenMeteoWeatherApiService>((client) =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddInfrastructure(builder.Environment, builder.Configuration);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/", "Weather predictor"); });

app.UseCustomExceptionHandler();
app.UseHsts();
app.UseHttpsRedirection();

app.MapControllers();
app.MapGet("/", () => "Always On");

app.Run();