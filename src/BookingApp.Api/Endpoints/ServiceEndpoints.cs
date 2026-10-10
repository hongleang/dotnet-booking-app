using BookingApp.Api.Auth;
using BookingApp.Api.Data;
using BookingApp.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingApp.Api.Endpoints;

public static class ServiceEndpoints
{
    public record ServiceDto(int Id, string Name, int DurationInMinutes, decimal Price);
    
    public record CreateServiceRequest(string Name, int DurationInMinutes, decimal Price);
    
    public static void MapServiceEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/services");

        group.MapGet("/", async (AppDbContext db) =>
            await db.Services
                .Where(service => service.IsActive)
                .OrderBy(service => service.Name)
                .Select(service => new ServiceDto(service.Id, service.Name, service.DurationMinutes, service.Price))
                .ToListAsync());

        group.MapGet("/{id}", async (AppDbContext db, int id) =>
            await db.Services
                .Where(service => service.IsActive)
                .FirstOrDefaultAsync(service => service.Id == id)
        );

        group.MapPost("/", async (AppDbContext db, CreateServiceRequest request) =>
        {
            var service = new Service
            {
                Name = request.Name,
                DurationMinutes = request.DurationInMinutes,
                Price = request.Price
            };
            
            db.Services.Add(service);
            await db.SaveChangesAsync();
            
            var dto = new ServiceDto(service.Id, service.Name, service.DurationMinutes, Math.Round(service.Price, 2));
            return Results.Created($"/services/{service.Id}", dto);
        }).RequireAuthorization(Policies.AdminOnly);
    }
}