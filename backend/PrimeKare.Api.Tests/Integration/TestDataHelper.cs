using PrimeKare.Api.Data;
using PrimeKare.Api.Models;
using Microsoft.Extensions.DependencyInjection;

namespace PrimeKare.Api.Tests.Integration;

public static class TestDataHelper
{
    public static async Task<AppDbContext> CreateCleanDatabase(
        CustomWebApplicationFactory factory)
    {
        var scope = factory.Services.CreateScope();

        var context = scope.ServiceProvider
            .GetRequiredService<AppDbContext>();

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        return context;
    }

    public static async Task<Customer> AddCustomer(
        AppDbContext context)
    {
        var customer = new Customer
        {
            Id = 1,
            Name = "PK",
            Phone = "91234567",
            Email = "pk@example.com"
        };

        context.Customers.Add(customer);

        await context.SaveChangesAsync();

        return customer;
    }
}