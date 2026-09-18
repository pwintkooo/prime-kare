using PrimeKare.Api.Data;
using PrimeKare.Api.Models;
using Microsoft.Extensions.DependencyInjection;

namespace PrimeKare.Api.Tests.Helpers;

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
        AppDbContext context,
        string name = "PK1",
        string phone = "91234567",
        string email = "pk1@example.com")
    {
        var customer = new Customer
        {
            Name = name,
            Phone = phone,
            Email = email,
        };

        context.Customers.Add(customer);

        await context.SaveChangesAsync();

        return customer;
    }

    public static async Task<Vehicle> AddVehicle(
        AppDbContext context,
        int customerId,
        string plateNumber = "SLA1234A",
        string make = "Toyota",
        string model = "Camry",
        int year = 2024,
        string status = "active",
        bool isDeleted = false)
    {
        var vehicle = new Vehicle
        {
            PlateNumber = plateNumber,
            Make = make,
            Model = model,
            Year = year,
            CustomerId = customerId,
            Status = status,
            IsDeleted = isDeleted
        };

        context.Vehicles.Add(vehicle);

        await context.SaveChangesAsync();

        return vehicle;
    }
}