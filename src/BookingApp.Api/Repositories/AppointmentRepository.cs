using BookingApp.Api.Data;
using BookingApp.Api.Models;
using BookingApp.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace BookingApp.Api.Repositories;

public class AppointmentRepository(AppDbContext db): IAppointmentRepository
{
    public async Task<List<Appointment>> GetForDayAsync(DateOnly day)
    {
        var (startUtc, endUtc) = BusinessTime.DayRangeUtc(day);

        return await db.Appointments
            .AsNoTracking()
            .Where(a => a.Start >= startUtc && a.Start < endUtc)
            .OrderBy(a => a.Start)
            .ToListAsync();
    }
}