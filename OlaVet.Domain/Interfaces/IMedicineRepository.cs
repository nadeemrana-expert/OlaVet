// =============================================
// File: OlaVet.Domain/Interfaces/IMedicineRepository.cs
// Specific repository for Medicine entity
// =============================================

using OlaVet.Domain.Entities;

namespace OlaVet.Domain.Interfaces;

/// <summary>
/// Medicine-specific repository methods
/// </summary>
public interface IMedicineRepository : IRepository<Medicine>
{
    /// <summary>
    /// Get medicine with type and inventory
    /// </summary>
    Task<Medicine?> GetWithDetailsAsync(int medicineId);
    
    /// <summary>
    /// Search medicines by name or manufacturer
    /// </summary>
    Task<IEnumerable<Medicine>> SearchAsync(string searchTerm);
    
    /// <summary>
    /// Get medicines by type
    /// </summary>
    Task<IEnumerable<Medicine>> GetByTypeAsync(int medicineTypeId);
    
    /// <summary>
    /// Get medicines in stock at a specific store
    /// </summary>
    Task<IEnumerable<Medicine>> GetInStockAtStoreAsync(int storeId);
    
    /// <summary>
    /// Get prescription-only medicines
    /// </summary>
    Task<IEnumerable<Medicine>> GetPrescriptionMedicinesAsync();
    
    /// <summary>
    /// Get over-the-counter medicines
    /// </summary>
    Task<IEnumerable<Medicine>> GetOtcMedicinesAsync();
    
    /// <summary>
    /// Get low stock medicines (below threshold)
    /// </summary>
    Task<IEnumerable<MedicineWithStock>> GetLowStockAsync(int threshold = 10);
}

/// <summary>
/// DTO for Medicine with stock information
/// </summary>
public class MedicineWithStock
{
    public int MedicineId { get; set; }
    public string MedicineName { get; set; } = string.Empty;
    public int? MG { get; set; }
    public decimal Price { get; set; }
    public string? Manufacturer { get; set; }
    public bool RequiresPrescription { get; set; }
    public int TotalStock { get; set; }
    public int StoreCount { get; set; }
}
