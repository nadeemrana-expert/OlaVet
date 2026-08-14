// =============================================
// File: OlaVet.API/Security/ResourceOwnerAttribute.cs
// Resource-level authorization - ensures users can only
// access their own resources
// =============================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace OlaVet.API.Security;

/// <summary>
/// Authorization filter that ensures users can only access their own resources.
/// Checks that the resource ID in the route matches the authenticated user's linked entity.
/// 
/// Usage: [ResourceOwner("petOwnerId")] on controller actions
/// This will check that the route parameter matches the user's petOwnerId claim.
/// Admins bypass this check.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class ResourceOwnerAttribute : Attribute, IAuthorizationFilter
{
    private readonly string _claimType;
    private readonly string _routeParamName;

    /// <param name="claimType">The claim type to check (e.g., "petOwnerId", "vetId")</param>
    /// <param name="routeParamName">The route parameter name (defaults to "id")</param>
    public ResourceOwnerAttribute(string claimType, string routeParamName = "id")
    {
        _claimType = claimType;
        _routeParamName = routeParamName;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;
        
        if (!user.Identity?.IsAuthenticated ?? true)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        // Admins bypass resource ownership checks
        if (user.IsInRole("Admin"))
        {
            return;
        }

        // Get the resource ID from the route
        if (!context.RouteData.Values.TryGetValue(_routeParamName, out var routeValue))
        {
            return; // No route parameter to check
        }

        // Get the user's claim
        var claimValue = user.Claims.FirstOrDefault(c => c.Type == _claimType)?.Value;
        
        if (claimValue == null || claimValue != routeValue?.ToString())
        {
            context.Result = new ForbidResult();
        }
    }
}
