// =============================================
// File: OlaVet.Infrastructure/Data/Configurations/PetConfiguration.cs
// Fluent API configuration for Pet entity
// =============================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OlaVet.Domain.Entities;

namespace OlaVet.Infrastructure.Data.Configurations;

public class PetConfiguration : IEntityTypeConfiguration<Pet>
{
    public void Configure(EntityTypeBuilder<Pet> builder)
    {
        builder.ToTable("Pet");
        
        builder.HasKey(p => p.PetId);
        
        // =============================================
        // PROPERTIES
        // =============================================
        
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(p => p.Species)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(p => p.Breed)
            .HasMaxLength(100);
        
        builder.Property(p => p.Color)
            .HasMaxLength(50);
        
        builder.Property(p => p.Gender)
            .HasMaxLength(10);
        
        builder.Property(p => p.PetWeight)
            .HasColumnType("decimal(8,2)");  // Up to 999,999.99 kg
        
        // =============================================
        // INDEXES
        // =============================================
        
        // Index for finding all pets of an owner (most common query)
        builder.HasIndex(p => p.PetOwnerId)
            .HasDatabaseName("IX_Pet_PetOwnerId");
        
        // Index for searching pets by species
        builder.HasIndex(p => p.Species)
            .HasDatabaseName("IX_Pet_Species");
        
        // Composite index for finding active pets by owner
        builder.HasIndex(p => new { p.PetOwnerId, p.IsActive })
            .HasDatabaseName("IX_Pet_PetOwnerId_IsActive");
        
        // Index on registration date for recent pets queries
        builder.HasIndex(p => p.RegistrationDate)
            .HasDatabaseName("IX_Pet_RegistrationDate");
        
        // =============================================
        // RELATIONSHIPS
        // =============================================
        
        // Many-to-One: Pet → PetOwner
        builder.HasOne(p => p.PetOwner)
            .WithMany(po => po.Pets)
            .HasForeignKey(p => p.PetOwnerId)
            .OnDelete(DeleteBehavior.Restrict);  // Don't allow deleting owner if they have pets
        
        // LEARNING NOTE: DeleteBehavior Options
        // - Cascade: Delete child when parent deleted (good for owned entities)
        // - Restrict: Prevent parent deletion if children exist (good for important data)
        // - SetNull: Set FK to null when parent deleted (good for optional relationships)
        // - NoAction: Do nothing (database handles it)
    }
}
