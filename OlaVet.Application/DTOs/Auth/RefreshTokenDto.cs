// =============================================
// File: OlaVet.Application/DTOs/Auth/RefreshTokenDto.cs
// DTO for refresh token request
// =============================================

namespace OlaVet.Application.DTOs.Auth;

public class RefreshTokenDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}
