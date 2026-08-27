namespace PrimeKare.Api.Services;

public interface ICurrentUserService
{
    int? UserId { get; }

    int? CustomerId { get; }

    bool IsCustomer { get; }

    bool IsAdmin { get; }
}