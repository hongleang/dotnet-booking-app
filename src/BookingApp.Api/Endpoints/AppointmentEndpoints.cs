using BookingApp.Api.Data;
using BookingApp.Api.Models;
using BookingApp.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace BookingApp.Api.Endpoints;

public record AppointmentDto(
    int Id,
    Client Client,
    Service Service,
    Staff Staff,
    DateTimeOffset StartUtc,
    DateTimeOffset EndUtc,
    AppointmentStatus Status,
    string? Notes = null
);

public static class AppointmentEndpoints
{
    public record AppointmentFilter(DateOnly? Date, int? StaffId, AppointmentStatus? Status, int Page = 1);


    public record CreateAppointmenttRequest(
        DateTimeOffset Start,
        DateTimeOffset End,
        AppointmentStatus Status,
        string Notes,
        string ClientId,
        string StaffId,
        string Service);

    public static void MapAppointmentEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/appointments");

        group.MapGet("/", async ([AsParameters] AppointmentFilter filter, AppDbContext db) =>
        {
            var query = db.Appointments.AsQueryable();

            if (filter.Date is not null)
            {
                var (startUtc, endUtc) = BusinessTime.DayRangeUtc(filter.Date.Value);
                query = query.Where(a => a.Start >= startUtc && a.Start < endUtc);
            }

            return await query
                .OrderBy(a => a.Start)
                .Select(a => new AppointmentDto(
                    a.Id, a.Client, a.Service, a.Staff,
                    a.Start, a.End, a.Status))
                .ToListAsync();
        });

        group.MapPost("/", async (CreateAppointmenttRequest request, AppDbContext db) =>
        {
            var service =
                await db.Services.FirstOrDefaultAsync(service => service.Id == Convert.ToInt16(request.Service));
            var client = await db.Clients.FirstOrDefaultAsync(client => client.Id == Convert.ToInt16(request.ClientId));
            var staff = await db.Staff.FirstOrDefaultAsync(staff => staff.Id == Convert.ToInt16(request.StaffId));

            if (service == null || client == null || staff == null)
            {
                // TODO: move this logic to fluent validation
                return Results.BadRequest("Service or client or staff not found.");
            }

            var assignment = new Appointment
            {
                Start = request.Start.ToUniversalTime(),
                End = request.End.ToUniversalTime(),
                ClientId = client.Id,
                ServiceId = service.Id,
                Notes = request.Notes,
                StaffId = staff.Id,
            };

            db.Appointments.Add(assignment);
            await db.SaveChangesAsync();

            return Results.Created($"/appointments/{assignment.Id}", new AppointmentDto(
                assignment.Id,
                assignment.Client,
                assignment.Service,
                assignment.Staff,
                assignment.Start,
                assignment.End,
                assignment.Status,
                assignment.Notes
            ));
        });
        
        // Fails: saves a +10:30 offset
        group.MapPost("/timezone-trap", async (AppDbContext db) =>
        {
            var service = await db.Services.OrderBy(s => s.Id).FirstAsync();
            var clientId = await db.Clients.OrderBy(c => c.Id).Select(c => c.Id).FirstAsync();
            var staffId = await db.Staff.OrderBy(s => s.Id).Select(s => s.Id).FirstAsync();

            var start = new DateTimeOffset(2026, 10, 12, 9, 0, 0, TimeSpan.FromHours(10.5));

            db.Appointments.Add(new Appointment
            {
                ClientId = clientId,
                ServiceId = service.Id,
                StaffId = staffId,
                Start = start,
                End = start.AddMinutes(service.DurationMinutes),
                Notes = "tz-test"
            });

            try
            {
                await db.SaveChangesAsync();
                return Results.Ok(new { saved = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);   // full details in the output window
                return Results.Ok(new
                {
                    saved = false,
                    error = ex.GetType().Name,
                    message = ex.Message,
                    inner = ex.InnerException?.Message
                });
            }
        });
    }
}