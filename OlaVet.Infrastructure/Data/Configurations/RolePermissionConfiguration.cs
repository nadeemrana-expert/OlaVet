// =============================================
// File: OlaVet.Infrastructure/Data/Configurations/RolePermissionConfiguration.cs
// Fluent API configuration for RolePermission join entity
// =============================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OlaVet.Domain.Entities;

namespace OlaVet.Infrastructure.Data.Configurations;

public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("RolePermission");
        
        // Composite primary key
        builder.HasKey(rp => new { rp.RoleId, rp.PermissionId });
    }
}
