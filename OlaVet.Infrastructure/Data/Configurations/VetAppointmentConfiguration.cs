// =============================================
// File: OlaVet.Infrastructure/Data/Configurations/VetAppointmentConfiguration.cs
// Configuration for VetAppointment - Most important transactional entity
// =============================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OlaVet.Domain.Entities;

namespace OlaVet.Infrastructure.Data.Configurations;

public class VetAppointmentConfiguration : IEntityTypeConfiguration<VetAppointment>
{
    public void Configure(EntityTypeBuilder<VetAppointment> builder)
    {
        builder.ToTable("VetAppointment");
        
        builder.HasKey(va => va.VetAppointmentId);
        
        // =============================================
        // PROPERTIES
        // =============================================
        
        builder.Property(va => va.Reason)
            .HasMaxLength(500);
        
        builder.Property(va => va.Notes)
            .HasMaxLength(1000);
        
        builder.Property(va => va.AppointmentDateTime)
            .IsRequired();
        
        // =============================================
        // CRITICAL INDEXES FOR PERFORMANCE
        // =============================================
        // These indexes are crucial for a production system with 500K+ appointments
        
        // 1. Index on AppointmentDateTime - Most common query: "appointments today/this week"
        builder.HasIndex(va => va.AppointmentDateTime)
            .HasDatabaseName("IX_VetAppointment_AppointmentDateTime");
        
        // 2. Index on StatusTypeId - Filter by status (Scheduled, Completed, etc.)
        builder.HasIndex(va => va.StatusTypeId)
            .HasDatabaseName("IX_VetAppointment_StatusTypeId");
        
        // 3. Index on VetId - "Show all appointments for this vet"
        builder.HasIndex(va => va.VetId)
            .HasDatabaseName("IX_VetAppointment_VetId");
        
        // 4. Index on PetOwnerId - "Show my appointments"
        builder.HasIndex(va => va.PetOwnerId)
            .HasDatabaseName("IX_VetAppointment_PetOwnerId");
        
        // 5. COMPOSITE INDEX - Most powerful for common queries
        // "Show scheduled appointments for vet X on date Y"
        builder.HasIndex(va => new { va.VetId, va.AppointmentDateTime, va.StatusTypeId })
            .HasDatabaseName("IX_VetAppointment_Vet_DateTime_Status");
        
        // 6. Covering index for appointment listings
        // Includes frequently queried columns to avoid table lookups
        builder.HasIndex(va => new { va.StatusTypeId, va.AppointmentDateTime })
            .IncludeProperties(va => new { va.VetId, va.PetId, va.Reason })
            .HasDatabaseName("IX_VetAppointment_Status_DateTime_Covering");
        
        // =============================================
        // RELATIONSHIPS
        // =============================================
        
        // Many-to-One: VetAppointment → Pet
        builder.HasOne(va => va.Pet)
            .WithMany(p => p.VetAppointments)
            .HasForeignKey(va => va.PetId)
            .OnDelete(DeleteBehavior.Restrict);  // Don't delete appointments if pet deleted
        
        // Many-to-One: VetAppointment → PetOwner
        builder.HasOne(va => va.PetOwner)
            .WithMany(po => po.VetAppointments)
            .HasForeignKey(va => va.PetOwnerId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Many-to-One: VetAppointment → Vet
        builder.HasOne(va => va.Vet)
            .WithMany(v => v.VetAppointments)
            .HasForeignKey(va => va.VetId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Many-to-One: VetAppointment → VetAppointmentType
        builder.HasOne(va => va.VetAppointmentType)
            .WithMany(vat => vat.VetAppointments)
            .HasForeignKey(va => va.VetAppointmentTypeId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Many-to-One: VetAppointment → StatusType
        builder.HasOne(va => va.StatusType)
            .WithMany(st => st.VetAppointments)
            .HasForeignKey(va => va.StatusTypeId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // One-to-One: VetAppointment ↔ VetPayment (Optional)
        builder.HasOne(va => va.VetPayment)
            .WithOne(vp => vp.VetAppointment)
            .HasForeignKey<VetPayment>(vp => vp.VetAppointmentId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // One-to-One: VetAppointment ↔ VetReview (Optional)
        builder.HasOne(va => va.VetReview)
            .WithOne(vr => vr.VetAppointment)
            .HasForeignKey<VetReview>(vr => vr.VetAppointmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
