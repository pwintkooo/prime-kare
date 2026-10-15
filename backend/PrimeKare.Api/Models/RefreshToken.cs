namespace PrimeKare.Api.Models;

public class RefreshToken
{
    public int Id { get; set; }

    public int UserId { get; set; }

    // Store only the SHA-256 hash, never the raw refresh token.
    public string TokenHash { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime ExpiresAt { get; set; }

    public DateTime? RevokedAt { get; set; }

    // Supports refresh-token rotation and reuse detection.
    public int? ReplacedByTokenId { get; set; }

    public User User { get; set; } = null!;
}