using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using FluentValidation;
using PrimeKare.Api.Data;
using PrimeKare.Api.DTOs.Auth;
using PrimeKare.Api.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using PrimeKare.Api.Services.Exceptions;

namespace PrimeKare.Api.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IConfiguration _configuration;
    private readonly IValidator<SignUpDto> _signUpValidator;

    public AuthService(
        AppDbContext context,
        IPasswordHasher<User> passwordHasher,
        IConfiguration configuration,
        IValidator<SignUpDto> signUpValidator)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _configuration = configuration;
        _signUpValidator = signUpValidator;
    }

    private string GenerateJwt(User user)
    {
        var claims = new List<Claim>
    {
        new Claim(
            ClaimTypes.NameIdentifier,
            user.Id.ToString()),

        new Claim(
            ClaimTypes.Email,
            user.Email),

        new Claim(
            ClaimTypes.Role,
            user.Role)
    };

        if (user.CustomerId.HasValue)
        {
            claims.Add(
                new Claim(
                    "CustomerId",
                    user.CustomerId.Value.ToString()));
        }

        var jwtKey = _configuration["Jwt:Key"];

        if (string.IsNullOrWhiteSpace(jwtKey))
        {
            throw new InvalidOperationException(
                "JWT key is not configured.");
        }

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtKey)
        );

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256
        );

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }

    public SignInResponseDto CreateSignInResponse(User user)
    {
        var token = GenerateJwt(user);

        return new SignInResponseDto
        {
            Token = token,

            User = new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role
            }
        };
    }

    public async Task<SignUpResponseDto> SignUpAsync(
    SignUpDto request)
    {
        var validationResult =
            await _signUpValidator
                .ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(
                validationResult.Errors
            );
        }

        var email = request.Email
            .Trim()
            .ToLowerInvariant();

        var existingUser =
            await _context.Users
                .FirstOrDefaultAsync(
                    u => u.Email == email
                );

        if (existingUser != null)
        {
            if (existingUser.Status == "inactive")
            {
                throw new InvalidOperationException(
                    "An account with this email already exists. Sign in to reactivate your account."
                );
            }

            throw new InvalidOperationException(
                "User already exists."
            );
        }

        var customer = new Customer
        {
            Name = request.Name.Trim(),
            Email = email,
            Phone = request.Phone,
            Status = "active",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Customers.Add(customer);

        var user = new User
        {
            Name = request.Name.Trim(),
            Email = email,
            Role = "Customer",
            Status = "active",
            Customer = customer,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        user.PasswordHash =
            _passwordHasher.HashPassword(
                user,
                request.Password
            );

        _context.Users.Add(user);

        await _context.SaveChangesAsync();

        return new SignUpResponseDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role,
            Status = user.Status,
            CreatedAt = user.CreatedAt,
            CustomerId = customer.Id,
            CustomerName = customer.Name,
            CustomerPhone = customer.Phone
        };
    }

    public async Task<SignInResponseDto> SignInAsync(
        SignInDto request)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var user = await _context.Users
            .FirstOrDefaultAsync(
                u => u.Email == email);

        if (user == null)
        {
            throw new KeyNotFoundException(
                "Invalid email or password."
            );
        }

        if (string.IsNullOrWhiteSpace(user.PasswordHash))
        {
            throw new InvalidOperationException(
                "Password sign in is not available for this account."
            );
        }

        var passwordResult =
            _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                request.Password
            );

        if (passwordResult ==
            PasswordVerificationResult.Failed)
        {
            throw new InvalidOperationException(
                "Invalid email or password."
            );
        }

        if (user.IsDeleted)
        {
            throw new InvalidOperationException(
                "This account is not available."
            );
        }

        if (user.Status == "inactive")
        {
            throw new AccountInactiveException(
                "Your account is inactive."
            );
        }

        if (user.Status != "active")
        {
            throw new InvalidOperationException(
                "This account is not available."
            );
        }

        return CreateSignInResponse(user);
    }

    public async Task<SignInResponseDto> ReactivateAccountAsync(
        ReactivateAccountDto request)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var user = await _context.Users
        .FirstOrDefaultAsync(u =>
        u.Email == email);

        if (user == null)
        {
            throw new KeyNotFoundException(
                "Account not found."
            );
        }

        if (user.IsDeleted)
        {
            throw new InvalidOperationException(
                "This account cannot be activated."
            );
        }

        if (user.Status == "active")
        {
            throw new InvalidOperationException(
                "This account is already active."
            );
        }

        if (string.IsNullOrWhiteSpace(user.PasswordHash))
        {
            throw new InvalidOperationException(
                "This account cannot be reactivated using a password."
            );
        }

        var passwordResult = _passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            request.Password
        );

        if (passwordResult == PasswordVerificationResult.Failed)
        {
            throw new InvalidOperationException(
                "Password is incorrect."
            );
        }

        user.Status = "active";
        user.UpdatedAt = DateTime.UtcNow;

        if (user.CustomerId != null)
        {
            var customer = await _context.Customers
            .FirstOrDefaultAsync(c =>
            c.Id == user.CustomerId.Value);

            if (customer != null)
            {
                customer.Status = "active";
                customer.UpdatedAt = DateTime.UtcNow;
            }
        }

        await _context.SaveChangesAsync();
        return CreateSignInResponse(user);
    }
}