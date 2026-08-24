using System.ComponentModel.DataAnnotations;

namespace PrimeKare.Api.DTOs.Customers;

public class UpdateCustomerDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string Phone { get; set; } = string.Empty;

    [Required]
    public string Status { get; set; } = "active";

    [EmailAddress]
    [MaxLength(150)]
    public string Email { get; set; } = string.Empty;
}