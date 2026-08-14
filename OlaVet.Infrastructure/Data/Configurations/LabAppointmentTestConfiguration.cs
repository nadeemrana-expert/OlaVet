using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OlaVet.Domain.Entities;

namespace OlaVet.Infrastructure.Data.Configurations;

public class LabAppointmentTestConfiguration : IEntityTypeConfiguration<LabAppointmentTest>
{
    public void Configure(EntityTypeBuilder<LabAppointmentTest> builder)
    {
        builder.ToTable("LabAppointmentTest");

        builder.HasKey(lat => lat.LabAppointmentTestId);

        // =============================================
        // PROPERTIES
        // =============================================
        builder.Property(lat => lat.TestResult)
            .HasMaxLength(2000); // Results can be long text

        builder.Property(lat => lat.ResultFile)
            .HasMaxLength(500); // URL or File path

        // =============================================
        // INDEXES
        // =============================================
        // Crucial for retrieving all tests for a single appointment
        builder.HasIndex(lat => lat.LabAppointmentId)
            .HasDatabaseName("IX_LabAppointmentTest_AppointmentId");

        // =============================================
        // RELATIONSHIPS
        // =============================================
        builder.HasOne(lat => lat.LabAppointment)
            .WithMany(la => la.LabAppointmentTests)
            .HasForeignKey(lat => lat.LabAppointmentId)
            .OnDelete(DeleteBehavior.Cascade); // If appointment is deleted, delete its tests

        builder.HasOne(lat => lat.LabTest)
            .WithMany(lt => lt.LabAppointmentTests)
            .HasForeignKey(lat => lat.LabTestId)
            .OnDelete(DeleteBehavior.Restrict); // Don't delete test results if catalog item is deleted
    }
}
