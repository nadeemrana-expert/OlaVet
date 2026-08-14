// =============================================
// File: OlaVet.Infrastructure/Data/Configurations/MedicineOrderDetailConfiguration.cs
// Configuration for MedicineOrderDetail (Order Line Items)
// =============================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OlaVet.Domain.Entities;

namespace OlaVet.Infrastructure.Data.Configurations;

public class MedicineOrderDetailConfiguration : IEntityTypeConfiguration<MedicineOrderDetail>
{
    public void Configure(EntityTypeBuilder<MedicineOrderDetail> builder)
    {
        builder.ToTable("MedicineOrderDetails");
        
        builder.HasKey(mod => mod.OrderDetailId);
        
        // =============================================
        // PROPERTIES
        // =============================================
        
        builder.Property(mod => mod.Quantity)
            .IsRequired();
        
        builder.Property(mod => mod.UnitPrice)
            .HasColumnType("decimal(18,2)")
            .IsRequired();
        
        // =============================================
        // COMPUTED COLUMN - Database calculates this
        // =============================================
        
        builder.Property(mod => mod.Subtotal)
            .HasColumnType("decimal(18,2)")
            .HasComputedColumnSql("[Quantity] * [UnitPrice]", stored: true);
        // stored: true = computed once and stored (faster reads, slower writes)
        // stored: false = computed on every read (slower reads, faster writes)
        
        // =============================================
        // RELATIONSHIPS
        // =============================================
        
        builder.HasOne(mod => mod.MedicineOrder)
            .WithMany(mo => mo.MedicineOrderDetails)
            .HasForeignKey(mod => mod.MedicineOrderId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(mod => mod.Medicine)
            .WithMany(m => m.MedicineOrderDetails)
            .HasForeignKey(mod => mod.MedicineId)
            .OnDelete(DeleteBehavior.Restrict);  // Don't delete medicine if it's in orders
        
        // =============================================
        // INDEXES
        // =============================================
        
        // Index for finding all items in an order
        builder.HasIndex(mod => mod.MedicineOrderId)
            .HasDatabaseName("IX_MedicineOrderDetail_MedicineOrderId");
        
        // Index for finding all orders containing a medicine
        builder.HasIndex(mod => mod.MedicineId)
            .HasDatabaseName("IX_MedicineOrderDetail_MedicineId");
    }
}
