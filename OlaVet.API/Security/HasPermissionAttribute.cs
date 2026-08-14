// =============================================
// File: OlaVet.API/Security/HasPermissionAttribute.cs
// Custom authorization attribute for permission-level checks
// =============================================

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace OlaVet.API.Security;

/// <summary>
/// Authorization attribute that checks for specific permissions.
/// Usage: [HasPermission("pets.read")] or [HasPermission("pets.read", "pets.create")]
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class HasPermissionAttribute : AuthorizeAttribute, IAuthorizationFilter
{
    private readonly string[] _permissions;

    public HasPermissionAttribute(params string[] permissions)
    {
        _permissions = permissions;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;
        
        if (!user.Identity?.IsAuthenticated ?? true)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        // Check if user has ANY of the required permissions (OR logic)
        var userPermissions = user.Claims
            .Where(c => c.Type == "permission")
            .Select(c => c.Value)
            .ToHashSet();

        // Admin with full access bypasses permission checks
        if (userPermissions.Contains("admin.full"))
        {
            return;
        }

        if (!_permissions.Any(p => userPermissions.Contains(p)))
        {
            context.Result = new ForbidResult();
        }
    }
}
