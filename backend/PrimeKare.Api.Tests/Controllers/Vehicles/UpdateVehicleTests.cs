using Microsoft.AspNetCore.Mvc;
using PrimeKare.Api.Controllers;
using PrimeKare.Api.Services;
using PrimeKare.Api.Tests.Helpers;

namespace PrimeKare.Api.Tests.Controllers.Vehicles;

public class UpdateVehicleTests
{
    [Fact]
    public async Task AsCustomer_UpdateOwnVehicle_ReturnsNoContent()
    {
        using var context = TestDbContextFactory.Create();

        var customer = await TestDataHelper.AddCustomer(context);
        var vehicle = await TestDataHelper.AddVehicle(context, customer.Id);

        var currentUser = TestCurrentUserFactory.CreateCustomer(customer.Id);

        var vehicleService = new VehicleService(context);

        var controller = new VehiclesController(
            vehicleService,
            currentUser.Object);

        var request = TestDtoFactory.UpdateVehicle();

        var result = await controller.UpdateVehicle(vehicle.Id, request);

        var noContentResult = Assert.IsType<NoContentResult>(result);

        Assert.Equal(204, noContentResult.StatusCode);

        var updatedVehicle = await context.Vehicles.FindAsync(vehicle.Id);

        Assert.NotNull(updatedVehicle);

        Assert.Equal(vehicle.Id, updatedVehicle.Id);
        Assert.Equal(request.PlateNumber.Trim().ToUpperInvariant(), updatedVehicle.PlateNumber);
        Assert.Equal(request.Make.Trim().ToUpperInvariant(), updatedVehicle.Make);
        Assert.Equal(request.Model.Trim().ToUpperInvariant(), updatedVehicle.Model);
        Assert.Equal(request.Year, updatedVehicle.Year);
        Assert.Equal(vehicle.CustomerId, updatedVehicle.CustomerId);
    }

    [Fact]
    public async Task AsCustomer_UpdateAnotherCustomersVehicle_ReturnsForbidden()
    {
        using var context = TestDbContextFactory.Create();

        var customer1 = await TestDataHelper.AddCustomer(context);

        var customer2 = await TestDataHelper.AddCustomer(
            context,
            name: "PK2",
            phone: "12345678",
            email: "pk2@example.com");

        var currentUser = TestCurrentUserFactory.CreateCustomer(customer1.Id);

        var vehicle = await TestDataHelper.AddVehicle(context, customer2.Id);

        var vehicleService = new VehicleService(context);

        var controller = new VehiclesController(
            vehicleService,
            currentUser.Object);

        var request = TestDtoFactory.UpdateVehicle();

        var result = await controller.UpdateVehicle(vehicle.Id, request);

        var forbiddenResult =
        Assert.IsType<ObjectResult>(result);

        Assert.Equal(
            403,
            forbiddenResult.StatusCode);
    }

    [Fact]
    public async Task AsAdmin_UpdateAnyActiveVehicle_ReturnsNoContent()
    {
        using var context = TestDbContextFactory.Create();

        var customer = await TestDataHelper.AddCustomer(context);
        var vehicle = await TestDataHelper.AddVehicle(context, customer.Id);

        var currentUser = TestCurrentUserFactory.CreateAdmin();

        var vehicleService = new VehicleService(context);

        var controller = new VehiclesController(
            vehicleService,
            currentUser.Object);

        var request = TestDtoFactory.UpdateVehicle();

        var result = await controller.UpdateVehicle(vehicle.Id, request);

        Assert.IsType<NoContentResult>(result);

        var updatedVehicle = await context.Vehicles.FindAsync(vehicle.Id);

        Assert.NotNull(updatedVehicle);

        Assert.Equal(vehicle.Id, updatedVehicle.Id);
        Assert.Equal(request.PlateNumber.Trim().ToUpperInvariant(), updatedVehicle.PlateNumber);
        Assert.Equal(request.Make.Trim().ToUpperInvariant(), updatedVehicle.Make);
        Assert.Equal(request.Model.Trim().ToUpperInvariant(), updatedVehicle.Model);
        Assert.Equal(request.Year, updatedVehicle.Year);
        Assert.Equal(vehicle.CustomerId, updatedVehicle.CustomerId);
    }

    [Fact]
    public async Task AsCustomer_UpdateNonExistingVehicle_ReturnsNotFound()
    {
        using var context = TestDbContextFactory.Create();

        var customer = await TestDataHelper.AddCustomer(context);

        var currentUser = TestCurrentUserFactory.CreateAdmin();

        var vehicleService = new VehicleService(context);

        var controller = new VehiclesController(
            vehicleService,
            currentUser.Object);

        var request = TestDtoFactory.UpdateVehicle();

        var result = await controller.UpdateVehicle(999, request);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);

        Assert.Equal(404, notFoundResult.StatusCode);
    }

    [Fact]
    public async Task UpdateVehicle_WithDuplicatePlateNumber_ReturnsConflict()
    {
        using var context = TestDbContextFactory.Create();

        var customer = await TestDataHelper.AddCustomer(context);
        var vehicle1 = await TestDataHelper.AddVehicle(context, customer.Id);

        var vehicle2 = await TestDataHelper.AddVehicle(
            context, 
            customer.Id,
            plateNumber: "ABC123");

        var currentUser = TestCurrentUserFactory.CreateCustomer(customer.Id);

        var vehicleService = new VehicleService(context);

        var controller = new VehiclesController(
            vehicleService,
            currentUser.Object);

        var request = TestDtoFactory.UpdateVehicle(plateNumber: "ABC123");

        var result = await controller.UpdateVehicle(vehicle1.Id, request);

        var conflictResult = Assert.IsType<ConflictObjectResult>(result);

        Assert.Equal(409, conflictResult.StatusCode);
    }

    [Fact]
    public async Task UpdateVehicle_WithArchivedStatus_ReturnsBadRequest()
    {
        using var context = TestDbContextFactory.Create();

        var customer = await TestDataHelper.AddCustomer(context);
        var vehicle = await TestDataHelper.AddVehicle(
            context, 
            customer.Id,
            status: "archived");

        var currentUser = TestCurrentUserFactory.CreateCustomer(customer.Id);

        var vehicleService = new VehicleService(context);

        var controller = new VehiclesController(
            vehicleService,
            currentUser.Object);

        var request = TestDtoFactory.UpdateVehicle();

        var result = await controller.UpdateVehicle(vehicle.Id, request);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);

        Assert.Equal(400, badRequestResult.StatusCode);
    }
}
