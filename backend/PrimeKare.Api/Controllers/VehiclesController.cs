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
            return BadRequest(new
            {
                message =
                    "Customer account is not properly configured."
            });
        }

        var vehicles = await _vehicleService
            .GetVehiclesAsync(customerId);

        return vehicles;
    }

    [Authorize(
    Roles = "Admin,Receptionist,Mechanic,Customer")]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<VehicleDto>>
    GetVehicle(int id)
    {
        var customerId =
            _currentUser.IsCustomer
                ? _currentUser.CustomerId
                : null;

        if (_currentUser.IsCustomer &&
            customerId == null)
        {
            return BadRequest(new
            {
                message =
                    "Customer account is not properly configured."
            });
        }

        try
        {
            var vehicle =
                await _vehicleService
                    .GetVehicleAsync(
                        id,
                        customerId);

            return Ok(vehicle);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                message = ex.Message
            });
        }
    }

    [Authorize(Roles = "Customer")]
    [HttpPost]
    public async Task<ActionResult<VehicleDto>>
    CreateVehicle(CreateVehicleDto dto)
    {
        var customerId =
            _currentUser.CustomerId;

        if (!customerId.HasValue)
        {
            return BadRequest(new
            {
                message =
                    "Customer account is not properly configured."
            });
        }

        try
        {
            var vehicle =
                await _vehicleService
                    .CreateVehicleAsync(
                        dto,
                        customerId.Value);

            return CreatedAtAction(
                nameof(GetVehicle),
                new { id = vehicle.Id },
                vehicle);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
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

    [Authorize(Roles = "Admin,Receptionist,Customer")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult>
    UpdateVehicle(
        int id,
        UpdateVehicleDto dto)
    {
        var customerId =
            _currentUser.IsCustomer
                ? _currentUser.CustomerId
                : null;

        if (_currentUser.IsCustomer &&
            customerId == null)
        {
            return BadRequest(new
            {
                message =
                    "Customer account is not properly configured."
            });
        }

        try
        {
            await _vehicleService
                .UpdateVehicleAsync(
                    id,
                    dto,
                    customerId,
                    _currentUser.IsAdmin);

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
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
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
    [HttpDelete("{id:int}")]
    public async Task<IActionResult>
    DeleteVehicle(int id)
    {
        var customerId =
            _currentUser.IsCustomer
                ? _currentUser.CustomerId
                : null;

        if (_currentUser.IsCustomer &&
            customerId == null)
        {
            return BadRequest(new
            {
                message =
                    "Customer account is not properly configured."
            });
        }

        try
        {
            await _vehicleService
                .DeleteVehicleAsync(
                    id,
                    customerId,
                    _currentUser.IsAdmin);

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
    }

    [Authorize(Roles = "Admin,Receptionist")]
    [HttpPost("admin")]
    public async Task<ActionResult<VehicleDto>>
    AdminCreateVehicle(
        AdminCreateVehicleDto dto)
    {
        try
        {
            var vehicle =
                await _vehicleService
                    .AdminCreateVehicleAsync(dto);

            return CreatedAtAction(
                nameof(GetVehicle),
                new { id = vehicle.Id },
                vehicle);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
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
}