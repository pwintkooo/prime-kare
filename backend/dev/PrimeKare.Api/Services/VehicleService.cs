using Microsoft.EntityFrameworkCore;
using PrimeKare.Api.Data;
using PrimeKare.Api.DTOs.Vehicles;
using PrimeKare.Api.Models;

namespace PrimeKare.Api.Services;

public class VehicleService : IVehicleService
{
    private readonly AppDbContext _context;

    public VehicleService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<VehicleDto>> GetVehiclesAsync(
        int? customerId = null)
    {
        var query = _context.Vehicles
            .AsQueryable();

        if (customerId.HasValue)
        {
            query = query.Where(v =>
                v.CustomerId == customerId.Value &&
                v.Status == "active");
        }

        return await query
            .Select(v => new VehicleDto
            {
                Id = v.Id,
                PlateNumber = v.PlateNumber,
                Make = v.Make,
                Model = v.Model,
                Year = v.Year,
                Status = v.Status,
                CustomerId = v.CustomerId,
                CreatedAt = v.CreatedAt,
                UpdatedAt = v.UpdatedAt
            })
            .ToListAsync();
    }

    public async Task<VehicleDto?> GetVehicleAsync(
        int id,
        int? customerId = null)
    {
        var query = _context.Vehicles
            .Where(v => v.Id == id);

        if (customerId.HasValue)
        {
            query = query.Where(v =>
                v.CustomerId == customerId.Value &&
                v.Status == "active");
        }

        return await query
            .Select(v => new VehicleDto
            {
                Id = v.Id,
                PlateNumber = v.PlateNumber,
                Make = v.Make,
                Model = v.Model,
                Year = v.Year,
                Status = v.Status,
                CustomerId = v.CustomerId,
                CreatedAt = v.CreatedAt,
                UpdatedAt = v.UpdatedAt
            })
            .FirstOrDefaultAsync();
    }

    public async Task<VehicleDto?> CreateVehicleAsync(
        CreateVehicleDto dto)
    {
        var customerExists = await _context.Customers
            .AnyAsync(c => c.Id == dto.CustomerId);

        if (!customerExists)
        {
            return null;
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

        return new VehicleDto
        {
            Id = vehicle.Id,
            PlateNumber = vehicle.PlateNumber,
            Make = vehicle.Make,
            Model = vehicle.Model,
            Year = vehicle.Year,
            Status = vehicle.Status,
            CustomerId = vehicle.CustomerId
        };
    }

    public async Task<bool> UpdateVehicleAsync(
        int id,
        UpdateVehicleDto dto,
        int? customerId = null,
        bool isAdmin = false)
    {
        var vehicle = await _context.Vehicles
            .FirstOrDefaultAsync(v => v.Id == id);

        if (vehicle == null)
        {
            throw new KeyNotFoundException("Vehicle not found.");
        }

        if (customerId.HasValue &&
            vehicle.CustomerId != customerId.Value)
        {
            return false;
        }

        if (vehicle.Status == "archived" && !isAdmin)
        {
            return false;
        }

        var customerExists = await _context.Customers
            .AnyAsync(c => c.Id == dto.CustomerId);

        if (!customerExists)
        {
            return false;
        }

        vehicle.PlateNumber = dto.PlateNumber;
        vehicle.Make = dto.Make;
        vehicle.Model = dto.Model;
        vehicle.Year = dto.Year;
        vehicle.CustomerId = dto.CustomerId;
        vehicle.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteVehicleAsync(
        int id,
        int? customerId = null,
        bool isAdmin = false)
    {
        var vehicle = await _context.Vehicles
            .FirstOrDefaultAsync(v => v.Id == id);

        if (vehicle == null)
        {
            return false;
        }

        if (customerId.HasValue &&
            vehicle.CustomerId != customerId.Value)
        {
            return false;
        }

        if (isAdmin)
        {
            _context.Vehicles.Remove(vehicle);
        }
        else
        {
            vehicle.Status = "archived";
            vehicle.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        return true;
    }
}