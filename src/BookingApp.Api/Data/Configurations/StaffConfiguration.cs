using BookingApp.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingApp.Api.Data.Configurations;

public class StaffConfiguration : IEntityTypeConfiguration<Staff>
{
    public void Configure(EntityTypeBuilder<Staff> builder)
    {
        builder.Property(s => s.FullName).HasMaxLength(200);
        builder.Property(s => s.Email).HasMaxLength(254);
        builder.HasIndex(s => s.Email).IsUnique();
        
        builder.HasData(
            new Staff { Id = 1, FullName = "Emma Walsh", Email = "emma@sunrisephysio.example" },
            new Staff { Id = 2, FullName = "Jack Nguyen", Email = "jack@sunrisephysio.example" });
    }
}