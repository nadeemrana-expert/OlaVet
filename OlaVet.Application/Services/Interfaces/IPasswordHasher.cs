// =============================================
// File: OlaVet.Application/Services/Interfaces/IPasswordHasher.cs
// Password hashing service interface
// =============================================

namespace OlaVet.Application.Services.Interfaces;

/// <summary>
/// Service for password hashing and verification using BCrypt
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Hash a password using BCrypt
    /// </summary>
    string HashPassword(string password);
    
    /// <summary>
    /// Verify a password against a BCrypt hash
    /// </summary>
    bool VerifyPassword(string password, string passwordHash);
}
