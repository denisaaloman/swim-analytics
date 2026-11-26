using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwimAnalytics.Domain.Entities;

namespace SwimAnalytics.Data.Configurations;

public class SplitConfiguration : IEntityTypeConfiguration<Split>
{
    public void Configure(EntityTypeBuilder<Split> builder)
    {
        builder.Property<int>("Id");
        builder.HasKey("Id");

        builder.HasOne(split => split.Result)
               .WithMany(result => result.Splits)
               .HasForeignKey("ResultId")
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex("ResultId", nameof(Split.DistanceMeters)).IsUnique();
    }
}