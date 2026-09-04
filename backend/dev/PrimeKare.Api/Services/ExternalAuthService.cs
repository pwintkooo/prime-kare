using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using PrimeKare.Api.Data;
using PrimeKare.Api.DTOs.Auth;
using PrimeKare.Api.Models;

namespace PrimeKare.Api.Services;

public class ExternalAuthService : IExternalAuthService
{
    private readonly AppDbContext _context;
    private readonly IAuthService _authService;

    public ExternalAuthService(
        AppDbContext context,
        IAuthService authService)
    {
        _context = context;
        _authService = authService;
    }

    public async Task<SignInResponseDto> HandleGoogleLoginAsync(
    ClaimsPrincipal principal)
    {
        var googleUserId = principal.FindFirstValue(
            ClaimTypes.NameIdentifier);

        var email = principal.FindFirstValue(
            ClaimTypes.Email);

        var name = principal.FindFirstValue(
            ClaimTypes.Name);

        if (string.IsNullOrWhiteSpace(googleUserId) ||
            string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidOperationException(
                "Google account information is incomplete.");
        }

        var externalLogin = await _context.ExternalLogins
            .Include(e => e.User)
            .FirstOrDefaultAsync(e =>
                e.Provider == "Google" &&
                e.ProviderUserId == googleUserId);

        if (externalLogin == null)
        {
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);

            if (existingUser != null)
            {
                throw new InvalidOperationException(
                    "An account with this email already exists. " +
                    "Please sign in using your existing login method.");
            }

            var customer = new Customer
            {
                Name = name,
                Email = email,
                Phone = null,
                Status = "active",
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var newUser = new User
            {
                Name = name,
                Email = email,
                Role = "Customer",
                Status = "active",
                Customer = customer,
                PasswordHash = null,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var newExternalLogin = new ExternalLogin
            {
                Provider = "Google",
                ProviderUserId = googleUserId,
                User = newUser
            };

            _context.Customers.Add(customer);
            _context.Users.Add(newUser);
            _context.ExternalLogins.Add(newExternalLogin);

            await _context.SaveChangesAsync();

            return _authService.CreateSignInResponse(newUser);
        }

        var user = externalLogin.User;

        if (user.Status != "active")
        {
            throw new UnauthorizedAccessException(
                "User account is not active.");
        }

        return _authService.CreateSignInResponse(user);
    }
}