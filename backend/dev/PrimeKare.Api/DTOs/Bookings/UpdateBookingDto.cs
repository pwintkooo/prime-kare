namespace PrimeKare.Api.DTOs.Bookings;

public class UpdateBookingDto
{
    public int VehicleId { get; set; }
    
    public int ServiceId { get; set; }

    public DateOnly BookingDate { get; set; }

    public TimeSpan BookingTime { get; set; }

    public string? Notes { get; set; }
    
}