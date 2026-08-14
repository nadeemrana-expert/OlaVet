// =============================================
// File: OlaVet.Infrastructure/Data/Configurations/MedicineOrderConfiguration.cs
// Configuration for MedicineOrder (Order Header)
// =============================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OlaVet.Domain.Entities;

namespace OlaVet.Infrastructure.Data.Configurations;

public class MedicineOrderConfiguration : IEntityTypeConfiguration<MedicineOrder>
{
    public void Configure(EntityTypeBuilder<MedicineOrder> builder)
    {
        builder.ToTable("MedicineOrder");
        
        builder.HasKey(mo => mo.MedicineOrderId);
        
        // =============================================
        // PROPERTIES
        // =============================================
        
        builder.Property(mo => mo.TotalAmount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();
        
        builder.Property(mo => mo.DeliveryAddress)
            .HasMaxLength(300);
        
        builder.Property(mo => mo.OrderDateTime)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");  // Database-level default
        
        // =============================================
        // INDEXES
        // =============================================
        
        // Index on PetOwnerId - "My orders"
        builder.HasIndex(mo => mo.PetOwnerId)
            .HasDatabaseName("IX_MedicineOrder_PetOwnerId");
        
        // Index on OrderDateTime - "Recent orders"
        builder.HasIndex(mo => mo.OrderDateTime)
            .HasDatabaseName("IX_MedicineOrder_OrderDateTime");
        
        // Index on StatusTypeId - "Pending orders", "Delivered orders"
        builder.HasIndex(mo => mo.StatusTypeId)
            .HasDatabaseName("IX_MedicineOrder_StatusTypeId");
        
        // Composite index for store's orders by status
        builder.HasIndex(mo => new { mo.StoreId, mo.StatusTypeId, mo.OrderDateTime })
            .HasDatabaseName("IX_MedicineOrder_Store_Status_DateTime");
        
        // =============================================
        // RELATIONSHIPS
        // =============================================
        
        builder.HasOne(mo => mo.PetOwner)
            .WithMany(po => po.MedicineOrders)
            .HasForeignKey(mo => mo.PetOwnerId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(mo => mo.Store)
            .WithMany(s => s.MedicineOrders)
            .HasForeignKey(mo => mo.StoreId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(mo => mo.StatusType)
            .WithMany(st => st.MedicineOrders)
            .HasForeignKey(mo => mo.StatusTypeId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // One-to-Many: MedicineOrder → MedicineOrderDetails
        builder.HasMany(mo => mo.MedicineOrderDetails)
            .WithOne(mod => mod.MedicineOrder)
            .HasForeignKey(mod => mod.MedicineOrderId)
            .OnDelete(DeleteBehavior.Cascade);  // Delete line items when order deleted
    }
}
