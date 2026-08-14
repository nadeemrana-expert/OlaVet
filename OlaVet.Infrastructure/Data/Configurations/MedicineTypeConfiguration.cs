// =============================================
// SIMPLE LOOKUP TABLE TEMPLATE
// Use this for: MedicineType, RecordType, etc.
// =============================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OlaVet.Domain.Entities.Lookups;

namespace OlaVet.Infrastructure.Data.Configurations;

public class MedicineTypeConfiguration : IEntityTypeConfiguration<MedicineType>
{
    public void Configure(EntityTypeBuilder<MedicineType> builder)
    {
        builder.ToTable("MedicineType");
        builder.HasKey(mt => mt.MedicineTypeId);
        
        builder.Property(mt => mt.TypeName)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(mt => mt.Description)
            .HasMaxLength(200);
        
        // Unique index on type name
        builder.HasIndex(mt => mt.TypeName)
            .IsUnique()
            .HasDatabaseName("IX_MedicineType_TypeName");
    }
}


// Copy above for RecordType, VetAppointmentType, StatusType
// Just change entity name and adjust properties
