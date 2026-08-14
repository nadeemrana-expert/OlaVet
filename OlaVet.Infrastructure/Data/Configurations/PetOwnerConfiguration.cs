// =============================================
// File: OlaVet.Infrastructure/Data/Configurations/PetOwnerConfiguration.cs
// Fluent API configuration for PetOwner entity
// =============================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OlaVet.Domain.Entities;

namespace OlaVet.Infrastructure.Data.Configurations;

/// <summary>
/// Configuration for PetOwner entity
/// Implements IEntityTypeConfiguration to separate configuration from DbContext
/// </summary>
public class PetOwnerConfiguration : IEntityTypeConfiguration<PetOwner>
{
    public void Configure(EntityTypeBuilder<PetOwner> builder)
    {
        // =============================================
        // TABLE CONFIGURATION
        // =============================================
        
        // Specify table name (optional - defaults to DbSet property name)
        builder.ToTable("PetOwner");
        
        // =============================================
        // PRIMARY KEY
        // =============================================
        
        builder.HasKey(p => p.PetOwnerId);
        
        // =============================================
        // PROPERTY CONFIGURATION
        // =============================================
        
        // Required properties with max length
        builder.Property(p => p.OwnerName)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(p => p.Email)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(p => p.ContactNumber)
            .IsRequired()
            .HasMaxLength(20);
        
        // Optional properties
        builder.Property(p => p.HomeAddress)
            .HasMaxLength(300);
        
        builder.Property(p => p.Gender)
            .HasMaxLength(10);
        
        // Decimal configuration - IMPORTANT for money fields
        builder.Property(p => p.Wallet)
            .HasColumnType("decimal(18,2)")  // 18 digits total, 2 after decimal
            .HasDefaultValue(0);              // Default value in database
        
        // =============================================
        // INDEXES FOR PERFORMANCE
        // =============================================
        
        // Unique index on Email (can't have duplicate emails)
        builder.HasIndex(p => p.Email)
            .IsUnique()
            .HasDatabaseName("IX_PetOwner_Email");
        
        // Unique index on ContactNumber
        builder.HasIndex(p => p.ContactNumber)
            .IsUnique()
            .HasDatabaseName("IX_PetOwner_ContactNumber");
        
        // Non-unique index on RegistrationDate for queries like "owners registered in last month"
        builder.HasIndex(p => p.RegistrationDate)
            .HasDatabaseName("IX_PetOwner_RegistrationDate");
        
        // Composite index for common query patterns
        builder.HasIndex(p => new { p.IsActive, p.RegistrationDate })
            .HasDatabaseName("IX_PetOwner_IsActive_RegistrationDate");
        
        // =============================================
        // RELATIONSHIPS
        // =============================================
        // Note: Navigation properties are configured automatically by convention
        // We only need to configure them if we want non-default behavior
        
        // One-to-Many: PetOwner → Pets
        builder.HasMany(p => p.Pets)
            .WithOne(pet => pet.PetOwner)
            .HasForeignKey(pet => pet.PetOwnerId)
            .OnDelete(DeleteBehavior.Cascade);  // Delete pets when owner is deleted
        
        // LEARNING NOTE: Why configure relationships?
        // - Control cascade behavior (Cascade, Restrict, SetNull)
        // - Specify foreign key names
        // - Configure required vs optional relationships
    }
}
