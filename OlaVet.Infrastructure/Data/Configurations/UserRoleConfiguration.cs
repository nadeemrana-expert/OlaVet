// =============================================
// File: OlaVet.Infrastructure/Data/Configurations/UserRoleConfiguration.cs
// Fluent API configuration for UserRole join entity
// =============================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OlaVet.Domain.Entities;

namespace OlaVet.Infrastructure.Data.Configurations;

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("UserRole");
        
        // Composite primary key
        builder.HasKey(ur => new { ur.UserId, ur.RoleId });
    }
}
