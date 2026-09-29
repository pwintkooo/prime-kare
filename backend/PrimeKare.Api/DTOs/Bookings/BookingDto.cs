namespace PrimeKare.Api.DTOs.Bookings;

public class BookingDto
{
    public int Id { get; set; }
    public string ReferenceNumber { get; set; } = string.Empty;
    public int? CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;

    public int? VehicleId { get; set; }
    public string VehiclePlateNumber { get; set; } = string.Empty;
    public string VehicleMake { get; set; } = string.Empty;
    public string VehicleModel { get; set; } = string.Empty;

    public int ServiceId { get; set; }
    public string ServiceName { get; set; } = string.Empty;

    public DateOnly BookingDate { get; set; }

    public TimeSpan BookingTime { get; set; }

    public string Status { get; set; } = string.Empty;

    public string? Notes { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}