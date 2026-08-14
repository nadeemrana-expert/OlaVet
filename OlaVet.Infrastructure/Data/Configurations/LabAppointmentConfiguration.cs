using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OlaVet.Domain.Entities;

namespace OlaVet.Infrastructure.Data.Configurations;

public class LabAppointmentConfiguration : IEntityTypeConfiguration<LabAppointment>
{
    public void Configure(EntityTypeBuilder<LabAppointment> builder)
    {
        builder.ToTable("LabAppointment");

        builder.HasKey(la => la.LabAppointmentId);

        // =============================================
        // PROPERTIES
        // =============================================
        builder.Property(la => la.Notes)
            .HasMaxLength(1000);

        builder.Property(la => la.AppointmentDateTime)
            .IsRequired();

        builder.Property(la => la.CreatedDate)
            .HasDefaultValueSql("GETUTCDATE()");

        // =============================================
        // CRITICAL INDEXES
        // =============================================
        builder.HasIndex(la => la.AppointmentDateTime)
            .HasDatabaseName("IX_LabAppointment_AppointmentDateTime");

        builder.HasIndex(la => la.StatusTypeId)
            .HasDatabaseName("IX_LabAppointment_StatusTypeId");

        // Composite: "Find lab appointments for this owner on this date"
        builder.HasIndex(la => new { la.PetOwnerId, la.AppointmentDateTime })
            .HasDatabaseName("IX_LabAppointment_Owner_DateTime");

        // Covering Index for Lab Dashboard (Status + Date + Lab filtering)
        builder.HasIndex(la => new { la.LabId, la.StatusTypeId, la.AppointmentDateTime })
            .IncludeProperties(la => new { la.PetId, la.Notes })
            .HasDatabaseName("IX_LabAppointment_Lab_Status_Date_Covering");

        // =============================================
        // RELATIONSHIPS
        // =============================================
        builder.HasOne(la => la.Pet)
            .WithMany(p => p.LabAppointments)
            .HasForeignKey(la => la.PetId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(la => la.PetOwner)
            .WithMany(po => po.LabAppointments)
            .HasForeignKey(la => la.PetOwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(la => la.Lab)
            .WithMany(l => l.LabAppointments)
            .HasForeignKey(la => la.LabId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(la => la.StatusType)
            .WithMany(st => st.LabAppointments)
            .HasForeignKey(la => la.StatusTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        // One-to-One: LabAppointment ↔ LabPayment
        builder.HasOne(la => la.LabPayment)
            .WithOne(lp => lp.LabAppointment)
            .HasForeignKey<LabPayment>(lp => lp.LabAppointmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
