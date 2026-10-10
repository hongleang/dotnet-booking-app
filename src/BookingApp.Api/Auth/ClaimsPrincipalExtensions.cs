using System.Security.Claims;

namespace BookingApp.Api.Auth;

public static class ClaimsPrincipalExtensions
{
    public static int? GetStaffId(this ClaimsPrincipal user) =>
        int.TryParse(user.FindFirst(AppClaims.StaffId)?.Value, out var id) ? id : null;   
}