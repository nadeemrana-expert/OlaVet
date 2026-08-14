// =============================================
// File: OlaVet.Domain/Entities/PetOwner.cs
// Pet owner (customer) entity
// =============================================

using OlaVet.Domain.Common;

namespace OlaVet.Domain.Entities;

/// <summary>
/// Pet owner - the customer who uses OlaVet services
/// </summary>
public class PetOwner : BaseEntity
{
    // Primary Key
    public int PetOwnerId { get; set; }
    
    // Basic Information
    public string OwnerName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string ContactNumber { get; set; } = string.Empty;
    public string? HomeAddress { get; set; }
    public int? Age { get; set; }
    public string? Gender { get; set; }
    
    // Wallet for storing balance
    public decimal Wallet { get; set; } = 0;
    
    public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
    
    // =============================================
    // NAVIGATION PROPERTIES (Relationships)
    // =============================================
    // These allow EF Core to automatically load related data
    
    /// <summary>
    /// All pets owned by this owner
    /// EF Core convention: ICollection for one-to-many
    /// </summary>
    public virtual ICollection<Pet> Pets { get; set; } = new List<Pet>();
    
    /// <summary>
    /// All vet appointments booked by this owner
    /// </summary>
    public virtual ICollection<VetAppointment> VetAppointments { get; set; } = new List<VetAppointment>();
    
    /// <summary>
    /// All lab appointments booked
    /// </summary>
    public virtual ICollection<LabAppointment> LabAppointments { get; set; } = new List<LabAppointment>();
    
    /// <summary>
    /// All medicine orders placed
    /// </summary>
    public virtual ICollection<MedicineOrder> MedicineOrders { get; set; } = new List<MedicineOrder>();
    
    /// <summary>
    /// Medical records for their pets
    /// </summary>
    public virtual ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
    
    /// <summary>
    /// Reviews left for vets
    /// </summary>
    public virtual ICollection<VetReview> VetReviews { get; set; } = new List<VetReview>();
    
    /// <summary>
    /// Reviews left for labs
    /// </summary>
    public virtual ICollection<LabReview> LabReviews { get; set; } = new List<LabReview>();
    
    /// <summary>
    /// Reviews left for stores
    /// </summary>
    public virtual ICollection<StoreReview> StoreReviews { get; set; } = new List<StoreReview>();
    
    /// <summary>
    /// Payment history for vet services
    /// </summary>
    public virtual ICollection<VetPayment> VetPayments { get; set; } = new List<VetPayment>();
    
    /// <summary>
    /// Payment history for lab services
    /// </summary>
    public virtual ICollection<LabPayment> LabPayments { get; set; } = new List<LabPayment>();
    
    /// <summary>
    /// Payment history for store purchases
    /// </summary>
    public virtual ICollection<StorePayment> StorePayments { get; set; } = new List<StorePayment>();
}
