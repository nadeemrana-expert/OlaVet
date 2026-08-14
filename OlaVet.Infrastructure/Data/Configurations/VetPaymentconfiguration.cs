// =============================================
// PAYMENT ENTITY TEMPLATE
// Use this for: VetPayment, LabPayment, StorePayment
// =============================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OlaVet.Domain.Entities;

namespace OlaVet.Infrastructure.Data.Configurations;

public class VetPaymentConfiguration : IEntityTypeConfiguration<VetPayment>
{
    public void Configure(EntityTypeBuilder<VetPayment> builder)
    {
        builder.ToTable("VetPayment");
        builder.HasKey(vp => vp.VetPaymentId);
        
        // Properties
        builder.Property(vp => vp.Amount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();
        
        builder.Property(vp => vp.PaymentMethod)
            .HasMaxLength(50);
        
        builder.Property(vp => vp.TransactionId)
            .HasMaxLength(100);
        
        // Unique index on TransactionId
        builder.HasIndex(vp => vp.TransactionId)
            .IsUnique()
            .HasDatabaseName("IX_VetPayment_TransactionId");
        
        // Index on payment date
        builder.HasIndex(vp => vp.PaymentDateTime)
            .HasDatabaseName("IX_VetPayment_PaymentDateTime");
        
        // FK indexes for query performance
        builder.HasIndex(vp => vp.VetId)
            .HasDatabaseName("IX_VetPayment_VetId");
        
        builder.HasIndex(vp => vp.PetOwnerId)
            .HasDatabaseName("IX_VetPayment_PetOwnerId");
        
        // Relationships configured in VetAppointment
    }
}
