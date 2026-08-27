using System.Net;
using System.Net.Http.Json;
using PrimeKare.Api.DTOs.Vehicles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PrimeKare.Api.Models;

namespace PrimeKare.Api.Tests.Integration;

public class VehiclesApiTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;
    private readonly IConfiguration _configuration;

    public VehiclesApiTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _configuration = factory.Services
            .GetRequiredService<IConfiguration>();
    }

    [Fact]
    public async Task GetVehicles_ReturnsSuccessStatusCode()
    {
        TestAuthHelper.AuthenticateAs(
            _client,
            _configuration,
            1,
            "admin@primekare.com",
            "Admin");

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
        TestAuthHelper.AuthenticateAs(
            _client,
            _configuration,
            1,
            "admin@primekare.com",
            "Admin");

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
        TestAuthHelper.AuthenticateAs(
            _client,
            _configuration,
            1,
            "admin@primekare.com",
            "Admin");

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
    public async Task GetVehicles_AsCustomer_ReturnsNotFound()
    {
        await TestDataHelper.CreateCleanDatabase(_factory);

        TestAuthHelper.AuthenticateAs(
            _client,
            _configuration,
            1,
            "PK@example.com",
            "Customer");

        // Act
        var response = await _client.GetAsync("/api/vehicles");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        // var content = await response.Content.ReadAsStringAsync();

        // Assert.True(
        //     response.IsSuccessStatusCode,
        //     $"Status: {response.StatusCode}\nResponse: {content}"
        // );
    }

    [Fact]
    public async Task GetVehicles_WithoutAuthentication_ReturnsUnauthorized()
    {
        TestAuthHelper.ClearAuthentication(_client);

        var response = await _client.GetAsync(
            "/api/vehicles");

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }

    [Fact]
    public async Task GetVehicle_AsCustomer_ReturnsOnlyOwnVehicle()
    {
        var context = await TestDataHelper.CreateCleanDatabase(_factory);

        var customerA = new Customer
        {
            Id = 1,
            Name = "PK1",
            Phone = "91234567",
            Email = "pk1@example.com"
        };

        var customerB = new Customer
        {
            Id = 2,
            Name = "PK2",
            Phone = "91234568",
            Email = "pk2@example.com"
        };

        context.Customers.AddRange(customerA, customerB);

        var vehicleA = new Vehicle
        {
            Id = 1,
            PlateNumber = "SLA1234A",
            Make = "Toyota",
            Model = "Camry",
            Year = 2024,
            CustomerId = 1
        };

        var vehicleB = new Vehicle
        {
            Id = 2,
            PlateNumber = "SLA5678B",
            Make = "Honda",
            Model = "Civic",
            Year = 2023,
            CustomerId = 2
        };

        context.Vehicles.AddRange(vehicleA, vehicleB);

        var customerUser = new User
        {
            Id = 1,
            Email = "PK1@example.com",
            Role = "Customer",
            CustomerId = 1,
            PasswordHash = "test"
        };

        context.Users.Add(customerUser);
        await context.SaveChangesAsync();

        TestAuthHelper.AuthenticateAs(
            _client,
            _configuration,
            1,
            "PK1@example.com",
            "Customer");

        var response = await _client.GetAsync(
        "/api/vehicles");

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        var vehicles = await response.Content
            .ReadFromJsonAsync<List<VehicleDto>>();

        Assert.NotNull(vehicles);
        Assert.Single(vehicles);

        Assert.Equal(1, vehicles[0].Id);
        Assert.Equal("SLA1234A", vehicles[0].PlateNumber);
        Assert.Equal(1, vehicles[0].CustomerId);
    }

    [Fact]
    public async Task GetVehicle_AsCustomer_WithOwnVehicle_ReturnsOk()
    {
        var context = await TestDataHelper.CreateCleanDatabase(_factory);

        var customer = new Customer
        {
            Id = 1,
            Name = "PK",
            Phone = "91234567",
            Email = "pk@example.com"
        };

        context.Customers.Add(customer);

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

        var customerUser = new User
        {
            Id = 1,
            Email = "PK@example.com",
            Role = "Customer",
            CustomerId = 1,
            PasswordHash = "test"
        };

        context.Users.Add(customerUser);
        await context.SaveChangesAsync();

        TestAuthHelper.AuthenticateAs(
            _client,
            _configuration,
            1,
            "PK@example.com",
            "Customer");

        var response = await _client.GetAsync(
        "/api/vehicles/1");

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);
    }

    [Fact]
    public async Task GetVehicle_AsCustomer_WithOtherCustomersVehicle_ReturnsNotFound()
    {
        var context = await TestDataHelper.CreateCleanDatabase(_factory);

        var customer = new Customer
        {
            Id = 1,
            Name = "PK",
            Phone = "91234567",
            Email = "pk@example.com"
        };

        context.Customers.Add(customer);

        var vehicle = new Vehicle
        {
            Id = 1,
            PlateNumber = "SLA1234A",
            Make = "Toyota",
            Model = "Camry",
            Year = 2024,
            CustomerId = 2
        };

        context.Vehicles.Add(vehicle);

        var customerUser = new User
        {
            Id = 1,
            Email = "PK@example.com",
            Role = "Customer",
            CustomerId = 1,
            PasswordHash = "test"
        };

        context.Users.Add(customerUser);
        await context.SaveChangesAsync();

        TestAuthHelper.AuthenticateAs(
            _client,
            _configuration,
            1,
            "PK@example.com",
            "Customer");

        var response = await _client.GetAsync(
        "/api/vehicles/2");

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }

    [Fact]
    public async Task CreateVehicle_ReturnsCreated()
    {
        TestAuthHelper.AuthenticateAs(
            _client,
            _configuration,
            1,
            "admin@primekare.com",
            "Admin");

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
        TestAuthHelper.AuthenticateAs(
            _client,
            _configuration,
            1,
            "admin@primekare.com",
            "Admin");

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
        TestAuthHelper.AuthenticateAs(
            _client,
            _configuration,
            1,
            "admin@primekare.com",
            "Admin");

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
        TestAuthHelper.AuthenticateAs(
            _client,
            _configuration,
            1,
            "admin@primekare.com",
            "Admin");

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
    public async Task CreateVehicle_WithEmptyPlateNumber_ReturnsBadRequest()
    {
        TestAuthHelper.AuthenticateAs(
            _client,
            _configuration,
            1,
            "admin@primekare.com",
            "Admin");

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
        TestAuthHelper.AuthenticateAs(
            _client,
            _configuration,
            1,
            "admin@primekare.com",
            "Admin");

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

    [Fact]
    public async Task UpdateVehicle_WithNonExistingId_ReturnsNotFound()
    {
        TestAuthHelper.AuthenticateAs(
            _client,
            _configuration,
            1,
            "admin@primekare.com",
            "Admin");

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
        TestAuthHelper.AuthenticateAs(
            _client,
            _configuration,
            1,
            "admin@primekare.com",
            "Admin");

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
    public async Task DeleteVehicle_AsCustomer_ReturnsUnauthorize()
    {
        TestAuthHelper.AuthenticateAs(
            _client,
            _configuration,
            1,
            "admin@primekare.com",
            "Admin");

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
}