namespace PrimeKare.Api.Models;

public class Booking
{
    public int Id { get; set; }
    public string ReferenceNumber { get; set; } = string.Empty;

    // Registered customer
    public int? CustomerId { get; set; }
    public Customer? Customer { get; set; }

    // Saved vehicle
    public int? VehicleId { get; set; }
    public Vehicle? Vehicle { get; set; }

    public int ServiceId { get; set; }
    public Service Service { get; set; } = null!;

    // Customer / guest details
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;

    // Vehicle details
    public string VehiclePlateNumber { get; set; } = string.Empty;
    public string VehicleMake { get; set; } = string.Empty;
    public string VehicleModel { get; set; } = string.Empty;

    public DateOnly BookingDate { get; set; }

    public TimeSpan BookingTime { get; set; }

    public string Status { get; set; } = "pending";

    public string? Notes { get; set; }

    public string? GuestAccessToken { get; set; }
    
    public bool IsDeleted { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}