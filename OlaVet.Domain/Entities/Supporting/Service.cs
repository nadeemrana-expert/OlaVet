// =============================================
// File: OlaVet.Domain/Entities/Service.cs
// Services offered by a vet
// =============================================

namespace OlaVet.Domain.Entities;

/// <summary>
/// Services offered by a veterinarian
/// Example: "Dental Cleaning - 5000 PKR"
/// </summary>
public class Service
{
    public int ServiceId { get; set; }
    
    // Foreign Key
    public int VetId { get; set; }
    
    public string ServiceType { get; set; } = string.Empty;
    public string? ServiceDescription { get; set; }
    public decimal? ServiceFee { get; set; }
    
    // Navigation property
    public virtual Vet Vet { get; set; } = null!;
}
