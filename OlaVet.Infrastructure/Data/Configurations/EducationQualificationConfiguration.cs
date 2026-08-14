using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OlaVet.Domain.Entities;

namespace OlaVet.Infrastructure.Data.Configurations;

public class EducationQualificationConfiguration : IEntityTypeConfiguration<EducationQualification>
{
    public void Configure(EntityTypeBuilder<EducationQualification> builder)
    {
        builder.ToTable("EducationQualification");

        builder.HasKey(eq => eq.EducationId);

        // PROPERTIES
        builder.Property(eq => eq.QualificationName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(eq => eq.Institute)
            .HasMaxLength(200);

        // INDEXES
        builder.HasIndex(eq => eq.VetId)
            .HasDatabaseName("IX_EducationQualification_VetId");

        // RELATIONSHIPS
        builder.HasOne(eq => eq.Vet)
            .WithMany(v => v.EducationQualifications)
            .HasForeignKey(eq => eq.VetId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
