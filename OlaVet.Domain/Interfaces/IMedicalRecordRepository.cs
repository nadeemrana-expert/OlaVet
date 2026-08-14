// =============================================
// File: OlaVet.Domain/Interfaces/IMedicalRecordRepository.cs
// Specific repository for MedicalRecord entity
// =============================================

using OlaVet.Domain.Entities;

namespace OlaVet.Domain.Interfaces;

/// <summary>
/// Medical record-specific repository methods
/// </summary>
public interface IMedicalRecordRepository : IRepository<MedicalRecord>
{
    /// <summary>
    /// Get medical record with all related data
    /// </summary>
    Task<MedicalRecord?> GetWithDetailsAsync(int recordId);
    
    /// <summary>
    /// Get all records for a pet
    /// </summary>
    Task<IEnumerable<MedicalRecord>> GetByPetIdAsync(int petId);
    
    /// <summary>
    /// Get all records for a pet owner
    /// </summary>
    Task<IEnumerable<MedicalRecord>> GetByOwnerIdAsync(int ownerId);
    
    /// <summary>
    /// Get records by type
    /// </summary>
    Task<IEnumerable<MedicalRecord>> GetByTypeAsync(int recordTypeId);
    
    /// <summary>
    /// Get records created by a specific vet
    /// </summary>
    Task<IEnumerable<MedicalRecord>> GetByVetIdAsync(int vetId);
    
    /// <summary>
    /// Get records within date range
    /// </summary>
    Task<IEnumerable<MedicalRecord>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    
    /// <summary>
    /// Search records by diagnosis
    /// </summary>
    Task<IEnumerable<MedicalRecord>> SearchByDiagnosisAsync(string searchTerm);
}
