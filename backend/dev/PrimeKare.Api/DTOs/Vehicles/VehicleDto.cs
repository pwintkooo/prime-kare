namespace PrimeKare.Api.DTOs.Vehicles;

public class VehicleDto
{
    public int Id { get; set; }

    public string PlateNumber { get; set; } = string.Empty;

    public string Make { get; set; } = string.Empty;

    public string Model { get; set; } = string.Empty;

    public int Year { get; set; }

    public string Status { get; set; } = "active";
    
    public bool IsDeleted { get; set; } = false;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public int CustomerId { get; set; }
}