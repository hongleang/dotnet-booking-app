using BookingApp.Api.Models;

namespace BookingApp.Api.Repositories;

public interface IAppointmentRepository
{
    Task<List<Appointment>> GetForDayAsync(DateOnly day);
}