// =============================================
// File: OlaVet.Infrastructure/Data/Configurations/RefreshTokenConfiguration.cs
// Fluent API configuration for RefreshToken entity
// =============================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OlaVet.Domain.Entities;

namespace OlaVet.Infrastructure.Data.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshToken");
        
        builder.HasKey(rt => rt.RefreshTokenId);
        
        builder.Property(rt => rt.Token)
            .IsRequired()
            .HasMaxLength(512);
        
        builder.Property(rt => rt.CreatedByIp)
            .HasMaxLength(50);
        
        builder.Property(rt => rt.RevokedByIp)
            .HasMaxLength(50);
        
        builder.Property(rt => rt.ReplacedByToken)
            .HasMaxLength(512);
        
        builder.Property(rt => rt.RevokeReason)
            .HasMaxLength(256);
        
        // Index for fast lookup by token
        builder.HasIndex(rt => rt.Token)
            .HasDatabaseName("IX_RefreshToken_Token");
        
        // Ignore computed properties
        builder.Ignore(rt => rt.IsExpired);
        builder.Ignore(rt => rt.IsRevoked);
        builder.Ignore(rt => rt.IsActive);
    }
}
