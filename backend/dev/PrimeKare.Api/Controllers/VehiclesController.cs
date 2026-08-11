using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrimeKare.Api.Data;
using PrimeKare.Api.Models;
using PrimeKare.Api.DTOs.Vehicles;

namespace PrimeKare.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VehiclesController : ControllerBase
{
    private readonly AppDbContext _context;

    public VehiclesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<VehicleDto>>> GetVehicles()
    {
        var vehicles = await _context.Vehicles
        .Select(vehicle => new VehicleDto
        {
            Id = vehicle.Id,
            PlateNumber = vehicle.PlateNumber,
            Make = vehicle.Make,
            Model = vehicle.Model,
            Year = vehicle.Year,
            CustomerId = vehicle.CustomerId
        })
        .ToListAsync();

        return vehicles;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<VehicleDto>> GetVehicle(int id)
    {
        var vehicle = await _context.Vehicles.FindAsync(id);

        if (vehicle == null)
        {
            return NotFound();
        }

        var vehicleDto = new VehicleDto
        {
            Id = vehicle.Id,
            PlateNumber = vehicle.PlateNumber,
            Make = vehicle.Make,
            Model = vehicle.Model,
            Year = vehicle.Year,
            CustomerId = vehicle.CustomerId
        };

        return vehicleDto;
    }

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
            CustomerId = dto.CustomerId
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
            CustomerId = vehicle.CustomerId
        };

        return CreatedAtAction(
            nameof(GetVehicle),
            new { id = vehicle.Id },
            vehicleDto
        );
    }

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

        await _context.SaveChangesAsync();

        return NoContent();
    }

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
}