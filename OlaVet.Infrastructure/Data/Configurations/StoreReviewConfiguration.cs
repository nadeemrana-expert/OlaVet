// =============================================
// REVIEW ENTITY TEMPLATE
// Use this for: VetReview, LabReview, StoreReview
// =============================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OlaVet.Domain.Entities;

namespace OlaVet.Infrastructure.Data.Configurations;

public class StoreReviewConfiguration : IEntityTypeConfiguration<StoreReview>
{
    public void Configure(EntityTypeBuilder<StoreReview> builder)
    {
        builder.ToTable("StoreReview");
        builder.HasKey(sr => sr.StoreReviewId);
        
        // Properties
        builder.Property(sr => sr.Rating)
            .IsRequired();
        
        builder.Property(sr => sr.Comments)
            .HasMaxLength(1000);
        
        // Check constraint: Rating must be 1-5
        builder.HasCheckConstraint(
            "CK_StoreReview_Rating",
            "[Rating] >= 1 AND [Rating] <= 5");
        
        // Indexes
        builder.HasIndex(sr => sr.StoreId)
            .HasDatabaseName("IX_StoreReview_StoreId");
        
        builder.HasIndex(sr => sr.Rating)
            .HasDatabaseName("IX_StoreReview_Rating");
        
        builder.HasIndex(vr => vr.ReviewDateTime)
            .HasDatabaseName("IX_StoreReview_ReviewDateTime");
        
        builder.HasIndex(sr => sr.PetOwnerId)
            .HasDatabaseName("IX_StoreReview_PetOwnerId");
        
        // Composite covering index for rating aggregation queries
        builder.HasIndex(sr => new { sr.StoreId, sr.Rating })
            .HasDatabaseName("IX_StoreReview_StoreId_Rating");
    }
}
