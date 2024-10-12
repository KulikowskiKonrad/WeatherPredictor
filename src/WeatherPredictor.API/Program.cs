using WeatherPredictor.API.Extensions;
using WeatherPredictor.API.Middleware;
using WeatherPredictor.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

builder.Services.AddControllers();
builder.Services.AddLogging();
builder.Services.AddApplicationInsightsTelemetry(opt => { opt.EnableAdaptiveSampling = false; });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Configuration.AddCustomAzureAppConfiguration(builder.Configuration);

builder.Services.AddHttpClient();

builder.Services.AddInfrastructure(builder.Environment, builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCustomExceptionHandler();
app.UseHsts();
app.UseHttpsRedirection();

app.MapControllers();
app.MapGet("/", () => "Always On");

app.Run();