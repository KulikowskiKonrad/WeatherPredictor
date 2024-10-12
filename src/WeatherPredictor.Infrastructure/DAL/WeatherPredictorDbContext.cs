using Microsoft.EntityFrameworkCore;
using WeatherPredictor.Domain.Entities;

namespace WeatherPredictor.Infrastructure.DAL;

public class WeatherPredictorDbContext : DbContext
{
    public DbSet<Weather> Weathers { get; set; }

    public WeatherPredictorDbContext(DbContextOptions<WeatherPredictorDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(WeatherPredictorInfrastructureAssembly.Assembly);
    }
}