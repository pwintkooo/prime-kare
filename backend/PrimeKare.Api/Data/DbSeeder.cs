//create an admin account
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PrimeKare.Api.Models;

namespace PrimeKare.Api.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(
        AppDbContext context,
        IPasswordHasher<User> passwordHasher)
    {
        if (await context.Users.AnyAsync(u => u.Role == "Admin"))
        {
            return;
        }

        var admin = new User
        {
            Email = "admin@primekare.com",
            Role = "Admin",
            CustomerId = null
        };

        admin.PasswordHash = passwordHasher.HashPassword(
            admin,
            "Admin*123"
        );

        context.Users.Add(admin);

        await context.SaveChangesAsync();
    }
}