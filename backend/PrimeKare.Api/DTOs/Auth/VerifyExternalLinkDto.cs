namespace PrimeKare.Api.DTOs.Auth;

public class VerifyExternalLinkDto
{
    public string Code { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}