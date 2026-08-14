using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OlaVet.Domain.Entities;

namespace OlaVet.Infrastructure.Data.Configurations;

public class ServiceConfiguration : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> builder)
    {
        builder.ToTable("Service");

        builder.HasKey(s => s.ServiceId);

        // PROPERTIES
        builder.Property(s => s.ServiceType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.ServiceDescription)
            .HasMaxLength(500);

        builder.Property(s => s.ServiceFee)
            .HasColumnType("decimal(18,2)");

        // INDEXES
        builder.HasIndex(s => new { s.VetId, s.ServiceType })
            .HasDatabaseName("IX_Service_Vet_Type");

        // RELATIONSHIPS
        builder.HasOne(s => s.Vet)
            .WithMany(v => v.Services)
            .HasForeignKey(s => s.VetId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
