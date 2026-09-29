namespace PrimeKare.Api.DTOs.Bookings;

public class CreateBookingDto
{
    public int? VehicleId { get; set; }

    public int ServiceId { get; set; }

    public DateOnly BookingDate { get; set; }

    public TimeSpan BookingTime { get; set; }

    public string? CustomerName { get; set; }
    public string? CustomerEmail { get; set; }
    public string? CustomerPhone { get; set; }

    public string? VehiclePlateNumber { get; set; }
    public string? VehicleMake { get; set; }
    public string? VehicleModel { get; set; }

    public string? Notes { get; set; }
}