using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PrimeKare.Api.DTOs.Vehicles;
using PrimeKare.Api.Services;
using PrimeKare.Api.Services.Exceptions;

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
            return NotFound(new
            {
                message = "Customer account not found."
            });
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
            return NotFound(new
            {
                message = "Customer account not found."
            });
        }

        var vehicle = await _vehicleService
            .GetVehicleAsync(id, customerId);

        if (vehicle == null)
        {
            return NotFound(new
            {
                message = "Vehicle not found."
            });
        }

        return vehicle;
    }

    [Authorize(Roles = "Customer")]
    [HttpPost]
    public async Task<ActionResult<VehicleDto>> CreateVehicle(
        CreateVehicleDto dto)
    {
        var customerId = _currentUser.CustomerId;

        if (customerId == null)
        {
            return Unauthorized(new
            {
                message =
                    "Customer account is not associated with a customer."
            });
        }

        try
        {
            var vehicle = await _vehicleService
                .CreateVehicleAsync(
                    dto,
                    customerId.Value);

            if (vehicle == null)
            {
                return BadRequest(new
                {
                    message = "Customer does not exist."
                });
            }

            return CreatedAtAction(
                nameof(GetVehicle),
                new { id = vehicle.Id },
                vehicle);
        }
        catch (ConflictException ex)
        {
            return Conflict(new
            {
                message = ex.Message
            });
        }
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
            return NotFound(new
            {
                message = "Customer account not found."
            });
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
                return BadRequest(new
                {
                    message = "Vehicle could not be updated."
                });
            }

            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                message = ex.Message
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new
            {
                message = ex.Message
            });
        }
        catch (ConflictException ex)
        {
            return Conflict(new
            {
                message = ex.Message
            });
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
            return NotFound(new
            {
                message = "Customer account not found."
            });
        }

        var deleted = await _vehicleService
            .DeleteVehicleAsync(
                id,
                customerId,
                _currentUser.IsAdmin);

        if (!deleted)
        {
            return NotFound(new
            {
                message = "Vehicle not found."
            });
        }

        return NoContent();
    }

    [Authorize(Roles = "Admin,Receptionist")]
    [HttpPost("admin")]
    public async Task<ActionResult<VehicleDto>> AdminCreateVehicle(
        AdminCreateVehicleDto dto)
    {
        try
        {
            var vehicle = await _vehicleService
                .AdminCreateVehicleAsync(dto);

            if (vehicle == null)
            {
                return BadRequest(new
                {
                    message = "Customer does not exist."
                });
            }

            return CreatedAtAction(
                nameof(GetVehicle),
                new { id = vehicle.Id },
                vehicle);
        }
        catch (ConflictException ex)
        {
            return Conflict(new
            {
                message = ex.Message
            });
        }
    }
}