namespace BookingApp.Api.Models;

public class TimeOff
{
    public int Id { get; set; }

    public int StaffId { get; set; }
    public Staff Staff { get; set; } = null!;

    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
    public string? Reason { get; set; }
}