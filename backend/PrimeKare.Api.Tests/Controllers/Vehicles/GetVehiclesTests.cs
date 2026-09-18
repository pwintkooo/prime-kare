using Microsoft.AspNetCore.Mvc;
using PrimeKare.Api.Controllers;
using PrimeKare.Api.DTOs.Vehicles;
using PrimeKare.Api.Services;
using PrimeKare.Api.Tests.Helpers;

namespace PrimeKare.Api.Tests.Controllers.Vehicles;

public class GetVehiclesTests
{
    [Fact]
    public async Task AsCustomer_WithOwnVehicles_ReturnsActiveVehicles()
    {
        using var context = TestDbContextFactory.Create();

        var customer1 = await TestDataHelper.AddCustomer(context);

        var customer2 = await TestDataHelper.AddCustomer(
            context,
            name: "PK2",
            email: "pk2@example.com",
            phone: "12345678");

        var vehicle1 = await TestDataHelper.AddVehicle(context, customer1.Id);

        var vehicle2 = await TestDataHelper.AddVehicle(
            context,
            customer1.Id,
            plateNumber: "ABC123",
            model: "Mercedes",
            make: "Benz",
            year: 2025);

        var vehicle3 = await TestDataHelper.AddVehicle(
            context,
            customer1.Id,
            plateNumber: "ABC123",
            model: "Mercedes",
            make: "Benz",
            year: 2025,
            status: "archived");

        var vehicle4 = await TestDataHelper.AddVehicle(
            context,
            customer2.Id,
            plateNumber: "DEF123",
            model: "Mercedes",
            make: "Benz",
            year: 2024);

        var currentUser = TestCurrentUserFactory.CreateCustomer(customer1.Id);

        var vehicleService = new VehicleService(context);

        var controller = new VehiclesController(
            vehicleService,
            currentUser.Object);

        var result = await controller.GetVehicles();

        var returnedVehicles = Assert.IsType<List<VehicleDto>>(result.Value);

        Assert.Equal(2, returnedVehicles.Count);
        Assert.Equal(vehicle1.PlateNumber, returnedVehicles[0].PlateNumber);
        Assert.Equal(vehicle2.PlateNumber, returnedVehicles[1].PlateNumber);
    }

    [Fact]
    public async Task AsAdmin_ReturnsAllActiveVehicles()
    {
        using var context = TestDbContextFactory.Create();

        var customer1 = await TestDataHelper.AddCustomer(context);

        var customer2 = await TestDataHelper.AddCustomer(
            context,
            name: "PK2",
            email: "pk2@example.com",
            phone: "12345678");

        var vehicle1 = await TestDataHelper.AddVehicle(context, customer1.Id);

        var vehicle2 = await TestDataHelper.AddVehicle(
            context,
            customer2.Id,
            plateNumber: "DEF123",
            model: "Mercedes",
            make: "Benz",
            year: 2024,
            status: "archived");

        var vehicle3 = await TestDataHelper.AddVehicle(
            context,
            customer2.Id,
            plateNumber: "GHI123",
            model: "Mercedes",
            make: "Benz",
            year: 2025,
            status: "archived",
            isDeleted: true);

        var currentUser = TestCurrentUserFactory.CreateAdmin();

        var vehicleService = new VehicleService(context);

        var controller = new VehiclesController(
            vehicleService,
            currentUser.Object);

        var result = await controller.GetVehicles();

        var returnedVehicles = Assert.IsType<List<VehicleDto>>(result.Value);

        Assert.Equal(2, returnedVehicles.Count);
        Assert.Contains(
            returnedVehicles,
             v => v.PlateNumber == vehicle1.PlateNumber);
        Assert.Contains(
            returnedVehicles,
            v => v.PlateNumber == vehicle2.PlateNumber);
        Assert.DoesNotContain(
            returnedVehicles,
            v => v.PlateNumber == vehicle3.PlateNumber);
    }
}