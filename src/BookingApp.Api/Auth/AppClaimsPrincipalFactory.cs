using System.Security.Claims;
using BookingApp.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace BookingApp.Api.Auth;

public class AppClaimsPrincipalFactory(
    UserManager<AppUser> userManager,
    RoleManager<IdentityRole<int>> roleManager,
    IOptions<IdentityOptions> options)
    : UserClaimsPrincipalFactory<AppUser, IdentityRole<int>>(userManager, roleManager, options)
{
    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(AppUser user)
    {
        var identity = await base.GenerateClaimsAsync(user);
        
        if (user.StaffId is int staffId)
        {
            identity.AddClaim(new Claim(AppClaims.StaffId, staffId.ToString()));
        }

        return identity;
    }
}