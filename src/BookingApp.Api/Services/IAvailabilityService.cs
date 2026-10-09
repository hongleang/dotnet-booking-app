namespace BookingApp.Api.Services;

public interface IAvailabilityService
{
    Task<bool> IsSlotFreeAsync(DateTimeOffset start, int minutes);
}