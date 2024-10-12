using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WeatherPredictor.Domain.Entities;

namespace WeatherPredictor.Infrastructure.DAL.Configurations;

public class WeatherConfiguration : IEntityTypeConfiguration<Weather>
{
    public void Configure(EntityTypeBuilder<Weather> builder)
    {
        builder.ToTable("Weather");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id);
        builder.Property(x => x.Latitude)
            .IsRequired();
        builder.Property(x => x.Longitude);
        builder.Property(x => x.CreateDate)
            .IsRequired();
        builder.Property(x => x.Forecast)
            .IsRequired()
            .HasMaxLength(2500);
    }
}