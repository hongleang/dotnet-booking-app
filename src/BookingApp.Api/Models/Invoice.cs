namespace BookingApp.Api.Models;

public class Invoice
{
    public int Id { get; set; }
    public required string InvoiceNumber { get; set; }

    public int AppointmentId { get; set; }
    public Appointment Appointment { get; set; } = null!;

    public decimal Total { get; set; }
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;
    public DateTimeOffset IssuedAt { get; set; } = DateTimeOffset.UtcNow;
}