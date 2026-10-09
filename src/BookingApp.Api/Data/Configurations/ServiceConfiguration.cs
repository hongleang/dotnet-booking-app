using BookingApp.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingApp.Api.Data.Configurations;

public class ServiceConfiguration : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> builder)
    {
        builder.Property(s => s.Name).HasMaxLength(100);
        builder.Property(s => s.Price).HasPrecision(10, 2);
        
        builder.HasData(
            new Service { Id = 1, Name = "Initial Consultation", DurationMinutes = 60,  Price = 120.00m },
            new Service { Id = 2, Name = "Standard Physio",      DurationMinutes = 45,  Price = 89.50m },
            new Service { Id = 3, Name = "Remedial Massage",     DurationMinutes = 60,  Price = 110.00m },
            new Service { Id = 4, Name = "Dry Needling",         DurationMinutes = 30,  Price = 75.00m },
            new Service { Id = 5, Name = "Follow-up Check",      DurationMinutes = 30,  Price = 60.00m });
    }
}