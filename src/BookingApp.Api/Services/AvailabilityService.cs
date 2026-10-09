using BookingApp.Api.Models;
using BookingApp.Api.Repositories;

namespace BookingApp.Api.Services;

public class AvailabilityService(IAppointmentRepository repository) : IAvailabilityService
{
    public async Task<bool> IsSlotFreeAsync(DateTimeOffset start, int minutes)
    {
        var day = BusinessTime.DateOf(start);
        var end = start.AddMinutes(minutes);

        var dayAppointments = await repository.GetForDayAsync(day);

        return !dayAppointments
            .Where(a => a.Status != AppointmentStatus.Cancelled)
            .Any(a => Overlaps(a.Start, a.End, start, end));
    }

    private static bool Overlaps(
        DateTimeOffset startA, DateTimeOffset endA,
        DateTimeOffset startB, DateTimeOffset endB) =>
        startA < endB && startB < endA;
}