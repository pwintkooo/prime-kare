using Microsoft.AspNetCore.Mvc;
using PrimeKare.Api.Controllers;
using PrimeKare.Api.DTOs.Vehicles;
using PrimeKare.Api.Services;
using PrimeKare.Api.Tests.Helpers;

namespace PrimeKare.Api.Tests.Controllers.Vehicles;

public class CreateVehicleTests
{
    [Fact]
    public async Task CreateVehicle_WithValidCustomer_ReturnsCreatedVehicle()
    {
        using var context = TestDbContextFactory.Create();

        var customer = await TestDataHelper.AddCustomer(context);

        var currentUser = TestCurrentUserFactory.CreateCustomer(customer.Id);

        var vehicleService = new VehicleService(context);

        var controller = new VehiclesController(
            vehicleService,
            currentUser.Object);

        var request = TestDtoFactory.CreateVehicle();

        var result = await controller.CreateVehicle(request);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);

        var vehicle = Assert.IsType<VehicleDto>(createdResult.Value);

        Assert.Equal(request.PlateNumber.Trim().ToUpperInvariant(), vehicle.PlateNumber);
        Assert.Equal(request.Make.Trim().ToUpperInvariant(), vehicle.Make);
        Assert.Equal(request.Model.Trim().ToUpperInvariant(), vehicle.Model);
        Assert.Equal(request.Year, vehicle.Year);
        Assert.Equal(customer.Id, vehicle.CustomerId);
    }

    [Fact]
    public async Task CreateVehicle_WithCustomerIdMissing_ReturnsBadRequest()
    {
        using var context = TestDbContextFactory.Create();

        var currentUser = TestCurrentUserFactory.CreateCustomerWithoutCustomerId();

        var vehicleService = new VehicleService(context);

        var controller =
            new VehiclesController(
                vehicleService,
                currentUser.Object);

        var request = TestDtoFactory.CreateVehicle();

        var result = await controller.CreateVehicle(request);

        var badRequest =
            Assert.IsType<BadRequestObjectResult>(
                result.Result);

        Assert.Equal(400, badRequest.StatusCode);

        Assert.NotNull(badRequest.Value);

        var message = badRequest.Value
            .GetType()
            .GetProperty("message")?
            .GetValue(badRequest.Value);

        Assert.Equal(
            "Customer account is not properly configured.",
            message);
    }

    [Fact]
    public async Task CreateVehicle_WithCustomerNotExist_ReturnsNotFound()
    {
        using var context = TestDbContextFactory.Create();

        var currentUser = TestCurrentUserFactory.CreateCustomer(999);

        var vehicleService = new VehicleService(context);

        var controller =
            new VehiclesController(
                vehicleService,
                currentUser.Object);

        var request = TestDtoFactory.CreateVehicle();

        var result = await controller.CreateVehicle(request);

        var notFoundResult =
            Assert.IsType<NotFoundObjectResult>(
                result.Result);

        Assert.Equal(404, notFoundResult.StatusCode);
    }

    [Fact]
    public async Task CreateVehicle_WithDuplicatePlateNumber_ReturnsConflict()
    {
        using var context = TestDbContextFactory.Create();

        var customer = await TestDataHelper.AddCustomer(context);

        var currentUser = TestCurrentUserFactory.CreateCustomer(customer.Id);

        var vehicle = await TestDataHelper.AddVehicle(context, customer.Id);

        var request = TestDtoFactory.CreateVehicle(
            plateNumber: "SLA1234A"
        );

        var vehicleService = new VehicleService(context);

        var controller = new VehiclesController(
            vehicleService,
            currentUser.Object);

        var result = await controller.CreateVehicle(request);

        Assert.IsType<ConflictObjectResult>(result.Result);
    }
}