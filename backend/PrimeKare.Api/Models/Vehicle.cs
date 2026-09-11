namespace PrimeKare.Api.Models;

public class Vehicle
{
    public int Id { get; set; }

    public string PlateNumber { get; set; } = string.Empty;

    public string Make { get; set; } = string.Empty;

    public string Model { get; set; } = string.Empty;

    public int Year { get; set; }

    public string Status { get; set; } = "active";
    
    public bool IsDeleted { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Foreign Key
    public int CustomerId { get; set; }

    // Navigation Property
    public Customer Customer { get; set; } = null!;

    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}