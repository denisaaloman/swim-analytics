using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwimAnalytics.Domain.Entities;
namespace SwimAnalytics.Data.Configurations;

public class ResultConfiguration : IEntityTypeConfiguration<Result>
{
    public void Configure(EntityTypeBuilder<Result> builder)
    {
        builder.Property<int>("Id");
        builder.HasKey("Id");

        builder.HasOne(result => result.Swimmer)
               .WithMany(swimmer => swimmer.Results)
               .HasForeignKey("SwimmerId")
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(result => result.Event)
               .WithMany(e => e.Results)
               .HasForeignKey("EventId")
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(result => result.Meet)
               .WithMany(meet => meet.Results)
               .HasForeignKey("MeetId")
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex("EventId", "TimeMs");
        builder.HasIndex("SwimmerId", "EventId");
        builder.HasIndex("MeetId");
    }
}