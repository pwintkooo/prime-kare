using System.Security.Claims;
using System.Security.Cryptography;
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

    private static string GenerateAuthCode()
    {
        return Convert.ToBase64String(
            RandomNumberGenerator.GetBytes(32)
        );
    }

    public async Task<string> HandleGoogleLoginAsync(
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

        // Google account is not linked to PrimeKare yet
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

            var code = GenerateAuthCode();

            var authCode = new ExternalAuthCode
            {
                Code = code,
                User = newUser,
                ExpiresAt = DateTime.UtcNow.AddMinutes(2),
                IsUsed = false
            };

            _context.Customers.Add(customer);
            _context.Users.Add(newUser);
            _context.ExternalLogins.Add(newExternalLogin);
            _context.ExternalAuthCodes.Add(authCode);

            await _context.SaveChangesAsync();

            return code;
        }

        // Google account is already linked to PrimeKare
        var user = externalLogin.User;

        if (user.Status != "active")
        {
            throw new UnauthorizedAccessException(
                "User account is not active.");
        }

        var existingCode = GenerateAuthCode();

        var existingAuthCode = new ExternalAuthCode
        {
            Code = existingCode,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddMinutes(2),
            IsUsed = false
        };

        _context.ExternalAuthCodes.Add(existingAuthCode);

        await _context.SaveChangesAsync();

        return existingCode;
    }

    public async Task<SignInResponseDto?> ExchangeCodeAsync(
        string code)
    {
        var authCode = await _context.ExternalAuthCodes
            .Include(c => c.User)
            .FirstOrDefaultAsync(c =>
                c.Code == code);

        if (authCode == null)
            return null;

        if (authCode.IsUsed)
            return null;

        if (authCode.ExpiresAt <= DateTime.UtcNow)
            return null;

        var user = authCode.User;

        if (user.Status != "active")
            return null;

        authCode.IsUsed = true;

        await _context.SaveChangesAsync();

        return _authService.CreateSignInResponse(user);
    }
}