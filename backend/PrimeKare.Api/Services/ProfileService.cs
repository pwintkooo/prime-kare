using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PrimeKare.Api.Data;
using PrimeKare.Api.DTOs.Profile;
using PrimeKare.Api.Models;
using PrimeKare.Api.Services.Exceptions;

namespace PrimeKare.Api.Services;

public class ProfileService : IProfileService
{
    private readonly AppDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IValidator<ChangePasswordDto> _changePasswordValidator;
    private readonly IValidator<UpdateProfileDto> _updateProfileValidator;
    private readonly IValidator<ChangeEmailDto> _changeEmailValidator;

    public ProfileService(
        AppDbContext context,
        ICurrentUserService currentUserService,
        IPasswordHasher<User> passwordHasher,
        IValidator<ChangePasswordDto> changePasswordValidator,
        IValidator<UpdateProfileDto> updateProfileValidator,
        IValidator<ChangeEmailDto> changeEmailValidator)
    {
        _context = context;
        _currentUserService = currentUserService;
        _passwordHasher = passwordHasher;
        _changePasswordValidator = changePasswordValidator;
        _updateProfileValidator = updateProfileValidator;
        _changeEmailValidator = changeEmailValidator;
    }

    public async Task<ProfileDto> GetProfileAsync()
    {
        var userId = GetCurrentUserId();

        var user = await _context.Users
            .Include(u => u.ExternalLogins)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            throw new KeyNotFoundException(
                "User not found."
            );
        }

        var isExternalAccount =
            user.ExternalLogins.Any();

        string? phone = null;

        if (user.CustomerId != null)
        {
            phone = await _context.Customers
                .Where(c => c.Id == user.CustomerId.Value)
                .Select(c => c.Phone)
                .FirstOrDefaultAsync();
        }

        return new ProfileDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role,
            Phone = phone,

            HasPassword =
                !string.IsNullOrWhiteSpace(
                    user.PasswordHash
                ),

            CanChangePassword =
                !isExternalAccount &&
                !string.IsNullOrWhiteSpace(
                    user.PasswordHash
                ),

            CanChangeEmail =
                !isExternalAccount &&
                !string.IsNullOrWhiteSpace(
                    user.PasswordHash
                ),

            ExternalProviders = user.ExternalLogins
                .Select(login => login.Provider)
                .ToList(),
        };
    }

    public async Task UpdateProfileAsync(
        UpdateProfileDto dto)
    {
        var validationResult =
            await _updateProfileValidator
                .ValidateAsync(dto);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(
                validationResult.Errors
            );
        }

        var userId = GetCurrentUserId();

        var user = await _context.Users
            .FirstOrDefaultAsync(
                u => u.Id == userId
            );

        if (user == null)
        {
            throw new KeyNotFoundException(
                "User not found."
            );
        }

        var name = dto.Name.Trim();

        user.Name = name;
        user.UpdatedAt = DateTime.UtcNow;

        if (user.CustomerId != null)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(
                    c => c.Id == user.CustomerId.Value
                );

            if (customer != null)
            {
                customer.Name = name;
                customer.Phone =
                    string.IsNullOrWhiteSpace(dto.Phone)
                        ? null
                        : dto.Phone.Trim();

                customer.UpdatedAt =
                    DateTime.UtcNow;
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task ChangePasswordAsync(
        ChangePasswordDto dto)
    {
        var validationResult =
            await _changePasswordValidator
                .ValidateAsync(dto);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(
                validationResult.Errors
            );
        }

        var userId = GetCurrentUserId();

        var user = await _context.Users
            .Include(u => u.ExternalLogins)
            .FirstOrDefaultAsync(
                u => u.Id == userId
            );

        if (user == null)
        {
            throw new KeyNotFoundException(
                "User not found."
            );
        }

        if (user.ExternalLogins.Any())
        {
            throw new InvalidOperationException(
                "Password cannot be changed for an external login account."
            );
        }

        if (string.IsNullOrWhiteSpace(
            user.PasswordHash))
        {
            throw new InvalidOperationException(
                "Password change is not available for this account."
            );
        }

        var passwordResult =
            _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                dto.CurrentPassword
            );

        if (passwordResult ==
            PasswordVerificationResult.Failed)
        {
            throw new InvalidOperationException(
                "Current password is incorrect."
            );
        }

        user.PasswordHash =
            _passwordHasher.HashPassword(
                user,
                dto.NewPassword
            );

        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    public async Task ChangeEmailAsync(
        ChangeEmailDto dto)
    {

        var validationResult =
            await _changeEmailValidator
                .ValidateAsync(dto);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(
                validationResult.Errors
            );
        }

        var userId = GetCurrentUserId();

        var user = await _context.Users
        .Include(u => u.ExternalLogins)
        .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            throw new KeyNotFoundException(
                "User not found."
            );
        }

        if (user.ExternalLogins.Any())
        {
            throw new InvalidOperationException(
                "Email cannot be changed for an external login account."
            );
        }

        if (string.IsNullOrWhiteSpace(user.PasswordHash))
        {
            throw new InvalidOperationException(
                "Password authentication is not available for this account."
            );
        }

        var passwordResult =
        _passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            dto.CurrentPassword
        );

        if (passwordResult == PasswordVerificationResult.Failed)
        {
            throw new InvalidOperationException(
                "Current password is incorrect."
            );
        }

        var email = dto.NewEmail.Trim().ToLowerInvariant();

        if (string.Equals(
            email,
            user.Email,
            StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "New email cannot be the same as the current email."
                    );
                }

        var emailExists = await _context.Users
        .AnyAsync(u => u.Email == email && u.Id != userId);

        if (emailExists)
        {
            throw new ConflictException(
                "Email is already in use."
            );
        }

        user.Email = email;
        user.UpdatedAt = DateTime.UtcNow;

        if (user.CustomerId != null)
        {
            var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.Id == user.CustomerId.Value);

            if (customer != null)
            {
                customer.Email = email;
                customer.UpdatedAt = DateTime.UtcNow;
            }
        }

        await _context.SaveChangesAsync();
    }

    private int GetCurrentUserId()
    {
        var userId =
            _currentUserService.UserId;

        if (userId == null)
        {
            throw new UnauthorizedAccessException();
        }

        return userId.Value;
    }
}