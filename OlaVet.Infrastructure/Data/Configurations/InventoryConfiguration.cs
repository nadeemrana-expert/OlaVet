// =============================================
// File: OlaVet.Infrastructure/Data/Configurations/InventoryConfiguration.cs
// Configuration for Inventory (Junction table with extra data)
// =============================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OlaVet.Domain.Entities;

namespace OlaVet.Infrastructure.Data.Configurations;

public class InventoryConfiguration : IEntityTypeConfiguration<Inventory>
{
    public void Configure(EntityTypeBuilder<Inventory> builder)
    {
        builder.ToTable("Inventory");
        
        builder.HasKey(i => i.InventoryId);
        
        // =============================================
        // PROPERTIES
        // =============================================
        
        builder.Property(i => i.Quantity)
            .IsRequired()
            .HasDefaultValue(0);
        
        // =============================================
        // UNIQUE CONSTRAINT
        // =============================================
        // A store can only have ONE inventory record per medicine
        builder.HasIndex(i => new { i.StoreId, i.MedicineId })
            .IsUnique()
            .HasDatabaseName("IX_Inventory_Store_Medicine_Unique");
        
        // =============================================
        // RELATIONSHIPS
        // =============================================
        
        builder.HasOne(i => i.Store)
            .WithMany(s => s.Inventories)
            .HasForeignKey(i => i.StoreId)
            .OnDelete(DeleteBehavior.Cascade);  // Delete inventory when store deleted
        
        builder.HasOne(i => i.Medicine)
            .WithMany(m => m.Inventories)
            .HasForeignKey(i => i.MedicineId)
            .OnDelete(DeleteBehavior.Restrict);  // Don't delete medicine if it's in inventory
        
        // =============================================
        // ADDITIONAL INDEXES
        // =============================================
        
        // Find all medicines in a store
        builder.HasIndex(i => i.StoreId)
            .HasDatabaseName("IX_Inventory_StoreId");
        
        // Find which stores stock a medicine
        builder.HasIndex(i => i.MedicineId)
            .HasDatabaseName("IX_Inventory_MedicineId");
        
        // Find low stock items
        builder.HasIndex(i => i.Quantity)
            .HasDatabaseName("IX_Inventory_Quantity");
    }
}
