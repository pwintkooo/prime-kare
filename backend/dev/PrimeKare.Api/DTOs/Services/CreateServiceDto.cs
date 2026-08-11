using System.ComponentModel.DataAnnotations;

namespace PrimeKare.Api.DTOs.Services;
public class CreateServiceDto
{
    [Required]
    [MaxLength(100)]
    public string Name {get; set;} = string.Empty;

    [Required]
    [MaxLength(500)]
    public string Description {get; set;} = string.Empty;

    [Range(0, 100000)]
    public decimal Price {get; set;}

    [Range(1, 1440)]
    public int EstimatedMinutes {get; set;}
    public bool IsActive {get; set;} = true;
}