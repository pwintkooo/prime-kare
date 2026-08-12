using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using PrimeKare.Api.DTOs.Vehicles;
using Microsoft.Extensions.DependencyInjection;
using PrimeKare.Api.Data;
using PrimeKare.Api.Models;

namespace PrimeKare.Api.Tests.Integration;

public class VehiclesApiTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public VehiclesApiTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetVehicles_ReturnsSuccessStatusCode()
    {
        // Act
        var response = await _client.GetAsync("/api/vehicles");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // var content = await response.Content.ReadAsStringAsync();

        // Assert.True(
        //     response.IsSuccessStatusCode,
        //     $"Status: {response.StatusCode}\nResponse: {content}"
        // );
    }

    [Fact]
    public async Task GetVehicles_ReturnsJSONResponse()
    {
        // Act
        var response = await _client.GetAsync("/api/vehicles");

        // Assert
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();

        Assert.NotNull(content);
    }

    [Fact]
    public async Task GetVehicle_NonExistingId_ReturnsNotFound()
    {
        // using var scope = _factory.Services.CreateScope();

        // var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // context.Database.EnsureDeleted();
        // context.Database.EnsureCreated();

        await TestDataHelper.CreateCleanDatabase(_factory);

        var response = await _client.GetAsync(
            "api/vehicles/1"
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateVehicle_ReturnsCreated()
    {
        // using var scope = _factory.Services.CreateScope();

        // var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // context.Database.EnsureDeleted();
        // context.Database.EnsureCreated();

        // context.Customers.Add(new Customer
        // {
        //     Id = 1,
        //     Name = "PK",
        //     Phone = "91234567",
        //     Email = "pk@example.com"
        // });

        //await context.SaveChangesAsync();

        //use testdatahelper
        var context = await TestDataHelper.CreateCleanDatabase(_factory);

        await TestDataHelper.AddCustomer(context);

        var request = new CreateVehicleDto
        {
            PlateNumber = "SLA1234A",
            Make = "Toyota",
            Model = "Camry",
            Year = 2024,
            CustomerId = 1
        };

        var response = await _client.PostAsJsonAsync(
            "api/vehicles",
            request
        );

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var createdVehicle = await response.Content
        .ReadFromJsonAsync<VehicleDto>();

        Assert.NotNull(createdVehicle);

        Assert.Equal("SLA1234A", createdVehicle.PlateNumber);
        Assert.Equal("Toyota", createdVehicle.Make);
        Assert.Equal("Camry", createdVehicle.Model);
        Assert.Equal(2024, createdVehicle.Year);
        Assert.Equal(1, createdVehicle.CustomerId);
    }

    [Fact]
    public async Task CreateVehicleWithInvalidCustomer_ReturnsBadRequest()
    {
        await TestDataHelper.CreateCleanDatabase(_factory);

        var request = new CreateVehicleDto
        {
            PlateNumber = "SLA1234A",
            Make = "Toyota",
            Model = "Camry",
            Year = 2024,
            CustomerId = 1
        };

        var response = await _client.PostAsJsonAsync(
            "api/vehicles",
            request
        );

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateVehicle_GetVehicle_ReturnsVehicle()
    {
        var context = await TestDataHelper.CreateCleanDatabase(_factory);

        await TestDataHelper.AddCustomer(context);

        var request = new CreateVehicleDto
        {
            PlateNumber = "SLA1234A",
            Make = "Toyota",
            Model = "Camry",
            Year = 2024,
            CustomerId = 1
        };

        //Act1
        var createResponse = await _client.PostAsJsonAsync(
            "api/vehicles",
            request
        );

        Assert.Equal(
        HttpStatusCode.Created,
        createResponse.StatusCode
        );

        var createdVehicle = await createResponse.Content
            .ReadFromJsonAsync<VehicleDto>();

        Assert.NotNull(createdVehicle);

        //Act2
        var getResponse = await _client.GetAsync($"/api/vehicles/{createdVehicle.Id}");

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var vehicle = await getResponse.Content
        .ReadFromJsonAsync<VehicleDto>();

        Assert.NotNull(vehicle);

        Assert.Equal(createdVehicle.Id, vehicle.Id);
        Assert.Equal("SLA1234A", vehicle.PlateNumber);
        Assert.Equal("Toyota", vehicle.Make);
        Assert.Equal("Camry", vehicle.Model);
        Assert.Equal(2024, vehicle.Year);
        Assert.Equal(1, vehicle.CustomerId);
    }

    [Fact]
    public async Task CreateVehicle_ThenUpdateVehicle_ThenGetVehicle_ReturnsVehicle()
    {
        var context = await TestDataHelper.CreateCleanDatabase(_factory);

        await TestDataHelper.AddCustomer(context);

        //create vehicle
        var request = new CreateVehicleDto
        {
            PlateNumber = "SLA1234A",
            Make = "Toyota",
            Model = "Camry",
            Year = 2024,
            CustomerId = 1
        };

        var createResponse = await _client.PostAsJsonAsync(
            "api/vehicles",
            request
        );

        Assert.Equal(
        HttpStatusCode.Created,
        createResponse.StatusCode
        );

        var createdVehicle = await createResponse.Content
            .ReadFromJsonAsync<VehicleDto>();

        Assert.NotNull(createdVehicle);

        //update vehicle
        var updateRequest = new UpdateVehicleDto
        {
            PlateNumber = "SLB5678B",
            Make = "Honda",
            Model = "Civic",
            Year = 2025,
            CustomerId = 1
        };

        var updateResponse = await _client.PutAsJsonAsync(
            $"/api/vehicles/{createdVehicle.Id}",
            updateRequest
        );

        Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);

        //get vehicle
        var getResponse = await _client.GetAsync($"/api/vehicles/{createdVehicle.Id}");

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var getVehicle = await getResponse.Content
        .ReadFromJsonAsync<VehicleDto>();

        Assert.NotNull(getVehicle);

        Assert.Equal(createdVehicle.Id, getVehicle.Id);
        Assert.Equal("SLB5678B", getVehicle.PlateNumber);
        Assert.Equal("Honda", getVehicle.Make);
        Assert.Equal("Civic", getVehicle.Model);
        Assert.Equal(2025, getVehicle.Year);
        Assert.Equal(1, getVehicle.CustomerId);
    }

    [Fact]
    public async Task UpdateVehicle_WithNonExistingId_ReturnsNotFound()
    {
        await TestDataHelper.CreateCleanDatabase(_factory);

        //update vehicle
        var updateRequest = new UpdateVehicleDto
        {
            PlateNumber = "SLB5678B",
            Make = "Honda",
            Model = "Civic",
            Year = 2025,
            CustomerId = 1
        };

        var updateResponse = await _client.PutAsJsonAsync(
            $"/api/vehicles/1",
            updateRequest
        );

        Assert.Equal(HttpStatusCode.NotFound, updateResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteVehicle_ThenGetVehicle_ReturnsNotFound()
    {
        var context = await TestDataHelper.CreateCleanDatabase(_factory);

        await TestDataHelper.AddCustomer(context);

        var request = new CreateVehicleDto
        {
            PlateNumber = "SLA1234A",
            Make = "Toyota",
            Model = "Camry",
            Year = 2024,
            CustomerId = 1
        };

        var createResponse = await _client.PostAsJsonAsync(
            "api/vehicles",
            request
        );

        Assert.Equal(
        HttpStatusCode.Created,
        createResponse.StatusCode
        );

        var createdVehicle = await createResponse.Content
            .ReadFromJsonAsync<VehicleDto>();

        Assert.NotNull(createdVehicle);

        var deleteResponse = await _client.DeleteAsync($"/api/vehicles/{createdVehicle.Id}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getResponse = await _client.GetAsync($"/api/vehicles/{createdVehicle.Id}");

        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task CreateVehicle_WithEmptyPlateNumber_ReturnsBadRequest()
    {
        var context = await TestDataHelper.CreateCleanDatabase(_factory);

        await TestDataHelper.AddCustomer(context);

        var request = new CreateVehicleDto
        {
            PlateNumber = "",
            Make = "Toyota",
            Model = "Camry",
            Year = 2024,
            CustomerId = 1
        };

        var createResponse = await _client.PostAsJsonAsync(
            "api/vehicles",
            request
        );

        Assert.Equal(
        HttpStatusCode.BadRequest,
        createResponse.StatusCode
        );
    }

    [Fact]
    public async Task CreateVehicle_ThenUpdateVehicle_WithEmptyMake_ReturnsBadRequest()
    {
        var context = await TestDataHelper.CreateCleanDatabase(_factory);

        await TestDataHelper.AddCustomer(context);

        //create vehicle
        var request = new CreateVehicleDto
        {
            PlateNumber = "SLA1234A",
            Make = "Toyota",
            Model = "Camry",
            Year = 2024,
            CustomerId = 1
        };

        var createResponse = await _client.PostAsJsonAsync(
            "api/vehicles",
            request
        );

        Assert.Equal(
        HttpStatusCode.Created,
        createResponse.StatusCode
        );

        var createdVehicle = await createResponse.Content
            .ReadFromJsonAsync<VehicleDto>();

        Assert.NotNull(createdVehicle);

        //update vehicle
        var updateRequest = new UpdateVehicleDto
        {
            PlateNumber = "SLB5678B",
            Make = "",
            Model = "Civic",
            Year = 2025,
            CustomerId = 1
        };

        var updateResponse = await _client.PutAsJsonAsync(
            $"/api/vehicles/{createdVehicle.Id}",
            updateRequest
        );

        Assert.Equal(HttpStatusCode.BadRequest, updateResponse.StatusCode);
    }
}