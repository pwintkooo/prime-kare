namespace PrimeKare.Api.DTOs.Auth;

public class ReactivateAccountDto
{
    public string Email {get; set;} = string.Empty;
    public string Password {get; set;} = string.Empty;
}