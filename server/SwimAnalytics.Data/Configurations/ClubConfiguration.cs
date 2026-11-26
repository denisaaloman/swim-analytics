using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwimAnalytics.Domain.Entities;

namespace SwimAnalytics.Data.Configurations;

public class ClubConfiguration : IEntityTypeConfiguration<Club>
{
    public void Configure(EntityTypeBuilder<Club> builder)
    {
        builder.Property<int>("Id");
        builder.HasKey("Id");

        builder.Property(club => club.Name).IsRequired().HasMaxLength(100);
        builder.Property(club => club.City).HasMaxLength(100);
    }
}