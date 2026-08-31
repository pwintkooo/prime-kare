using System.Security.Claims;
using PrimeKare.Api.Data;

namespace PrimeKare.Api.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(
        IHttpContextAccessor httpContextAccessor,
        AppDbContext context)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int? UserId
    {
        get
        {
            var userId = _httpContextAccessor
                .HttpContext?
                .User
                .FindFirstValue(
                    ClaimTypes.NameIdentifier);

            return int.TryParse(userId, out var id)
                ? id
                : null;
        }
    }

    public bool IsCustomer =>
        _httpContextAccessor
            .HttpContext?
            .User
            .IsInRole("Customer") == true;

    public bool IsAdmin =>
        _httpContextAccessor
            .HttpContext?
            .User
            .IsInRole("Admin") == true;

    public int? CustomerId
    {
        get
        {
            if (!IsCustomer)
            {
                return null;
            }

            var customerId = _httpContextAccessor
                .HttpContext?
                .User
                .FindFirstValue("CustomerId");

            return int.TryParse(customerId, out var id)
                ? id
                : null;
        }
    }
}