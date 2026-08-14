// =============================================
// REVIEW ENTITY TEMPLATE
// Use this for: VetReview, LabReview, StoreReview
// =============================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OlaVet.Domain.Entities;

namespace OlaVet.Infrastructure.Data.Configurations;

public class LabReviewConfiguration : IEntityTypeConfiguration<LabReview>
{
    public void Configure(EntityTypeBuilder<LabReview> builder)
    {
        builder.ToTable("LabReview");
        builder.HasKey(lr => lr.LabReviewId);
        
        // Properties
        builder.Property(lr => lr.Rating)
            .IsRequired();
        
        builder.Property(lr => lr.Comments)
            .HasMaxLength(1000);
        
        // Check constraint: Rating must be 1-5
        builder.HasCheckConstraint(
            "CK_LabReview_Rating",
            "[Rating] >= 1 AND [Rating] <= 5");
        
        // Indexes
        builder.HasIndex(lr => lr.LabId)
            .HasDatabaseName("IX_LabReview_LabId");
        
        builder.HasIndex(lr => lr.Rating)
            .HasDatabaseName("IX_LabReview_Rating");
        
        builder.HasIndex(lr => lr.ReviewDateTime)
            .HasDatabaseName("IX_LabReview_ReviewDateTime");
        
        builder.HasIndex(lr => lr.PetOwnerId)
            .HasDatabaseName("IX_LabReview_PetOwnerId");
        
        // Composite covering index for rating aggregation queries
        builder.HasIndex(lr => new { lr.LabId, lr.Rating })
            .HasDatabaseName("IX_LabReview_LabId_Rating");
    }
}
