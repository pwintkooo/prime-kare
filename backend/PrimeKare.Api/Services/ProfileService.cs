using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PrimeKare.Api.Data;
using PrimeKare.Api.DTOs.Profile;
using PrimeKare.Api.Models;

namespace PrimeKare.Api.Services;

public class ProfileService : IProfileService
{
    private readonly AppDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IValidator<ChangePasswordDto>
        _changePasswordValidator;

    public ProfileService(
        AppDbContext context,
        ICurrentUserService currentUserService,
        IPasswordHasher<User> passwordHasher,
        IValidator<ChangePasswordDto> changePasswordValidator)
    {
        _context = context;
        _currentUserService = currentUserService;
        _passwordHasher = passwordHasher;
        _changePasswordValidator = changePasswordValidator;
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

            ExternalProviders = user.ExternalLogins
                .Select(login => login.Provider)
                .ToList(),
        };
    }

    public async Task UpdateProfileAsync(
        UpdateProfileDto dto)
    {
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
                    dto.Phone?.Trim();

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
            .FirstOrDefaultAsync(
                u => u.Id == userId
            );

        if (user == null)
        {
            throw new KeyNotFoundException(
                "User not found."
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