using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OlaVet.Domain.Entities;

namespace OlaVet.Infrastructure.Data.Configurations;

public class VetAvailabilityConfiguration : IEntityTypeConfiguration<VetAvailability>
{
    public void Configure(EntityTypeBuilder<VetAvailability> builder)
    {
        builder.ToTable("VetAvailability");

        builder.HasKey(va => va.AvailabilityId);

        // PROPERTIES
        builder.Property(va => va.DayOfWeek)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(va => va.StartTime)
            .IsRequired();

        builder.Property(va => va.EndTime)
            .IsRequired();

        // INDEXES
        builder.HasIndex(va => new { va.VetId, va.DayOfWeek })
            .HasDatabaseName("IX_VetAvailability_Vet_Day");

        // RELATIONSHIPS
        builder.HasOne(va => va.Vet)
            .WithMany(v => v.Availabilities)
            .HasForeignKey(va => va.VetId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
