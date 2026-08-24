using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using PrimeKare.Api.Data;
using PrimeKare.Api.Models;
using PrimeKare.Api.DTOs.Vehicles;

namespace PrimeKare.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class VehiclesController : ControllerBase
{
    private readonly AppDbContext _context;

    public VehiclesController(AppDbContext context)
    {
        _context = context;
    }

    [Authorize(Roles = "Admin,Receptionist,Mechanic,Customer")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<VehicleDto>>> GetVehicles()
    {
        var query = _context.Vehicles.AsQueryable();

        if (User.IsInRole("Customer"))
        {
            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!int.TryParse(userId, out var parsedUserId))
            {
                return Unauthorized();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(
                    u => u.Id == int.Parse(userId));

            if (user == null || user.CustomerId == null)
            {
                return NotFound();
            }

            query = query.Where(
                vehicle => vehicle.CustomerId == user.CustomerId);
        }

        var vehicles = await query
            .Select(vehicle => new VehicleDto
            {
                Id = vehicle.Id,
                PlateNumber = vehicle.PlateNumber,
                Make = vehicle.Make,
                Model = vehicle.Model,
                Year = vehicle.Year,
                Status = vehicle.Status,
                CustomerId = vehicle.CustomerId
            })
            .ToListAsync();

        return vehicles;
    }

    [Authorize(Roles = "Admin,Receptionist,Mechanic,Customer")]
    [HttpGet("{id}")]
    public async Task<ActionResult<VehicleDto>> GetVehicle(int id)
    {
        var query = _context.Vehicles
            .Where(v => v.Id == id);

        if (User.IsInRole("Customer"))
        {
            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!int.TryParse(userId, out var parsedUserId))
            {
                return Unauthorized();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(
                    u => u.Id == parsedUserId);

            if (user == null || user.CustomerId == null)
            {
                return NotFound();
            }

            query = query.Where(
                v => v.CustomerId == user.CustomerId);
        }

        var vehicle = await query
            .Select(v => new VehicleDto
            {
                Id = v.Id,
                PlateNumber = v.PlateNumber,
                Make = v.Make,
                Model = v.Model,
                Year = v.Year,
                Status = v.Status,
                CustomerId = v.CustomerId
            })
            .FirstOrDefaultAsync();

        if (vehicle == null)
        {
            return NotFound();
        }

        return vehicle;
    }

    [Authorize(Roles = "Admin,Receptionist")]
    [HttpPost]
    public async Task<ActionResult<VehicleDto>> CreateVehicle(CreateVehicleDto dto)
    {
        var customerExists = await _context.Customers
        .AnyAsync(customer => customer.Id == dto.CustomerId);

        if (!customerExists)
        {
            return BadRequest("Customer does not exist!");
        }

        var vehicle = new Vehicle
        {
            PlateNumber = dto.PlateNumber,
            Make = dto.Make,
            Model = dto.Model,
            Year = dto.Year,
            CustomerId = dto.CustomerId,
            Status = "active",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Vehicles.Add(vehicle);

        await _context.SaveChangesAsync();

        var vehicleDto = new VehicleDto
        {
            Id = vehicle.Id,
            PlateNumber = vehicle.PlateNumber,
            Make = vehicle.Make,
            Model = vehicle.Model,
            Year = vehicle.Year,
            CustomerId = vehicle.CustomerId,
            Status = vehicle.Status
        };

        return CreatedAtAction(
            nameof(GetVehicle),
            new { id = vehicle.Id },
            vehicleDto
        );
    }

    [Authorize(Roles = "Admin,Receptionist")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateVehicle(int id, UpdateVehicleDto dto)
    {
        var existingVehicle = await _context.Vehicles.FindAsync(id);

        if (existingVehicle == null)
        {
            return NotFound();
        }

        var customerExists = await _context.Customers
        .AnyAsync(customer => customer.Id == dto.CustomerId);

        if (!customerExists)
        {
            return BadRequest("Customer does not exist!");
        }

        existingVehicle.PlateNumber = dto.PlateNumber;
        existingVehicle.Make = dto.Make;
        existingVehicle.Model = dto.Model;
        existingVehicle.Year = dto.Year;
        existingVehicle.CustomerId = dto.CustomerId;
        existingVehicle.Status = dto.Status;
        existingVehicle.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteVehicle(int id)
    {
        var vehicle = await _context.Vehicles.FindAsync(id);

        if (vehicle == null)
        {
            return NotFound();
        }

        _context.Vehicles.Remove(vehicle);

        await _context.SaveChangesAsync();

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