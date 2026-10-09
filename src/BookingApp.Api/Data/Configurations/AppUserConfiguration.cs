using BookingApp.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingApp.Api.Data.Configurations;

public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.HasOne(u => u.Staff)
            .WithOne()
            .HasForeignKey<AppUser>(u => u.StaffId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}