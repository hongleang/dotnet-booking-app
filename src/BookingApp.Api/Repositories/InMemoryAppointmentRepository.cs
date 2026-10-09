using BookingApp.Api.Models;

namespace BookingApp.Api.Repositories;

public class InMemoryAppointmentRepository : IAppointmentRepository
{
    private static readonly TimeSpan AdelaideOffset = TimeSpan.FromHours(10.5);

    private readonly List<Appointment> _appointments =
    [
        Make("Mia Chen",    12, 14,  0, 90, AppointmentStatus.Booked),
        Make("Liam Brown",  13,  9, 30, 30, AppointmentStatus.Booked),
        Make("Sam Taylor",  12,  9,  0, 60, AppointmentStatus.Booked),
        Make("Jordan Lee",  12, 10, 30, 30, AppointmentStatus.Cancelled),
        Make("Ava Wilson",  13, 11,  0, 60, AppointmentStatus.Booked),
        Make("Priya Nair",  12, 10,  0, 45, AppointmentStatus.Completed),
    ];

    public Task<List<Appointment>> GetForDayAsync(DateOnly day)
    {
        var result = _appointments
            .Where(a => DateOnly.FromDateTime(a.Start.DateTime) == day)
            .OrderBy(a => a.Start)
            .ToList();

        return Task.FromResult(result);
    }

    private static Appointment Make(
        string client, int day, int hour, int minute, int minutes, AppointmentStatus status)
    {
        var start = new DateTimeOffset(2026, 10, day, hour, minute, 0, AdelaideOffset);
        Client newClient = new Client
        {
            FullName = client,
            Email = "example@mail.com"
        };
            
        return new Appointment
        {
            Client = newClient,
            Start = start,
            End = start.AddMinutes(minutes),
            Status = status
        };
    }
}