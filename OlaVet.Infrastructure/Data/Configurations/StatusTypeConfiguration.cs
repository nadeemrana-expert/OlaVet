// =============================================
// SIMPLE LOOKUP TABLE TEMPLATE
// Use this for: MedicineType, RecordType, etc.
// =============================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OlaVet.Domain.Entities.Lookups;

namespace OlaVet.Infrastructure.Data.Configurations;

public class StatusTypeConfiguration : IEntityTypeConfiguration<StatusType>
{
    public void Configure(EntityTypeBuilder<StatusType> builder)
    {
        builder.ToTable("StatusType");
        builder.HasKey(s => s.StatusTypeId);
        
        builder.Property(s => s.StatusName)
            .IsRequired()
            .HasMaxLength(50);

	builder.Property(s => s.AppliesTo)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(s => s.Description)
            .HasMaxLength(200);
        
        // Unique index on type name
        builder.HasIndex(s => s.StatusName)
            .IsUnique()
            .HasDatabaseName("IX_StatusType_StatusName");

	// Unique constraint: Name + AppliesTo combo should be unique 
        // (e.g., You can't have two "Pending" statuses for "Appointment")
        builder.HasIndex(s => new { s.StatusName, s.AppliesTo })
            .IsUnique()
            .HasDatabaseName("UQ_StatusType_Name_AppliesTo");
    }
}

// Copy above for RecordType, VetAppointmentType, StatusType
// Just change entity name and adjust properties
