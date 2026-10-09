namespace BookingApp.Api.Models;

public class Client
{
    public int Id { get; set; }
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public string? Phone { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    
    public List<Appointment> Appointments { get; set; } = [];
}