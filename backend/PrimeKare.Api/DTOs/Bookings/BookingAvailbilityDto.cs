namespace PrimeKare.Api.DTOs.Bookings;

public class BookingAvailabilityDto
{
    public DateOnly Date { get; set; }
    public int ServiceId { get; set; }
    public List<string> AvailableTimes { get; set; } = [];
}