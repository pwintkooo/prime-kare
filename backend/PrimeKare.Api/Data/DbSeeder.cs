using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PrimeKare.Api.Models;

namespace PrimeKare.Api.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(
        AppDbContext context,
        IPasswordHasher<User> passwordHasher,
        string adminEmail,
        string adminPassword)
    {
        if (await context.Users.AnyAsync(
            u => u.Role == "Admin"))
        {
            return;
        }

        var admin = new User
        {
            Email = adminEmail.Trim().ToLowerInvariant(),
            Role = "Admin",
            CustomerId = null
        };

        admin.PasswordHash = passwordHasher.HashPassword(
            admin,
            adminPassword
        );

        context.Users.Add(admin);

        await context.SaveChangesAsync();
    }
}