using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwimAnalytics.Domain.Entities;
namespace SwimAnalytics.Data.Configurations;

public class MeetConfiguration : IEntityTypeConfiguration<Meet>
{
    public void Configure(EntityTypeBuilder<Meet> builder)
    {
        builder.Property<int>("Id");
        builder.HasKey("Id");

        builder.Property(meet => meet.Name).IsRequired().HasMaxLength(150);
        builder.Property(meet => meet.City).HasMaxLength(100);
        builder.HasIndex(meet => meet.Date);
    }
}