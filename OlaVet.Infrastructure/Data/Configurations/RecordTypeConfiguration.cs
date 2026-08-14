// =============================================
// SIMPLE LOOKUP TABLE TEMPLATE
// Use this for: MedicineType, RecordType, etc.
// =============================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OlaVet.Domain.Entities.Lookups;

namespace OlaVet.Infrastructure.Data.Configurations;

public class RecordTypeConfiguration : IEntityTypeConfiguration<RecordType>
{
    public void Configure(EntityTypeBuilder<RecordType> builder)
    {
        builder.ToTable("RecordType");
        builder.HasKey(rt => rt.RecordTypeId);
        
        builder.Property(rt => rt.TypeName)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(rt => rt.Description)
            .HasMaxLength(200);
        
        // Unique index on type name
        builder.HasIndex(rt => rt.TypeName)
            .IsUnique()
            .HasDatabaseName("IX_RecordType_TypeName");
    }
}

// Copy above for RecordType, VetAppointmentType, StatusType
// Just change entity name and adjust properties
