using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Net.Http.Headers;

namespace PrimeKare.Api.Tests.Helper;

public static class TestAuthHelper
{
    private const string JwtIssuer = "PrimeKare.Api";
    private const string JwtAudience = "PrimeKare.Frontend";

    public static string GenerateToken(
    IConfiguration configuration,
    int userId,
    string email,
    string role,
    int? customerId = null)
    {
        var jwtKey = configuration["Jwt:Key"]
            ?? throw new InvalidOperationException(
                "JWT key is not configured.");

        var claims = new List<Claim>
    {
        new Claim(
            ClaimTypes.NameIdentifier,
            userId.ToString()),

        new Claim(
            ClaimTypes.Email,
            email),

        new Claim(
            ClaimTypes.Role,
            role)
    };

        if (customerId.HasValue)
        {
            claims.Add(
                new Claim(
                    "CustomerId",
                    customerId.Value.ToString()));
        }

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtKey));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: JwtIssuer,
            audience: JwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }

    public static void AuthenticateAs(
    HttpClient client,
    IConfiguration configuration,
    int userId,
    string email,
    string role,
    int? customerId = null)
    {
        var token = GenerateToken(
            configuration,
            userId,
            email,
            role,
            customerId);

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                token);
    }

    public static void ClearAuthentication(
        HttpClient client)
    {
        client.DefaultRequestHeaders.Authorization = null;
    }
}