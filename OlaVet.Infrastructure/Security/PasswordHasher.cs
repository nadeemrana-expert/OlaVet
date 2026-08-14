// =============================================
// File: OlaVet.Infrastructure/Security/PasswordHasher.cs
// BCrypt password hashing implementation
// =============================================

using OlaVet.Application.Services.Interfaces;

namespace OlaVet.Infrastructure.Security;

/// <summary>
/// Password hashing using BCrypt (work factor 12)
/// BCrypt automatically handles salt generation and storage
/// </summary>
public class PasswordHasher : IPasswordHasher
{
    // Work factor: 12 = ~250ms per hash (good balance of security vs performance)
    private const int WorkFactor = 12;
    
    /// <summary>
    /// Hash a password using BCrypt with automatic salt generation
    /// </summary>
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);
    }
    
    /// <summary>
    /// Verify a password against a BCrypt hash
    /// BCrypt handles salt extraction from the hash automatically
    /// </summary>
    public bool VerifyPassword(string password, string passwordHash)
    {
        try
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
        catch
        {
            // If hash format is invalid, return false (don't throw)
            return false;
        }
    }
}
