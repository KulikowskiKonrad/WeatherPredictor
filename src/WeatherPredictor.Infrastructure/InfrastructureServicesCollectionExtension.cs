using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WeatherPredictor.Application.Abstractions.Commands;
using WeatherPredictor.Application.Services;
using WeatherPredictor.Application.Services.Abstractions;
using WeatherPredictor.Infrastructure.DAL;
using WeatherPredictor.Infrastructure.DAL.Repositories;

namespace WeatherPredictor.Infrastructure;

public static class InfrastructureServicesCollectionExtension
{
    public static void AddInfrastructure(
        this IServiceCollection services,
        IHostEnvironment environment,
        ConfigurationManager configurationManager)
    {
        services.AddMemoryCache();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<ICommand>());

        services.AddScoped<IWeatherApiService, WeatherApiService>();

        services.AddScoped(typeof(Logger<>));

        services.AddDatabase(configurationManager, environment);
        services.AddRepositories();
    }

    private static void AddDatabase(this IServiceCollection services,
        IConfiguration configuration, IHostEnvironment environment)
    {
        services.AddDbContext<WeatherPredictorDbContext>(
            x =>
            {
                var connectionString = configuration.GetConnectionString("DefaultConnection");

                x.UseSqlServer(connectionString ??
                               throw new CouldNotGetConfigurationException(),
                    options => { options.CommandTimeout(30); });

                if (environment.IsDevelopment())
                {
                    x.EnableSensitiveDataLogging();
                }
            });
    }

    private static void AddRepositories(this IServiceCollection services)
    {
        services.Scan(scan => scan
            .FromAssemblyOf<WeatherPredictorInfrastructureAssembly>()
            .AddClasses(classes => classes.AssignableTo<IRepository>())
            .AsImplementedInterfaces()
            .WithScopedLifetime());
    }
}