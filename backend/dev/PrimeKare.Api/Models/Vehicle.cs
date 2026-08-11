namespace PrimeKare.Api.Models;

public class Vehicle
{
    public int Id { get; set; }

    public string PlateNumber { get; set; } = string.Empty;

    public string Make { get; set; } = string.Empty;

    public string Model { get; set; } = string.Empty;

    public int Year { get; set; }

    // Foreign Key
    public int CustomerId { get; set; }

    // Navigation Property
    public Customer Customer { get; set; } = null!;
}