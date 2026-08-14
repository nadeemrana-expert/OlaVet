using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OlaVet.Domain.Entities;

namespace OlaVet.Infrastructure.Data.Configurations;

public class LabTestConfiguration : IEntityTypeConfiguration<LabTest>
{
    public void Configure(EntityTypeBuilder<LabTest> builder)
    {
        builder.ToTable("LabTest");

        builder.HasKey(lt => lt.LabTestId);

        // PROPERTIES
        builder.Property(lt => lt.LabTestName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(lt => lt.LabTestType)
            .HasMaxLength(100);

        builder.Property(lt => lt.LabTestDescription)
            .HasMaxLength(500);

        builder.Property(lt => lt.TestFee)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        // INDEXES
        builder.HasIndex(lt => lt.LabTestName)
            .HasDatabaseName("IX_LabTest_Name");

        builder.HasIndex(lt => lt.LabTestType)
            .HasDatabaseName("IX_LabTest_Type");

        // RELATIONSHIPS
        // Many-to-Many join table relationship
        builder.HasMany(lt => lt.LabAppointmentTests)
            .WithOne(lat => lat.LabTest)
            .HasForeignKey(lat => lat.LabTestId)
            .OnDelete(DeleteBehavior.Restrict); 
    }
}
