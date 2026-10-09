using Bogus;
using BookingApp.Api.Models;

namespace BookingApp.Api.Data.Factories;

public static class ClientFactory
{
    public static Faker<Client> Create() =>
        new Faker<Client>()
            .RuleFor(c => c.FullName, f => f.Name.FullName())
            .RuleFor(c => c.Email, f => f.Internet.Email().ToLowerInvariant())
            .RuleFor(c => c.Phone, f => f.Phone.PhoneNumber());
}