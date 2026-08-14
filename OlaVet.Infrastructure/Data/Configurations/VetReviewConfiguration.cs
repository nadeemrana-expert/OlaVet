// =============================================
// REVIEW ENTITY TEMPLATE
// Use this for: VetReview, LabReview, StoreReview
// =============================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OlaVet.Domain.Entities;

namespace OlaVet.Infrastructure.Data.Configurations;

public class VetReviewConfiguration : IEntityTypeConfiguration<VetReview>
{
    public void Configure(EntityTypeBuilder<VetReview> builder)
    {
        builder.ToTable("VetReview");
        builder.HasKey(vr => vr.VetReviewId);
        
        // Properties
        builder.Property(vr => vr.Rating)
            .IsRequired();
        
        builder.Property(vr => vr.Comments)
            .HasMaxLength(1000);
        
        // Check constraint: Rating must be 1-5
        builder.HasCheckConstraint(
            "CK_VetReview_Rating",
            "[Rating] >= 1 AND [Rating] <= 5");
        
        // Indexes
        builder.HasIndex(vr => vr.VetId)
            .HasDatabaseName("IX_VetReview_VetId");
        
        builder.HasIndex(vr => vr.Rating)
            .HasDatabaseName("IX_VetReview_Rating");
        
        builder.HasIndex(vr => vr.ReviewDateTime)
            .HasDatabaseName("IX_VetReview_ReviewDateTime");
        
        builder.HasIndex(vr => vr.PetOwnerId)
            .HasDatabaseName("IX_VetReview_PetOwnerId");
        
        // Composite covering index for rating aggregation queries
        builder.HasIndex(vr => new { vr.VetId, vr.Rating })
            .HasDatabaseName("IX_VetReview_VetId_Rating");
    }
}
