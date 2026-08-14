// =============================================
// LAB CONFIGURATION (Similar to Vet)
// =============================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OlaVet.Domain.Entities;

namespace OlaVet.Infrastructure.Data.Configurations;

public class LabConfiguration : IEntityTypeConfiguration<Lab>
{
    public void Configure(EntityTypeBuilder<Lab> builder)
    {
        builder.ToTable("Lab");
        builder.HasKey(l => l.LabId);
        
        builder.Property(l => l.LabName)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(l => l.LabAddress)
            .HasMaxLength(300);
        
        builder.Property(l => l.ContactNumber)
            .IsRequired()
            .HasMaxLength(20);
        
        builder.Property(l => l.Discount)
            .HasColumnType("decimal(5,2)")
            .HasDefaultValue(0);
        
        builder.Property(l => l.Specialization)
            .HasMaxLength(200);
        
        // Indexes
        builder.HasIndex(l => l.ContactNumber)
            .IsUnique();
        
        builder.HasIndex(l => l.IsActive);
        
        builder.HasIndex(l => new { l.IsActive, l.Specialization })
            .HasDatabaseName("IX_Lab_IsActive_Specialization");
    }
}
