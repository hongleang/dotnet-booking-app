using Bogus;
using BookingApp.Api.Auth;
using BookingApp.Api.Data.Factories;
using BookingApp.Api.Models;
using BookingApp.Api.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookingApp.Api.Data.Seeders;

public class DatabaseSeeder(
    AppDbContext db,
    UserManager<AppUser> users,
    RoleManager<IdentityRole<int>> roles,
    IConfiguration config,
    ILogger<DatabaseSeeder> logger
)
{
    public async Task SeedUserAsync()
    {
        foreach (var role in new[] { Roles.Admin, Roles.Staff })
        {
            if (!await roles.RoleExistsAsync(role))
            {
                await roles.CreateAsync(new IdentityRole<int>(role));
            }
        }

        // create an admin user
        await EnsureUserAsync("admin@example.com", config["Seed:AdminPassword"], Roles.Admin, staffId: null);
        
        // create user for each staff
        var staffPassword = config["Seed:StaffPassword"];
        foreach (var staff in await db.Staff.ToListAsync())
        {
            await EnsureUserAsync(staff.Email, staffPassword, Roles.Staff, staff.Id);
        }
    }

    public async Task SeedAsync()
    {
        if (await db.Clients.AnyAsync()) return;

        var clients = ClientFactory.Create().Generate(20);

        var services = await db.Services.ToListAsync();
        var staff = await db.Staff.ToListAsync();
        var rng = new Random(42);
        var today = BusinessTime.Today();

        for (var offset = 1; offset <= 14; offset++)
        {
            var day = today.AddDays(offset);
            if (day.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday) continue;

            foreach (var member in staff)
            {
                var cursor = new TimeOnly(9, 0);
                while (cursor < new TimeOnly(15, 0))
                {
                    if (rng.Next(10) < 3)
                    {
                        cursor = cursor.AddMinutes(30);
                        continue;
                    }

                    var service = services[rng.Next(services.Count)];
                    var start = BusinessTime.ToUtc(day, cursor);

                    db.Appointments.Add(new Appointment
                    {
                        Client = clients[rng.Next(clients.Count)],
                        Service = service,
                        Staff = member,
                        Start = start,
                        End = start.AddMinutes(service.DurationMinutes),
                        Status = rng.Next(10) == 0 ? AppointmentStatus.Cancelled : AppointmentStatus.Booked
                    });

                    cursor = cursor.AddMinutes(service.DurationMinutes + 15);
                }
            }
        }

        await db.SaveChangesAsync();
    }

    private async Task EnsureUserAsync(string email, string? password, string role, int? staffId)
    {
        var user = await users.FindByEmailAsync(email);

        if (user is null)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                logger.LogWarning("Seed:AdminPassword is not set; skipping the admin user.");
                return;
            }
        
            user = new AppUser { UserName = email, Email = email, EmailConfirmed = true, StaffId = staffId };
            ThrowIfFailed(await users.CreateAsync(user, password), email);
        }

        if (!await users.IsInRoleAsync(user, role))
        {
            ThrowIfFailed(await users.AddToRoleAsync(user, role), email);
        }
    }

    private static void ThrowIfFailed(IdentityResult result, string? email)
    {
        if (result.Succeeded) return;
        
        var errors = string.Join("; ", result.Errors.Select(e => e.Description));
        throw new InvalidOperationException($"Could not create the admin user: {errors}");
    }
}