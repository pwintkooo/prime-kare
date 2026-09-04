namespace PrimeKare.Api.Models;

public class ExternalLogin
{
    public int Id { get; set; }
    public string Provider { get; set; } = string.Empty;
    public string ProviderUserId { get; set; } = string.Empty;
    public int UserId { get; set; }
    public User User { get; set; } = null!;
}