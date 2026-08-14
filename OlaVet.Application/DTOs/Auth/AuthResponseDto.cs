// =============================================
// File: OlaVet.Application/DTOs/Auth/AuthResponseDto.cs
// DTO for authentication response
// =============================================

namespace OlaVet.Application.DTOs.Auth;

public class AuthResponseDto
{
    public int UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
    public DateTime AccessTokenExpiry { get; set; }
    public string RefreshToken { get; set; } = string.Empty;
    public IEnumerable<string> Roles { get; set; } = [];
    public IEnumerable<string> Permissions { get; set; } = [];
}
