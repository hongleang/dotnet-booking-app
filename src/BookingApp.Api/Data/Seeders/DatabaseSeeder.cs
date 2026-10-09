using Bogus;
using BookingApp.Api.Data.Factories;
using BookingApp.Api.Models;
using BookingApp.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace BookingApp.Api.Data.Seeders;

public class DatabaseSeeder(AppDbContext db)
{
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
}