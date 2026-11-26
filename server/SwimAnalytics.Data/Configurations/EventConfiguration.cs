using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwimAnalytics.Domain.Entities;
namespace SwimAnalytics.Data.Configurations;

public class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.Property<int>("Id");
        builder.HasKey("Id");

        // aceeași probă nu poate exista de două ori
        builder.HasIndex(e => new { e.Stroke, e.DistanceMeters, e.Gender }).IsUnique();
    }
}