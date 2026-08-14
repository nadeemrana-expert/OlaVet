// =============================================
// File: OlaVet.Application/DTOs/Auth/CreateStaffAccountDto.cs
// DTO for admin-only staff account creation
// =============================================

namespace OlaVet.Application.DTOs.Auth;

/// <summary>
/// Admin-only DTO for creating LabTechnician or StoreManager accounts.
/// These roles cannot self-register — they must be created by an admin
/// and linked to their domain entity (Lab or Store).
/// </summary>
public class CreateStaffAccountDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    
    /// <summary>
    /// Role: "LabTechnician" or "StoreManager"
    /// </summary>
    public string Role { get; set; } = string.Empty;
    
    /// <summary>
    /// The ID of the Lab or Store entity to link this user to.
    /// Required for LabTechnician (LabId) and StoreManager (StoreId).
    /// </summary>
    public int? EntityId { get; set; }
}
