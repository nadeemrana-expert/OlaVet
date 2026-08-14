// =============================================
// File: OlaVet.Infrastructure/Data/Configurations/MedicineConfiguration.cs
// Fluent API configuration for Medicine entity
// =============================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OlaVet.Domain.Entities;

namespace OlaVet.Infrastructure.Data.Configurations;

public class MedicineConfiguration : IEntityTypeConfiguration<Medicine>
{
    public void Configure(EntityTypeBuilder<Medicine> builder)
    {
        builder.ToTable("Medicine");
        
        builder.HasKey(m => m.MedicineId);
        
        // =============================================
        // PROPERTIES
        // =============================================
        
        builder.Property(m => m.MedicineName)
            .IsRequired()
            .HasMaxLength(150);
        
        builder.Property(m => m.Price)
            .HasColumnType("decimal(18,2)")
            .IsRequired();
        
        builder.Property(m => m.Manufacturer)
            .HasMaxLength(100);
        
        builder.Property(m => m.Description)
            .HasMaxLength(500);
        
        // =============================================
        // INDEXES
        // =============================================
        
        // Index for searching medicines by name
        builder.HasIndex(m => m.MedicineName)
            .HasDatabaseName("IX_Medicine_MedicineName");
        
        // Index for filtering by medicine type
        builder.HasIndex(m => m.MedicineTypeId)
            .HasDatabaseName("IX_Medicine_MedicineTypeId");
        
        // Index for active medicines
        builder.HasIndex(m => m.IsActive)
            .HasDatabaseName("IX_Medicine_IsActive");
        
        // Composite index for finding active medicines by type
        builder.HasIndex(m => new { m.IsActive, m.MedicineTypeId })
            .HasDatabaseName("IX_Medicine_IsActive_MedicineTypeId");
        
        // =============================================
        // RELATIONSHIPS
        // =============================================
        
        // Many-to-One: Medicine → MedicineType
        builder.HasOne(m => m.MedicineType)
            .WithMany(mt => mt.Medicines)
            .HasForeignKey(m => m.MedicineTypeId)
            .OnDelete(DeleteBehavior.SetNull);  // Don't delete medicines if type deleted, just set to null
    }
}
