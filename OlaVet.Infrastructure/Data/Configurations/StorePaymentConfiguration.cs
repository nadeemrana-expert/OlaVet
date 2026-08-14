// =============================================
// PAYMENT ENTITY TEMPLATE
// Use this for: VetPayment, LabPayment, StorePayment
// =============================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OlaVet.Domain.Entities;

namespace OlaVet.Infrastructure.Data.Configurations;

public class StorePaymentConfiguration : IEntityTypeConfiguration<StorePayment>
{
    public void Configure(EntityTypeBuilder<StorePayment> builder)
    {
        builder.ToTable("StorePayment");
        builder.HasKey(sp => sp.StorePaymentId);
        
        // Properties
        builder.Property(sp => sp.Amount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();
        
        builder.Property(sp => sp.PaymentMethod)
            .HasMaxLength(50);
        
        builder.Property(sp => sp.TransactionId)
            .HasMaxLength(100);
        
        // Unique index on TransactionId
        builder.HasIndex(sp => sp.TransactionId)
            .IsUnique()
            .HasDatabaseName("IX_StorePayment_TransactionId");
        
        // Index on payment date
        builder.HasIndex(sp => sp.PaymentDateTime)
            .HasDatabaseName("IX_StorePayment_PaymentDateTime");
        
        // FK indexes for query performance
        builder.HasIndex(sp => sp.StoreId)
            .HasDatabaseName("IX_StorePayment_StoreId");
        
        builder.HasIndex(sp => sp.PetOwnerId)
            .HasDatabaseName("IX_StorePayment_PetOwnerId");
        
        // Relationships configured in StoreAppointment
    }
}
