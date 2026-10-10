using Microsoft.AspNetCore.Identity;

namespace BookingApp.Api.Models;

public class AppUser : IdentityUser<int>
{
    public int? StaffId { get; set; }
    public Staff? Staff { get; set; }
}