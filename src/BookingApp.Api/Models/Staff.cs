namespace BookingApp.Api.Models;

public class Staff
{
    public int Id { get; set; }
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public bool IsActive { get; set; } = true;
    
    public List<Appointment> Appointments { get; set; } = [];
}