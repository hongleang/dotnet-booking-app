using System.Security.Claims;
using BookingApp.Api.Models;
using Microsoft.AspNetCore.Identity;

namespace BookingApp.Api.Endpoints;

public record LoginRequest(string Email, string Password);
public record CurrentUserDto(int Id, string Email, int? StaffId, IList<string> Roles);

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/auth");

        group.MapPost("/login", async (LoginRequest request, SignInManager<AppUser> signIn) =>
        {
            var result = await signIn.PasswordSignInAsync(
                request.Email, request.Password, isPersistent: false, lockoutOnFailure: true);

            return result.Succeeded ? Results.NoContent() : Results.Unauthorized();
        });

        group.MapPost("/logout", async (SignInManager<AppUser> signIn) =>
        {
            await signIn.SignOutAsync();
            return Results.NoContent();
        }).RequireAuthorization();

        group.MapGet("/me", async (ClaimsPrincipal principal, UserManager<AppUser> userManager) =>
        {
            var user = await userManager.GetUserAsync(principal);
            if (user is null) return Results.Unauthorized();
            
            var roles = await userManager.GetRolesAsync(user);
            return Results.Ok(new CurrentUserDto(user.Id, user.Email!, user.StaffId, roles));
        }).RequireAuthorization();
    }
}