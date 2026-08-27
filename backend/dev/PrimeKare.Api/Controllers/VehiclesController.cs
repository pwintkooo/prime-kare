using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PrimeKare.Api.DTOs.Vehicles;
using PrimeKare.Api.Services;

namespace PrimeKare.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class VehiclesController : ControllerBase
{
    private readonly IVehicleService _vehicleService;
    private readonly ICurrentUserService _currentUser;

    public VehiclesController(
        IVehicleService vehicleService,
        ICurrentUserService currentUser)
    {
        _vehicleService = vehicleService;
        _currentUser = currentUser;
    }

    [Authorize(Roles = "Admin,Receptionist,Mechanic,Customer")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<VehicleDto>>> GetVehicles()
    {
        var customerId = _currentUser.IsCustomer
            ? _currentUser.CustomerId
            : null;

        if (_currentUser.IsCustomer && customerId == null)
        {
            return NotFound();
        }

        var vehicles = await _vehicleService
            .GetVehiclesAsync(customerId);

        return vehicles;
    }

    [Authorize(Roles = "Admin,Receptionist,Mechanic,Customer")]
    [HttpGet("{id}")]
    public async Task<ActionResult<VehicleDto>> GetVehicle(int id)
    {
        var customerId = _currentUser.IsCustomer
            ? _currentUser.CustomerId
            : null;

        if (_currentUser.IsCustomer && customerId == null)
        {
            return NotFound();
        }

        var vehicle = await _vehicleService
            .GetVehicleAsync(id, customerId);

        if (vehicle == null)
        {
            return NotFound();
        }

        return vehicle;
    }

    [Authorize(Roles = "Admin,Receptionist,Customer")]
    [HttpPost]
    public async Task<ActionResult<VehicleDto>> CreateVehicle(
    CreateVehicleDto dto)
    {
        var vehicle = await _vehicleService
            .CreateVehicleAsync(dto);

        if (vehicle == null)
        {
            return BadRequest("Customer does not exist!");
        }

        return CreatedAtAction(
            nameof(GetVehicle),
            new { id = vehicle.Id },
            vehicle);
    }

    [Authorize(Roles = "Admin,Receptionist,Customer")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateVehicle(
    int id,
    UpdateVehicleDto dto)
    {
        var customerId = _currentUser.IsCustomer
            ? _currentUser.CustomerId
            : null;

        if (_currentUser.IsCustomer && customerId == null)
        {
            return NotFound();
        }

        try
        {
            var updated = await _vehicleService
                .UpdateVehicleAsync(
                    id,
                    dto,
                    customerId,
                    _currentUser.IsAdmin);

            if (!updated)
            {
                return BadRequest(
                    "Vehicle could not be updated.");
            }

            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [Authorize(Roles = "Admin,Customer")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteVehicle(int id)
    {
        var customerId = _currentUser.IsCustomer
            ? _currentUser.CustomerId
            : null;

        if (_currentUser.IsCustomer && customerId == null)
        {
            return NotFound();
        }

        var deleted = await _vehicleService
            .DeleteVehicleAsync(
                id,
                customerId,
                _currentUser.IsAdmin);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    // [Authorize(Roles = "Admin")]
    // [HttpGet("admin-test")]
    // public IActionResult AdminTest()
    // {
    //     return Ok(new
    //     {
    //         message = "You are an Admin."
    //     });
    // }
}