namespace BookingApp.Api.Models;

public class WorkingPeriod
{
    public int Id { get; set; }

    public int StaffId { get; set; }
    public Staff Staff { get; set; } = null!;

    public DayOfWeek DayOfWeek { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
}