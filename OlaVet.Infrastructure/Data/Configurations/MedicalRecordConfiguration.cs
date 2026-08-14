using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OlaVet.Domain.Entities;

namespace OlaVet.Infrastructure.Data.Configurations;

public class MedicalRecordConfiguration : IEntityTypeConfiguration<MedicalRecord>
{
    public void Configure(EntityTypeBuilder<MedicalRecord> builder)
    {
        builder.ToTable("MedicalRecord");

        builder.HasKey(mr => mr.RecordId);

        // =============================================
        // PROPERTIES
        // =============================================
        builder.Property(mr => mr.Diagnosis)
            .HasMaxLength(500);

        builder.Property(mr => mr.TreatmentDescription)
            .HasMaxLength(2000);

        builder.Property(mr => mr.AttachmentPath)
            .HasMaxLength(500);

        builder.Property(mr => mr.RecordDate)
            .HasDefaultValueSql("GETUTCDATE()");

        // =============================================
        // CRITICAL INDEXES
        // =============================================
        // 1. Primary query: "Show me this pet's medical history"
        builder.HasIndex(mr => new { mr.PetId, mr.RecordDate })
            .HasDatabaseName("IX_MedicalRecord_Pet_Date");

        // 2. Filter by record type (Vaccination, Surgery, etc.)
        builder.HasIndex(mr => mr.RecordTypeId)
            .HasDatabaseName("IX_MedicalRecord_RecordTypeId");

        // 3. Covering Index for Medical History Timeline
        builder.HasIndex(mr => new { mr.PetId, mr.RecordTypeId })
            .IncludeProperties(mr => new { mr.RecordDate, mr.Diagnosis })
            .HasDatabaseName("IX_MedicalRecord_Timeline_Covering");

        // =============================================
        // RELATIONSHIPS
        // =============================================
        builder.HasOne(mr => mr.Pet)
            .WithMany(p => p.MedicalRecords)
            .HasForeignKey(mr => mr.PetId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(mr => mr.PetOwner)
            .WithMany(po => po.MedicalRecords)
            .HasForeignKey(mr => mr.PetOwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(mr => mr.RecordType)
            .WithMany(rt => rt.MedicalRecords)
            .HasForeignKey(mr => mr.RecordTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(mr => mr.Vet)
            .WithMany(v => v.MedicalRecords)
            .HasForeignKey(mr => mr.VetId)
            .OnDelete(DeleteBehavior.SetNull); // Keep record even if vet leaves the system
    }
}
