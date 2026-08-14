// =============================================
// File: OlaVet.API/Extensions/ClaimsPrincipalExtensions.cs
// Helper methods to extract role & entity info from JWT claims
// =============================================

using System.Security.Claims;

namespace OlaVet.API.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static int GetUserId(this ClaimsPrincipal user)
        => int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    public static int? GetPetOwnerId(this ClaimsPrincipal user)
    {
        var val = user.FindFirstValue("petOwnerId");
        return val != null && int.TryParse(val, out var id) ? id : null;
    }

    public static int? GetVetId(this ClaimsPrincipal user)
    {
        var val = user.FindFirstValue("vetId");
        return val != null && int.TryParse(val, out var id) ? id : null;
    }

    public static bool IsAdmin(this ClaimsPrincipal user)
        => user.IsInRole("Admin");

    public static bool IsVet(this ClaimsPrincipal user)
        => user.IsInRole("Vet");

    public static bool IsPetOwner(this ClaimsPrincipal user)
        => user.IsInRole("PetOwner");

    public static bool IsLabTechnician(this ClaimsPrincipal user)
        => user.IsInRole("LabTechnician");

    public static bool IsStoreManager(this ClaimsPrincipal user)
        => user.IsInRole("StoreManager");

    public static string GetPrimaryRole(this ClaimsPrincipal user)
    {
        if (user.IsAdmin()) return "Admin";
        if (user.IsVet()) return "Vet";
        if (user.IsPetOwner()) return "PetOwner";
        if (user.IsLabTechnician()) return "LabTechnician";
        if (user.IsStoreManager()) return "StoreManager";
        return "Unknown";
    }
}
