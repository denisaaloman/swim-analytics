using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwimAnalytics.Domain.Entities;

namespace SwimAnalytics.Data.Configurations;

public class BaseTimeConfiguration : IEntityTypeConfiguration<BaseTime>
{
    public void Configure(EntityTypeBuilder<BaseTime> builder)
    {
        builder.Property<int>("Id");
        builder.HasKey("Id");

        builder.HasOne(baseTime => baseTime.Event)
               .WithMany(e => e.BaseTimes)
               .HasForeignKey("EventId")
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex("EventId", nameof(BaseTime.PoolLength)).IsUnique();
    }
}