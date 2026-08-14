// =============================================
// File: OlaVet.Application/DTOs/Auth/RegisterDto.cs
// DTO for user registration
// =============================================

namespace OlaVet.Application.DTOs.Auth;

public class RegisterDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    
    /// <summary>
    /// Role to register as: "PetOwner" or "Vet"
    /// </summary>
    public string Role { get; set; } = "PetOwner";
    
    /// <summary>
    /// GDPR consent flag (required for EU users)
    /// </summary>
    public bool GdprConsent { get; set; } = false;
}
