using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using PrimeKare.Api.Data;
using PrimeKare.Api.DTOs.Auth;
using PrimeKare.Api.Models;
using PrimeKare.Api.Services.Exceptions;

namespace PrimeKare.Api.Services;

public class ExternalAuthService : IExternalAuthService
{
    private readonly AppDbContext _context;
    private readonly IAuthService _authService;
    private readonly IPasswordHasher<User> _passwordHasher;

    public ExternalAuthService(
        AppDbContext context,
        IAuthService authService,
        IPasswordHasher<User> passwordHasher)
    {
        _context = context;
        _authService = authService;
        _passwordHasher = passwordHasher;
    }

    private static string GenerateAuthCode()
    {
        return Convert.ToBase64String(
            RandomNumberGenerator.GetBytes(32)
        );
    }

    public async Task<ExternalAuthResult> HandleGoogleLoginAsync(
    ClaimsPrincipal principal)
    {
        var googleUserId = principal.FindFirstValue(
            ClaimTypes.NameIdentifier);

        var email = principal.FindFirstValue(
            ClaimTypes.Email)?.Trim().ToLowerInvariant();

        var name = principal.FindFirstValue(
            ClaimTypes.Name);

        if (string.IsNullOrWhiteSpace(googleUserId) ||
            string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidOperationException(
                "Google account information is incomplete.");
        }

        // 1. Check whether this Google account is already linked
        var externalLogin = await _context.ExternalLogins
            .Include(e => e.User)
            .FirstOrDefaultAsync(e =>
                e.Provider == "Google" &&
                e.ProviderUserId == googleUserId);

        if (externalLogin != null)
        {
            var user = externalLogin.User;

            if (user.IsDeleted)
            {
                throw new InvalidOperationException(
                    "This account cannot be reactivated."
                );
            }

            if (user.Status == "inactive")
            {
                user.Status = "active";
                user.UpdatedAt = DateTime.UtcNow;

                if (user.CustomerId.HasValue)
                {
                    var customer =
                        await _context.Customers
                            .FirstOrDefaultAsync(c =>
                                c.Id == user.CustomerId.Value);

                    if (customer != null)
                    {
                        customer.Status = "active";
                        customer.UpdatedAt =
                            DateTime.UtcNow;
                    }
                }
            }
            else if (user.Status != "active")
            {
                throw new InvalidOperationException(
                    "This account is not available."
                );
            }

            var code = GenerateAuthCode();

            var authCode = new ExternalAuthCode
            {
                Code = code,
                UserId = user.Id,
                ExpiresAt =
                    DateTime.UtcNow.AddMinutes(2),
                IsUsed = false
            };

            _context.ExternalAuthCodes.Add(
                authCode);

            await _context.SaveChangesAsync();

            return new ExternalAuthResult
            {
                Code = code,
                Type = "login"
            };
        }

        // 2. Google isn't linked.
        // Check whether a PrimeKare account already uses this email.
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email);

        // 3. No existing account → create a new PrimeKare account
        if (existingUser == null)
        {
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

            return new ExternalAuthResult
            {
                Code = code,
                Type = "login"
            };

        }

        // 4. Existing PrimeKare account but Google isn't linked.
        // Create a temporary request to link Google after
        // the user proves ownership of the existing account.
        var linkCode = GenerateAuthCode();

        var linkRequest = new ExternalLinkRequest
        {
            Code = linkCode,
            UserId = existingUser.Id,
            Provider = "Google",
            ProviderUserId = googleUserId,
            ExpiresAt = DateTime.UtcNow.AddMinutes(2),
            IsUsed = false
        };

        _context.ExternalLinkRequests.Add(linkRequest);

        await _context.SaveChangesAsync();

        return new ExternalAuthResult
        {
            Code = linkCode,
            Type = "link"
        };
    }

    public async Task<SignInResponseDto>
    ExchangeCodeAsync(string code)
    {
        var authCode =
            await _context.ExternalAuthCodes
                .Include(c => c.User)
                .FirstOrDefaultAsync(c =>
                    c.Code == code);

        if (authCode == null)
        {
            throw new InvalidOperationException(
                "Authorization code is invalid."
            );
        }

        if (authCode.IsUsed)
        {
            throw new InvalidOperationException(
                "Authorization code has already been used."
            );
        }

        if (authCode.ExpiresAt <= DateTime.UtcNow)
        {
            throw new InvalidOperationException(
                "Authorization code has expired."
            );
        }

        var user = authCode.User;

        if (user.IsDeleted)
        {
            throw new InvalidOperationException(
                "This account is not available."
            );
        }

        if (user.Status != "active")
        {
            throw new InvalidOperationException(
                "This account is not active."
            );
        }

        authCode.IsUsed = true;

        await _context.SaveChangesAsync();

        return _authService
            .CreateSignInResponse(user);
    }

    public async Task<SignInResponseDto>
    VerifyAndLinkGoogleAsync(
        string code,
        string password)
    {
        var linkRequest =
            await _context.ExternalLinkRequests
                .Include(r => r.User)
                .FirstOrDefaultAsync(r =>
                    r.Code == code);

        if (linkRequest == null)
        {
            throw new InvalidOperationException(
                "Link request is invalid."
            );
        }

        if (linkRequest.IsUsed)
        {
            throw new InvalidOperationException(
                "Link request has already been used."
            );
        }

        if (linkRequest.ExpiresAt <= DateTime.UtcNow)
        {
            throw new InvalidOperationException(
                "Link request has expired."
            );
        }

        var user = linkRequest.User;

        if (user.IsDeleted)
        {
            throw new InvalidOperationException(
                "This account is not available."
            );
        }

        if (user.Status != "active")
        {
            throw new InvalidOperationException(
                "This account is not active."
            );
        }

        if (string.IsNullOrWhiteSpace(
            user.PasswordHash))
        {
            throw new InvalidOperationException(
                "Password verification is not available for this account."
            );
        }

        var passwordResult =
            _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                password
            );

        if (passwordResult ==
            PasswordVerificationResult.Failed)
        {
            throw new InvalidOperationException(
                "Password is incorrect."
            );
        }

        var existingExternalLogin =
            await _context.ExternalLogins
                .FirstOrDefaultAsync(e =>
                    e.Provider ==
                        linkRequest.Provider &&
                    e.ProviderUserId ==
                        linkRequest.ProviderUserId);

        if (existingExternalLogin != null)
        {
            throw new ConflictException(
                "This Google account is already linked to another account."
            );
        }

        var externalLogin = new ExternalLogin
        {
            UserId = user.Id,
            Provider = linkRequest.Provider,
            ProviderUserId =
                linkRequest.ProviderUserId
        };

        _context.ExternalLogins.Add(
            externalLogin);

        linkRequest.IsUsed = true;

        await _context.SaveChangesAsync();

        return _authService
            .CreateSignInResponse(user);
    }
}