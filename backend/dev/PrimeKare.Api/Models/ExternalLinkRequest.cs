namespace PrimeKare.Api.Models;

public class ExternalLinkRequest
{
    public int Id { get; set; }

    public string Code { get; set; } = string.Empty;

    public int UserId { get; set; }

    public string Provider { get; set; } = string.Empty;

    public string ProviderUserId { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }

    public bool IsUsed { get; set; } = false;

    public User User { get; set; } = null!;
}