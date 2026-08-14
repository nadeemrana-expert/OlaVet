// =============================================
// File: OlaVet.Infrastructure/Data/Configurations/PermissionConfiguration.cs
// Fluent API configuration for Permission entity
// =============================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OlaVet.Domain.Entities;

namespace OlaVet.Infrastructure.Data.Configurations;

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("Permission");
        
        builder.HasKey(p => p.PermissionId);
        
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(p => p.Description)
            .HasMaxLength(300);
        
        builder.Property(p => p.Category)
            .HasMaxLength(50);
        
        builder.HasIndex(p => p.Name)
            .IsUnique()
            .HasDatabaseName("IX_Permission_Name");
        
        builder.HasMany(p => p.RolePermissions)
            .WithOne(rp => rp.Permission)
            .HasForeignKey(rp => rp.PermissionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
