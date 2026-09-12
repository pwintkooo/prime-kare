namespace PrimeKare.Api.DTOs.Profile;

public class ProfileDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public string? Phone { get; set; }

    public bool HasPassword { get; set; }

    public List<string> ExternalProviders { get; set; } = [];
}