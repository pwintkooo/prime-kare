using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using PrimeKare.Api.Data;

namespace PrimeKare.Api.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly AppDbContext _context;

    public CurrentUserService(
        IHttpContextAccessor httpContextAccessor,
        AppDbContext context)
    {
        _httpContextAccessor = httpContextAccessor;
        _context = context;
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
            if (!IsCustomer || UserId == null)
            {
                return null;
            }

            var user = _context.Users
                .AsNoTracking()
                .FirstOrDefault(u =>
                    u.Id == UserId.Value);

            return user?.CustomerId;
        }
    }
}