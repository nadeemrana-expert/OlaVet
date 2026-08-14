// =============================================
// SIMPLE LOOKUP TABLE TEMPLATE
// Use this for: MedicineType, RecordType, etc.
// =============================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OlaVet.Domain.Entities.Lookups;

namespace OlaVet.Infrastructure.Data.Configurations;

public class VetAppointmentTypeTypeConfiguration : IEntityTypeConfiguration<VetAppointmentType>
{
    public void Configure(EntityTypeBuilder<VetAppointmentType> builder)
    {
        builder.ToTable("VetAppointmentType");
        builder.HasKey(vt => vt.VetAppointmentTypeId);
        
        builder.Property(vt => vt.TypeName)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(vt => vt.Description)
            .HasMaxLength(200);
        
        // Unique index on type name
        builder.HasIndex(vt => vt.TypeName)
            .IsUnique()
            .HasDatabaseName("IX_VetAppointmentType_TypeName");
    }
}

// Copy above for RecordType, VetAppointmentType, StatusType
// Just change entity name and adjust properties
