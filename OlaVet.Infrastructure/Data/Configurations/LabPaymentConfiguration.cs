// =============================================
// PAYMENT ENTITY TEMPLATE
// Use this for: VetPayment, LabPayment, StorePayment
// =============================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OlaVet.Domain.Entities;

namespace OlaVet.Infrastructure.Data.Configurations;

public class LabPaymentConfiguration : IEntityTypeConfiguration<LabPayment>
{
    public void Configure(EntityTypeBuilder<LabPayment> builder)
    {
        builder.ToTable("LabPayment");
        builder.HasKey(lp => lp.LabPaymentId);
        
        // Properties
        builder.Property(lp => lp.Amount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();
        
        builder.Property(lp => lp.PaymentMethod)
            .HasMaxLength(50);
        
        builder.Property(lp => lp.TransactionId)
            .HasMaxLength(100);
        
        // Unique index on TransactionId
        builder.HasIndex(lp => lp.TransactionId)
            .IsUnique()
            .HasDatabaseName("IX_LabPayment_TransactionId");
        
        // Index on payment date
        builder.HasIndex(lp => lp.PaymentDateTime)
            .HasDatabaseName("IX_LabPayment_PaymentDateTime");
        
        // FK indexes for query performance
        builder.HasIndex(lp => lp.LabId)
            .HasDatabaseName("IX_LabPayment_LabId");
        
        builder.HasIndex(lp => lp.PetOwnerId)
            .HasDatabaseName("IX_LabPayment_PetOwnerId");
        
        // Relationships configured in LabAppointment
    }
}
