using Microsoft.AspNetCore.Mvc;
using PrimeKare.Api.Controllers;
using PrimeKare.Api.Services;
using PrimeKare.Api.Tests.Helpers;

namespace PrimeKare.Api.Tests.Controllers.Vehicles;

public class DeleteVehicleTests
{
    [Fact]
    public async Task AsCustomer_DeleteOwnVehicle_ReturnsNoContent()
    {
        using var context = TestDbContextFactory.Create();

        var customer = await TestDataHelper.AddCustomer(context);
        var vehicle = await TestDataHelper.AddVehicle(context, customer.Id);

        var currentUser = TestCurrentUserFactory.CreateCustomer(customer.Id);

        var vehicleService = new VehicleService(context);

        var controller = new VehiclesController(
            vehicleService,
            currentUser.Object);

        var result = await controller.DeleteVehicle(vehicle.Id);

        var noContentResult = Assert.IsType<NoContentResult>(result);

        Assert.Equal(204, noContentResult.StatusCode);

        var deletedVehicle = await context.Vehicles.FindAsync(vehicle.Id);

        Assert.NotNull(deletedVehicle);

        Assert.Equal(
            "archived",
            deletedVehicle.Status);

        Assert.False(
            deletedVehicle.IsDeleted);
    }

    [Fact]
    public async Task AsCustomer_DeleteAnotherCustomersVehicle_ReturnsForbidden()
    {
        using var context = TestDbContextFactory.Create();

        var customer1 = await TestDataHelper.AddCustomer(context);

        var customer2 = await TestDataHelper.AddCustomer(
            context,
            name: "PK2",
            phone: "12345678",
            email: "pk2@example.com");

        var vehicle = await TestDataHelper.AddVehicle(context, customer2.Id);

        var currentUser = TestCurrentUserFactory.CreateCustomer(customer1.Id);

        var vehicleService = new VehicleService(context);

        var controller = new VehiclesController(
            vehicleService,
            currentUser.Object);

        var result = await controller.DeleteVehicle(vehicle.Id);

        var forbiddenResult = Assert.IsType<ObjectResult>(result);

        Assert.Equal(403, forbiddenResult.StatusCode);
    }

    [Fact]
    public async Task AsAdmin_DeleteVehicle_ReturnsNoContent()
    {
        using var context = TestDbContextFactory.Create();

        var customer = await TestDataHelper.AddCustomer(context);
        var vehicle = await TestDataHelper.AddVehicle(context, customer.Id);

        var currentUser = TestCurrentUserFactory.CreateAdmin();

        var vehicleService = new VehicleService(context);

        var controller = new VehiclesController(
            vehicleService,
            currentUser.Object);

        var result = await controller.DeleteVehicle(vehicle.Id);

        var noContentResult = Assert.IsType<NoContentResult>(result);

        Assert.Equal(204, noContentResult.StatusCode);

        var deletedVehicle = await context.Vehicles.FindAsync(vehicle.Id);

        Assert.NotNull(deletedVehicle);

        Assert.Equal(
            "archived",
            deletedVehicle.Status);

        Assert.True(
            deletedVehicle.IsDeleted);
    }

    [Fact]
    public async Task AsCustomer_DeleteNonExistingVehicle_ReturnsNotFound()
    {
        using var context = TestDbContextFactory.Create();

        var customer = await TestDataHelper.AddCustomer(context);

        var currentUser = TestCurrentUserFactory.CreateCustomer(customer.Id);

        var vehicleService = new VehicleService(context);

        var controller = new VehiclesController(
            vehicleService,
            currentUser.Object);

        var result = await controller.DeleteVehicle(999);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);

        Assert.Equal(404, notFoundResult.StatusCode);
    }
}