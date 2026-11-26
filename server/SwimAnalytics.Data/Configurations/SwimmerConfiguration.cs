using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwimAnalytics.Domain.Entities;

public class SwimmerConfiguration : IEntityTypeConfiguration<Swimmer>
{
    public void Configure(EntityTypeBuilder<Swimmer> builder)
    {
        builder.Property<int>("Id");
        builder.HasKey("Id");

        builder.Property(swimmer => swimmer.FirstName).IsRequired().HasMaxLength(50);
        builder.Property(swimmer => swimmer.LastName).IsRequired().HasMaxLength(50);

        builder.HasOne(swimmer => swimmer.Club)
               .WithMany(club => club.Swimmers)
               .HasForeignKey("ClubId")
               .IsRequired(false)
               .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex("ClubId");
        builder.HasIndex(swimmer => new { swimmer.LastName, swimmer.FirstName });
    }
}