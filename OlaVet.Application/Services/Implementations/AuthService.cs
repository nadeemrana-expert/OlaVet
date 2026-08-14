// =============================================
// File: OlaVet.Application/Services/Implementations/AuthService.cs
// Authentication service implementation
// =============================================

using Microsoft.Extensions.Logging;
using OlaVet.Application.Common;
using OlaVet.Application.DTOs.Auth;
using OlaVet.Application.Services.Interfaces;
using OlaVet.Domain.Entities;
using OlaVet.Domain.Interfaces;

namespace OlaVet.Application.Services.Implementations;

/// <summary>
/// Handles authentication: registration, login, token refresh, password changes
/// Uses BCrypt for password hashing and JWT with refresh token rotation
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IUnitOfWork unitOfWork,
        ITokenService tokenService,
        IPasswordHasher passwordHasher,
        ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    /// <summary>
    /// Roles allowed for self-registration.
    /// Admin, LabTechnician, StoreManager must be created by an admin.
    /// </summary>
    private static readonly HashSet<string> AllowedSelfRegistrationRoles = new(StringComparer.OrdinalIgnoreCase)
    {
        "PetOwner",
        "Vet"
    };

    /// <summary>
    /// Register a new user with BCrypt password hashing.
    /// Creates the corresponding domain entity (PetOwner or Vet) and links it.
    /// </summary>
    public async Task<Result<AuthResponseDto>> RegisterAsync(RegisterDto dto, string? ipAddress = null)
    {
        // Validate role is allowed for self-registration
        if (!AllowedSelfRegistrationRoles.Contains(dto.Role))
        {
            _logger.LogWarning("Registration attempt with disallowed role: {Role}", dto.Role);
            return Result<AuthResponseDto>.Failure(
                $"Self-registration is only allowed for PetOwner and Vet roles. " +
                $"'{dto.Role}' accounts must be created by an administrator.");
        }

        // Check if email already exists
        if (await _userRepository.EmailExistsAsync(dto.Email))
        {
            _logger.LogWarning("Registration attempt with existing email: {Email}", MaskEmail(dto.Email));
            return Result<AuthResponseDto>.Failure("An account with this email already exists");
        }

        // Get the role
        var role = await _roleRepository.GetByNameAsync(dto.Role);
        if (role == null)
        {
            return Result<AuthResponseDto>.Failure($"Role '{dto.Role}' not found");
        }

        // Create user with hashed password
        var user = new ApplicationUser
        {
            Email = dto.Email,
            PasswordHash = _passwordHasher.HashPassword(dto.Password),
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            PhoneNumber = dto.PhoneNumber,
            GdprConsentGiven = dto.GdprConsent,
            GdprConsentDate = dto.GdprConsent ? DateTime.UtcNow : null,
            EmailConfirmed = false // Will be confirmed via email (future)
        };

        // Assign role
        user.UserRoles.Add(new UserRole
        {
            Role = role,
            AssignedDate = DateTime.UtcNow
        });

        await _userRepository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        // Create the corresponding domain entity and link it
        if (dto.Role.Equals("PetOwner", StringComparison.OrdinalIgnoreCase))
        {
            var petOwner = new PetOwner
            {
                OwnerName = $"{dto.FirstName} {dto.LastName}".Trim(),
                Email = dto.Email,
                ContactNumber = dto.PhoneNumber ?? string.Empty,
                RegistrationDate = DateTime.UtcNow,
                IsActive = true
            };
            await _unitOfWork.PetOwners.AddAsync(petOwner);
            await _unitOfWork.SaveChangesAsync();
            
            user.PetOwnerId = petOwner.PetOwnerId;
        }
        else if (dto.Role.Equals("Vet", StringComparison.OrdinalIgnoreCase))
        {
            var vet = new Vet
            {
                VetName = $"{dto.FirstName} {dto.LastName}".Trim(),
                Email = dto.Email,
                ContactNumber = dto.PhoneNumber ?? string.Empty,
                Fee = 0, // Will be set later in profile setup
                IsActive = true
            };
            await _unitOfWork.Vets.AddAsync(vet);
            await _unitOfWork.SaveChangesAsync();
            
            user.VetId = vet.VetId;
        }

        // Generate tokens
        var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
        var permissions = user.UserRoles
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission.Name)
            .Distinct()
            .ToList();

        var accessToken = _tokenService.GenerateAccessToken(user, roles, permissions);
        var refreshToken = _tokenService.GenerateRefreshToken();

        // Save refresh token
        user.RefreshTokens.Add(new RefreshToken
        {
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedByIp = ipAddress
        });

        user.LastLoginDate = DateTime.UtcNow;
        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("User registered successfully: {UserId} as {Role}", user.UserId, dto.Role);

        return Result<AuthResponseDto>.Success(new AuthResponseDto
        {
            UserId = user.UserId,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            AccessToken = accessToken,
            AccessTokenExpiry = DateTime.UtcNow.AddHours(24),
            RefreshToken = refreshToken,
            Roles = roles,
            Permissions = permissions
        });
    }

    /// <summary>
    /// Login with email/password, returns JWT + refresh token
    /// Implements account lockout after 5 failed attempts
    /// </summary>
    public async Task<Result<AuthResponseDto>> LoginAsync(LoginDto dto, string? ipAddress = null)
    {
        var user = await _userRepository.GetByEmailAsync(dto.Email);
        
        if (user == null)
        {
            _logger.LogWarning("Login attempt for non-existent email: {Email}", MaskEmail(dto.Email));
            // Generic message to prevent email enumeration
            return Result<AuthResponseDto>.Failure("Invalid email or password");
        }

        // Check lockout
        if (user.IsLockedOut && user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow)
        {
            _logger.LogWarning("Login attempt for locked account: {UserId}", user.UserId);
            return Result<AuthResponseDto>.Failure("Account is locked. Please try again later.");
        }

        // Reset lockout if expired
        if (user.IsLockedOut && user.LockoutEnd.HasValue && user.LockoutEnd <= DateTime.UtcNow)
        {
            user.IsLockedOut = false;
            user.FailedLoginAttempts = 0;
            user.LockoutEnd = null;
        }

        // Verify password
        if (!_passwordHasher.VerifyPassword(dto.Password, user.PasswordHash))
        {
            user.FailedLoginAttempts++;
            
            // Lock account after 5 failed attempts (30 min lockout)
            if (user.FailedLoginAttempts >= 5)
            {
                user.IsLockedOut = true;
                user.LockoutEnd = DateTime.UtcNow.AddMinutes(30);
                _logger.LogWarning("Account locked due to failed attempts: {UserId}", user.UserId);
            }
            
            _userRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();
            
            return Result<AuthResponseDto>.Failure("Invalid email or password");
        }

        // Successful login - reset failed attempts
        user.FailedLoginAttempts = 0;
        user.IsLockedOut = false;
        user.LockoutEnd = null;
        user.LastLoginDate = DateTime.UtcNow;

        // Generate tokens
        var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
        var permissions = user.UserRoles
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission.Name)
            .Distinct()
            .ToList();

        var accessToken = _tokenService.GenerateAccessToken(user, roles, permissions);
        var refreshToken = _tokenService.GenerateRefreshToken();

        // Revoke all old refresh tokens for security
        foreach (var oldToken in user.RefreshTokens.Where(t => t.IsActive))
        {
            oldToken.RevokedAt = DateTime.UtcNow;
            oldToken.RevokedByIp = ipAddress;
            oldToken.RevokeReason = "Replaced by new login";
        }

        // Save new refresh token
        user.RefreshTokens.Add(new RefreshToken
        {
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedByIp = ipAddress
        });

        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("User logged in: {UserId}", user.UserId);

        return Result<AuthResponseDto>.Success(new AuthResponseDto
        {
            UserId = user.UserId,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            AccessToken = accessToken,
            AccessTokenExpiry = DateTime.UtcNow.AddHours(24),
            RefreshToken = refreshToken,
            Roles = roles,
            Permissions = permissions
        });
    }

    /// <summary>
    /// Refresh token rotation: old refresh token is revoked, new one issued
    /// This prevents refresh token reuse attacks
    /// </summary>
    public async Task<Result<AuthResponseDto>> RefreshTokenAsync(RefreshTokenDto dto, string? ipAddress = null)
    {
        // Validate the expired access token to get user ID
        var userId = _tokenService.GetUserIdFromExpiredToken(dto.AccessToken);
        if (userId == null)
        {
            return Result<AuthResponseDto>.Failure("Invalid access token");
        }

        var user = await _userRepository.GetByRefreshTokenAsync(dto.RefreshToken);
        if (user == null || user.UserId != userId.Value)
        {
            return Result<AuthResponseDto>.Failure("Invalid refresh token");
        }

        var existingToken = user.RefreshTokens.FirstOrDefault(t => t.Token == dto.RefreshToken);
        if (existingToken == null)
        {
            return Result<AuthResponseDto>.Failure("Refresh token not found");
        }

        // If token is revoked, it might be a reuse attack - revoke all tokens
        if (existingToken.IsRevoked)
        {
            _logger.LogWarning("Attempted reuse of revoked refresh token for user: {UserId}", user.UserId);
            
            foreach (var token in user.RefreshTokens.Where(t => t.IsActive))
            {
                token.RevokedAt = DateTime.UtcNow;
                token.RevokedByIp = ipAddress;
                token.RevokeReason = "Suspected token reuse attack";
            }
            
            _userRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();
            
            return Result<AuthResponseDto>.Failure("Token has been revoked. All sessions terminated for security.");
        }

        if (existingToken.IsExpired)
        {
            return Result<AuthResponseDto>.Failure("Refresh token has expired. Please login again.");
        }

        // Rotate: revoke old, create new
        var newRefreshToken = _tokenService.GenerateRefreshToken();
        
        existingToken.RevokedAt = DateTime.UtcNow;
        existingToken.RevokedByIp = ipAddress;
        existingToken.ReplacedByToken = newRefreshToken;
        existingToken.RevokeReason = "Rotated";

        var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
        var permissions = user.UserRoles
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission.Name)
            .Distinct()
            .ToList();

        var newAccessToken = _tokenService.GenerateAccessToken(user, roles, permissions);

        user.RefreshTokens.Add(new RefreshToken
        {
            Token = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedByIp = ipAddress
        });

        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return Result<AuthResponseDto>.Success(new AuthResponseDto
        {
            UserId = user.UserId,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            AccessToken = newAccessToken,
            AccessTokenExpiry = DateTime.UtcNow.AddHours(24),
            RefreshToken = newRefreshToken,
            Roles = roles,
            Permissions = permissions
        });
    }

    /// <summary>
    /// Revoke a refresh token (logout)
    /// </summary>
    public async Task<Result<bool>> RevokeTokenAsync(string refreshToken, string? ipAddress = null)
    {
        var user = await _userRepository.GetByRefreshTokenAsync(refreshToken);
        if (user == null)
        {
            return Result<bool>.Failure("Invalid refresh token");
        }

        var token = user.RefreshTokens.FirstOrDefault(t => t.Token == refreshToken);
        if (token == null || !token.IsActive)
        {
            return Result<bool>.Failure("Token is already revoked or expired");
        }

        token.RevokedAt = DateTime.UtcNow;
        token.RevokedByIp = ipAddress;
        token.RevokeReason = "User logout";

        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Token revoked for user: {UserId}", user.UserId);
        return Result<bool>.Success(true);
    }

    /// <summary>
    /// Change password for authenticated user
    /// </summary>
    public async Task<Result<bool>> ChangePasswordAsync(int userId, ChangePasswordDto dto)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return Result<bool>.Failure("User not found");
        }

        if (!_passwordHasher.VerifyPassword(dto.CurrentPassword, user.PasswordHash))
        {
            return Result<bool>.Failure("Current password is incorrect");
        }

        user.PasswordHash = _passwordHasher.HashPassword(dto.NewPassword);
        user.ModifiedDate = DateTime.UtcNow;

        // Revoke all refresh tokens (force re-login on all devices)
        foreach (var token in user.RefreshTokens.Where(t => t.IsActive))
        {
            token.RevokedAt = DateTime.UtcNow;
            token.RevokeReason = "Password changed";
        }

        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Password changed for user: {UserId}", userId);
        return Result<bool>.Success(true);
    }

    /// <summary>
    /// Get current authenticated user info
    /// </summary>
    public async Task<Result<AuthResponseDto>> GetCurrentUserAsync(int userId)
    {
        var user = await _userRepository.GetByIdWithRolesAsync(userId);
        if (user == null)
        {
            return Result<AuthResponseDto>.Failure("User not found");
        }

        var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
        var permissions = user.UserRoles
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission.Name)
            .Distinct()
            .ToList();

        return Result<AuthResponseDto>.Success(new AuthResponseDto
        {
            UserId = user.UserId,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Roles = roles,
            Permissions = permissions
        });
    }

    // =============================================
    // PRIVATE HELPERS
    // =============================================

    /// <summary>
    /// Admin-only: Create a staff account (LabTechnician or StoreManager)
    /// and link it to the corresponding domain entity
    /// </summary>
    public async Task<Result<AuthResponseDto>> CreateStaffAccountAsync(CreateStaffAccountDto dto)
    {
        // Only allow staff roles
        var allowedStaffRoles = new HashSet<string>(StringComparer.OrdinalIgnoreCase) 
        { 
            "LabTechnician", "StoreManager" 
        };
        
        if (!allowedStaffRoles.Contains(dto.Role))
        {
            return Result<AuthResponseDto>.Failure(
                "This endpoint is for creating LabTechnician or StoreManager accounts only.");
        }

        if (dto.EntityId == null || dto.EntityId <= 0)
        {
            return Result<AuthResponseDto>.Failure(
                "EntityId is required (LabId for LabTechnician, StoreId for StoreManager).");
        }

        // Validate entity exists
        if (dto.Role.Equals("LabTechnician", StringComparison.OrdinalIgnoreCase))
        {
            var lab = await _unitOfWork.Labs.GetByIdAsync(dto.EntityId.Value);
            if (lab == null)
                return Result<AuthResponseDto>.Failure($"Lab with ID {dto.EntityId} not found.");
        }
        else if (dto.Role.Equals("StoreManager", StringComparison.OrdinalIgnoreCase))
        {
            var store = await _unitOfWork.Stores.GetByIdAsync(dto.EntityId.Value);
            if (store == null)
                return Result<AuthResponseDto>.Failure($"Store with ID {dto.EntityId} not found.");
        }

        // Check if email already exists
        if (await _userRepository.EmailExistsAsync(dto.Email))
        {
            return Result<AuthResponseDto>.Failure("An account with this email already exists.");
        }

        // Get the role
        var role = await _roleRepository.GetByNameAsync(dto.Role);
        if (role == null)
        {
            return Result<AuthResponseDto>.Failure($"Role '{dto.Role}' not found.");
        }

        // Create user
        var user = new ApplicationUser
        {
            Email = dto.Email,
            PasswordHash = _passwordHasher.HashPassword(dto.Password),
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            PhoneNumber = dto.PhoneNumber,
            EmailConfirmed = true, // Admin-created accounts are pre-confirmed
            GdprConsentGiven = true,
            GdprConsentDate = DateTime.UtcNow
        };

        user.UserRoles.Add(new UserRole
        {
            Role = role,
            AssignedDate = DateTime.UtcNow
        });

        await _userRepository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Staff account created: {UserId} as {Role} linked to entity {EntityId}",
            user.UserId, dto.Role, dto.EntityId);

        var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
        var permissions = user.UserRoles
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission.Name)
            .Distinct()
            .ToList();

        return Result<AuthResponseDto>.Success(new AuthResponseDto
        {
            UserId = user.UserId,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Roles = roles,
            Permissions = permissions
        });
    }
    
    /// <summary>
    /// Mask email for logging (GDPR/data protection)
    /// "john@example.com" → "j***@e***.com"
    /// </summary>
    private static string MaskEmail(string email)
    {
        var parts = email.Split('@');
        if (parts.Length != 2) return "***";
        
        var localPart = parts[0].Length > 1 
            ? parts[0][0] + new string('*', parts[0].Length - 1) 
            : "*";
        
        var domainParts = parts[1].Split('.');
        var domainName = domainParts[0].Length > 1 
            ? domainParts[0][0] + new string('*', domainParts[0].Length - 1) 
            : "*";
        
        return $"{localPart}@{domainName}.{string.Join('.', domainParts.Skip(1))}";
    }
}
