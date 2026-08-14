// =============================================
// File: OlaVet.Infrastructure/Data/Configurations/VetConfiguration.cs
// Fluent API configuration for Vet entity
// =============================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OlaVet.Domain.Entities;

namespace OlaVet.Infrastructure.Data.Configurations;

public class VetConfiguration : IEntityTypeConfiguration<Vet>
{
    public void Configure(EntityTypeBuilder<Vet> builder)
    {
        builder.ToTable("Vet");
        
        builder.HasKey(v => v.VetId);
        
        // =============================================
        // PROPERTIES
        // =============================================
        
        builder.Property(v => v.VetName)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(v => v.Specialization)
            .HasMaxLength(100);
        
        builder.Property(v => v.ClinicLocation)
            .HasMaxLength(300);
        
        builder.Property(v => v.Fee)
            .HasColumnType("decimal(18,2)")
            .IsRequired();
        
        builder.Property(v => v.ContactNumber)
            .IsRequired()
            .HasMaxLength(20);
        
        builder.Property(v => v.Email)
            .HasMaxLength(100);
        
        builder.Property(v => v.LicenseNumber)
            .HasMaxLength(50);
        
        // =============================================
        // INDEXES
        // =============================================
        
        builder.HasIndex(v => v.ContactNumber)
            .IsUnique();
        
        // Index for searching by specialization
        builder.HasIndex(v => v.Specialization)
            .HasDatabaseName("IX_Vet_Specialization");
        
        // Composite index for active vets by specialization
        builder.HasIndex(v => new { v.IsActive, v.Specialization })
            .HasDatabaseName("IX_Vet_IsActive_Specialization");
        
        // Index for filtering by experience
        builder.HasIndex(v => v.YearsOfExperience)
            .HasDatabaseName("IX_Vet_YearsOfExperience");
        
        // =============================================
        // RELATIONSHIPS
        // =============================================
        
        // One-to-Many: Vet → EducationQualifications
        builder.HasMany(v => v.EducationQualifications)
            .WithOne(eq => eq.Vet)
            .HasForeignKey(eq => eq.VetId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // One-to-Many: Vet → Services
        builder.HasMany(v => v.Services)
            .WithOne(s => s.Vet)
            .HasForeignKey(s => s.VetId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // One-to-Many: Vet → Availabilities
        builder.HasMany(v => v.Availabilities)
            .WithOne(a => a.Vet)
            .HasForeignKey(a => a.VetId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

