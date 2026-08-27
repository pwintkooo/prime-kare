using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using PrimeKare.Api.Controllers;
using PrimeKare.Api.Data;
using PrimeKare.Api.DTOs.Vehicles;
using PrimeKare.Api.Models;
using PrimeKare.Api.Services;
using Moq;

namespace PrimeKare.Api.Tests.Controllers;

public class VehiclesControllerTests
{
    private AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task CreateVehicle_WithValidCustomer_ReturnsCreatedVehicle()
    {
        //arrange
        using var context = CreateDbContext();

        var customer = new Customer
        {
            Id = 1,
            Name = "PK",
            Phone = "1234",
            Email = "PK@gmail.com"
        };

        context.Customers.Add(customer);
        await context.SaveChangesAsync();

        var vehicleService = new VehicleService(context);
        var currentUser = new Mock<ICurrentUserService>();

        var controller = new VehiclesController(
            vehicleService,
            currentUser.Object);

        var dto = new CreateVehicleDto
        {
            PlateNumber = "SLA1234A",
            Make = "Toyota",
            Model = "Camry",
            Year = 2024,
            CustomerId = 1
        };

        //act
        var result = await controller.CreateVehicle(dto);

        //assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);

        var vehicle = Assert.IsType<VehicleDto>(createdResult.Value);

        Assert.Equal("SLA1234A", vehicle.PlateNumber);
        Assert.Equal("Toyota", vehicle.Make);
        Assert.Equal("Camry", vehicle.Model);
        Assert.Equal(2024, vehicle.Year);
        Assert.Equal(1, vehicle.CustomerId);
    }

    [Fact]
    public async Task CreateVehicle_WithInvalidCustomer_ReturnsBadRequest()
    {
        using var context = CreateDbContext();

        var vehicleService = new VehicleService(context);
        var currentUser = new Mock<ICurrentUserService>();

        var controller = new VehiclesController(
            vehicleService,
            currentUser.Object);

        var dto = new CreateVehicleDto
        {
            PlateNumber = "SLA1234A",
            Make = "Toyota",
            Model = "Camry",
            Year = 2024,
            CustomerId = 1
        };

        var result = await controller.CreateVehicle(dto);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);

        Assert.Equal(
            "Customer does not exist!",
            badRequestResult.Value
        );
    }

    [Fact]
    public async Task GetVehicle_WithExistingId_ReturnsVehicle()
    {
        using var context = CreateDbContext();

        var vehicle = new Vehicle
        {
            Id = 1,
            PlateNumber = "SLA1234A",
            Make = "Toyota",
            Model = "Camry",
            Year = 2024,
            CustomerId = 1
        };

        context.Vehicles.Add(vehicle);
        await context.SaveChangesAsync();

        var vehicleService = new VehicleService(context);

        var currentUser = new Mock<ICurrentUserService>();

        currentUser.Setup(x => x.IsCustomer)
            .Returns(false);

        var controller = new VehiclesController(
            vehicleService,
            currentUser.Object);

        var result = await controller.GetVehicle(1);

        var vehicleDto = Assert.IsType<VehicleDto>(result.Value);

        Assert.Equal(1, vehicleDto.Id);
        Assert.Equal("SLA1234A", vehicleDto.PlateNumber);
        Assert.Equal("Toyota", vehicleDto.Make);
        Assert.Equal("Camry", vehicleDto.Model);
        Assert.Equal(2024, vehicleDto.Year);
        Assert.Equal(1, vehicleDto.CustomerId);
    }

    [Fact]
    public async Task GetVehicles_ReturnsMultipleVehicles()
    {
        using var context = CreateDbContext();

        var vehicles = new List<Vehicle>
        {
            new Vehicle
            {
                Id = 1,
                PlateNumber = "SLA1234A",
                Make = "Toyota",
                Model = "Camry",
                Year = 2024,
                CustomerId = 1,
                Status = "active"
            },
            new Vehicle
            {
                Id = 2,
                PlateNumber = "SLA1234B",
                Make = "Toyota",
                Model = "Camry",
                Year = 2024,
                CustomerId = 1,
                Status = "active"
            },
            new Vehicle
            {
                Id = 3,
                PlateNumber = "SLA1234C",
                Make = "Toyota",
                Model = "Camry",
                Year = 2024,
                CustomerId = 1,
                Status = "active"
            }
        };

        context.Vehicles.AddRange(vehicles);
        await context.SaveChangesAsync();

        var vehicleService = new VehicleService(context);

        var currentUser = new Mock<ICurrentUserService>();

        currentUser.Setup(x => x.IsCustomer)
            .Returns(false);

        var controller = new VehiclesController(
            vehicleService,
            currentUser.Object);

        var result = await controller.GetVehicles();

        var returnedVehicles = Assert.IsType<List<VehicleDto>>(result.Value);

        Assert.Equal(3, returnedVehicles.Count);
        Assert.Equal("SLA1234A", returnedVehicles[0].PlateNumber);
        Assert.Equal("SLA1234B", returnedVehicles[1].PlateNumber);
        Assert.Equal("SLA1234C", returnedVehicles[2].PlateNumber);
    }

    [Fact]
    public async Task GetVehicle_WithNonExistingId_ReturnsNotFound()
    {
        using var context = CreateDbContext();

        var vehicleService = new VehicleService(context);

        var currentUser = new Mock<ICurrentUserService>();

        currentUser.Setup(x => x.IsCustomer)
            .Returns(false);

        var controller = new VehiclesController(
            vehicleService,
            currentUser.Object);

        var result = await controller.GetVehicle(1);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task UpdateVehicle_WithValidData_ReturnsNoContent()
    {
        //arrange
        using var context = CreateDbContext();

        var customer = new Customer
        {
            Id = 1,
            Name = "PK",
            Phone = "1234",
            Email = "PK@gmail.com"
        };

        var vehicle = new Vehicle
        {
            Id = 1,
            PlateNumber = "SLA1234A",
            Make = "Toyota",
            Model = "Camry",
            Year = 2024,
            CustomerId = 1
        };

        context.Customers.Add(customer);
        context.Vehicles.Add(vehicle);
        await context.SaveChangesAsync();

        var vehicleService = new VehicleService(context);
        var currentUser = new Mock<ICurrentUserService>();

        var controller = new VehiclesController(
            vehicleService,
            currentUser.Object);

        var dto = new UpdateVehicleDto
        {
            PlateNumber = "SLB5678B",
            Make = "Honda",
            Model = "Civic",
            Year = 2025,
            CustomerId = 1
        };

        //act
        var result = await controller.UpdateVehicle(1, dto);

        //assert
        Assert.IsType<NoContentResult>(result);

        var updatedVehicle = await context.Vehicles.FindAsync(1);

        Assert.NotNull(updatedVehicle);
        Assert.Equal("SLB5678B", updatedVehicle.PlateNumber);
        Assert.Equal("Honda", updatedVehicle.Make);
        Assert.Equal("Civic", updatedVehicle.Model);
        Assert.Equal(2025, updatedVehicle.Year);
        Assert.Equal(1, updatedVehicle.CustomerId);
    }

    [Fact]
    public async Task DeleteVehicle_WithExistingVehicle_ReturnsNoContent()
    {
        //arrange
        using var context = CreateDbContext();

        var vehicle = new Vehicle
        {
            Id = 1,
            PlateNumber = "SLA1234A",
            Make = "Toyota",
            Model = "Camry",
            Year = 2024,
            CustomerId = 1,
            Status = "active"
        };

        context.Vehicles.Add(vehicle);
        await context.SaveChangesAsync();

        var vehicleService = new VehicleService(context);
        var currentUser = new Mock<ICurrentUserService>();

        currentUser.Setup(x => x.IsAdmin)
        .Returns(true);

        currentUser.Setup(x => x.IsCustomer)
            .Returns(false);

        var controller = new VehiclesController(
            vehicleService,
            currentUser.Object);

        //act
        var result = await controller.DeleteVehicle(1);

        //assert
        Assert.IsType<NoContentResult>(result);

        var deletedVehicle = await context.Vehicles.FindAsync(1);

        Assert.Null(deletedVehicle);
    }

    [Fact]
    public async Task DeleteVehicle_WithNonExistingVehicle_ReturnsNotFound()
    {
        //arrange
        using var context = CreateDbContext();

        var vehicleService = new VehicleService(context);
        var currentUser = new Mock<ICurrentUserService>();

        var controller = new VehiclesController(
            vehicleService,
            currentUser.Object);

        //act
        var result = await controller.DeleteVehicle(1);

        //assert
        Assert.IsType<NotFoundResult>(result);
    }
}