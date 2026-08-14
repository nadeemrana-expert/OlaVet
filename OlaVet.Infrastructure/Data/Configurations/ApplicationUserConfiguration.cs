// =============================================
// File: OlaVet.Infrastructure/Data/Configurations/ApplicationUserConfiguration.cs
// Fluent API configuration for ApplicationUser entity
// =============================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OlaVet.Domain.Entities;

namespace OlaVet.Infrastructure.Data.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        // Table
        builder.ToTable("ApplicationUser");
        
        // Primary Key
        builder.HasKey(u => u.UserId);
        
        // Properties
        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(256);
        
        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(512);
        
        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(u => u.PhoneNumber)
            .HasMaxLength(20);
        
        builder.Property(u => u.TwoFactorSecret)
            .HasMaxLength(512);
        
        // Indexes
        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasDatabaseName("IX_ApplicationUser_Email");
        
        // Relationships
        builder.HasOne(u => u.PetOwner)
            .WithMany()
            .HasForeignKey(u => u.PetOwnerId)
            .OnDelete(DeleteBehavior.SetNull);
        
        builder.HasOne(u => u.Vet)
            .WithMany()
            .HasForeignKey(u => u.VetId)
            .OnDelete(DeleteBehavior.SetNull);
        
        builder.HasMany(u => u.RefreshTokens)
            .WithOne(rt => rt.User)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(u => u.UserRoles)
            .WithOne(ur => ur.User)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Query Filter (soft delete)
        builder.HasQueryFilter(u => u.IsActive);
    }
}
