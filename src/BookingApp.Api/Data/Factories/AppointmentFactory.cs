using Bogus;
using BookingApp.Api.Models;

namespace BookingApp.Api.Data.Factories;

public static class AppointmentFactory
{
    public static Faker<Appointment> Create(
        Client client, Service service, Staff staff, DateTimeOffset startUtc,
        AppointmentStatus status = AppointmentStatus.Booked) =>
        new Faker<Appointment>()
            .RuleFor(a => a.Client, client)
            .RuleFor(a => a.Service, service)
            .RuleFor(a => a.Staff, staff)
            .RuleFor(a => a.Start, startUtc)
            .RuleFor(a => a.End, startUtc.AddMinutes(service.DurationMinutes))
            .RuleFor(a => a.Status, status); 
}