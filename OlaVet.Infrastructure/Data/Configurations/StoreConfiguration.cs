using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OlaVet.Domain.Entities;

namespace OlaVet.Infrastructure.Data.Configurations;

public class StoreConfiguration : IEntityTypeConfiguration<Store>
{
    public void Configure(EntityTypeBuilder<Store> builder)
    {
        builder.ToTable("Store");

        builder.HasKey(s => s.StoreId);

        // PROPERTIES
        builder.Property(s => s.StoreName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(s => s.StoreAddress)
            .HasMaxLength(500);

        builder.Property(s => s.ContactNumber)
            .IsRequired()
            .HasMaxLength(20);

        // INDEXES
        builder.HasIndex(s => s.StoreName)
            .HasDatabaseName("IX_Store_Name");

        builder.HasIndex(s => s.ContactNumber)
            .IsUnique();

        // RELATIONSHIPS
        builder.HasMany(s => s.Inventories)
            .WithOne(i => i.Store)
            .HasForeignKey(i => i.StoreId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.MedicineOrders)
            .WithOne(mo => mo.Store)
            .HasForeignKey(mo => mo.StoreId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
