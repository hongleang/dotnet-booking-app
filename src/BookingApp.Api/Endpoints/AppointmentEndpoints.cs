using System.Security.Claims;
using BookingApp.Api.Auth;
using BookingApp.Api.Data;
using BookingApp.Api.Models;
using BookingApp.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace BookingApp.Api.Endpoints;

public record AppointmentDto(
    int Id, string ClientName, string ServiceName, string StaffName,
    DateTimeOffset StartUtc, DateTimeOffset EndUtc, AppointmentStatus Status)
{
    public static AppointmentDto FromEntity(Appointment a) =>
        new(a.Id, a.Client.FullName, a.Service.Name, a.Staff.FullName, a.Start, a.End, a.Status);
}

public static class AppointmentEndpoints
{
    public static void MapAppointmentEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/appointments")
            .RequireAuthorization(Policies.StaffOrAdmin);

        group.MapGet("/", async (DateOnly date, ClaimsPrincipal user, AppDbContext db) =>
        {
            var (startUtc, endUtc) = BusinessTime.DayRangeUtc(date);
            var query = db.Appointments.Where(a => a.Start >= startUtc && a.Start < endUtc);

            if (!user.IsInRole(Roles.Admin))
            {
                var staffId = user.GetStaffId();
                if (staffId is null) return Results.Forbid();

                query = query.Where(a => a.StaffId == staffId);
            }

            var items = await query
                .OrderBy(a => a.Start)
                .Select(a => new AppointmentDto(
                    a.Id, a.Client.FullName, a.Service.Name, a.Staff.FullName,
                    a.Start, a.End, a.Status))
                .ToListAsync();

            return Results.Ok(items);
        });

        group.MapGet("/{id:int}", async (
            int id, ClaimsPrincipal user, AppDbContext db, IAuthorizationService auth) =>
        {
            var appointment = await db.Appointments
                .AsNoTracking()
                .Include(a => a.Client)
                .Include(a => a.Service)
                .Include(a => a.Staff)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (appointment is null) return Results.NotFound();

            var result = await auth.AuthorizeAsync(user, appointment, Policies.CanViewAppointment);
            if (!result.Succeeded) return Results.NotFound();

            return Results.Ok(AppointmentDto.FromEntity(appointment));
        });
    }
}