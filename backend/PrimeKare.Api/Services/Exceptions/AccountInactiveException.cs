namespace PrimeKare.Api.Services.Exceptions;

public class AccountInactiveException : Exception
{
    public AccountInactiveException(string message)
        : base(message)
    {
    }
}