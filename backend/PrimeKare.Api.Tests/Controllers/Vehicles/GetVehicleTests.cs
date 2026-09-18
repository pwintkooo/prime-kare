using Microsoft.AspNetCore.Mvc;
using PrimeKare.Api.Controllers;
using PrimeKare.Api.DTOs.Vehicles;
using PrimeKare.Api.Services;
using PrimeKare.Api.Tests.Helpers;

namespace PrimeKare.Api.Tests.Controllers.Vehicles;

public class GetVehicleTests
{
    [Fact]
    public async Task AsCustomer_WithOwnVehicle_ReturnsVehicle()
    {
        using var context = TestDbContextFactory.Create();

        var customer = await TestDataHelper.AddCustomer(context);
        var vehicle = await TestDataHelper.AddVehicle(context, customer.Id);

        var currentUser = TestCurrentUserFactory.CreateCustomer(customer.Id);

        var vehicleService = new VehicleService(context);

        var controller = new VehiclesController(
            vehicleService,
            currentUser.Object);

        var result = await controller.GetVehicle(vehicle.Id);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);

        var vehicleDto = Assert.IsType<VehicleDto>(okResult.Value);

        Assert.Equal(vehicle.Id, vehicleDto.Id);
        Assert.Equal(vehicle.PlateNumber, vehicleDto.PlateNumber);
        Assert.Equal(vehicle.Make, vehicleDto.Make);
        Assert.Equal(vehicle.Model, vehicleDto.Model);
        Assert.Equal(vehicle.Year, vehicleDto.Year);
        Assert.Equal(vehicle.CustomerId, vehicleDto.CustomerId);
    }

    [Fact]
    public async Task AsCustomer_WithAnotherCustomersVehicle_ReturnsNotFound()
    {
        using var context = TestDbContextFactory.Create();

        var customer1 = await TestDataHelper.AddCustomer(context);

        var customer2 = await TestDataHelper.AddCustomer(
            context,
            name: "PK2",
            phone: "12345678",
            email: "pk2@example.com"
        );

        var vehicle = await TestDataHelper.AddVehicle(context, customer2.Id);

        var currentUser = TestCurrentUserFactory.CreateCustomer(customer1.Id);

        var vehicleService = new VehicleService(context);

        var controller = new VehiclesController(
            vehicleService,
            currentUser.Object);

        var result = await controller.GetVehicle(vehicle.Id);

        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task AsCustomer_WithNonExistingVehicle_ReturnsNotFound()
    {
        using var context = TestDbContextFactory.Create();

        var customer = await TestDataHelper.AddCustomer(context);

        var currentUser = TestCurrentUserFactory.CreateCustomer(customer.Id);

        var vehicleService = new VehicleService(context);

        var controller = new VehiclesController(
            vehicleService,
            currentUser.Object);

        var result = await controller.GetVehicle(999);

        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task AsAdmin_WithExistingVehicle_ReturnsVehicle()
    {
        using var context = TestDbContextFactory.Create();

        var customer = await TestDataHelper.AddCustomer(context);
        var vehicle = await TestDataHelper.AddVehicle(context, customer.Id);

        var currentUser = TestCurrentUserFactory.CreateAdmin();

        var vehicleService = new VehicleService(context);

        var controller = new VehiclesController(
            vehicleService,
            currentUser.Object);

        var result = await controller.GetVehicle(vehicle.Id);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);

        var vehicleDto = Assert.IsType<VehicleDto>(okResult.Value);

        Assert.Equal(vehicle.Id, vehicleDto.Id);
        Assert.Equal(vehicle.PlateNumber, vehicleDto.PlateNumber);
        Assert.Equal(vehicle.Make, vehicleDto.Make);
        Assert.Equal(vehicle.Model, vehicleDto.Model);
        Assert.Equal(vehicle.Year, vehicleDto.Year);
        Assert.Equal(vehicle.CustomerId, vehicleDto.CustomerId);
    }

    [Fact]
    public async Task AsAdmin_WithNonExistingVehicle_ReturnsNotFound()
    {
        using var context = TestDbContextFactory.Create();

        var customer = await TestDataHelper.AddCustomer(context);

        var currentUser = TestCurrentUserFactory.CreateAdmin();

        var vehicleService = new VehicleService(context);

        var controller = new VehiclesController(
            vehicleService,
            currentUser.Object);

        var result = await controller.GetVehicle(999);

        Assert.IsType<NotFoundObjectResult>(result.Result);
    }
}