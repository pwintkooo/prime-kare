namespace PrimeKare.Api.Models;

public class ExternalAuthCode
{
    public int Id { get; set; }

    public string Code { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }

    public bool IsUsed { get; set; } = false;
    public int UserId { get; set; }

    public User User { get; set; } = null!;
}