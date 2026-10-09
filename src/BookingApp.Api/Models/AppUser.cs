using Microsoft.AspNetCore.Identity;

namespace BookingApp.Api.Models;

public class AppUser : IdentityUser
{
    public int? StaffId { get; set; }
    public Staff? Staff { get; set; }
}