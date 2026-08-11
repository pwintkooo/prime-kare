using System.ComponentModel.DataAnnotations;

namespace PrimeKare.Api.DTOs.Vehicles;

public class CreateVehicleDto
{
    [Required]
    [MaxLength(20)]
    public string PlateNumber { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Make { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Model { get; set; } = string.Empty;

    [Range(1900, 2100)]
    public int Year { get; set; }

    [Required]
    public int CustomerId { get; set; }
}