using Microsoft.EntityFrameworkCore;
using PrimeKare.Api.Data;
using PrimeKare.Api.DTOs.Vehicles;
using PrimeKare.Api.Models;
using PrimeKare.Api.Services.Exceptions;

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
                v.Status == "active" &&
                !v.IsDeleted);
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
                IsDeleted = v.IsDeleted,
                CustomerId = v.CustomerId,
                CreatedAt = v.CreatedAt,
                UpdatedAt = v.UpdatedAt
            })
            .ToListAsync();
    }

    public async Task<VehicleDto> GetVehicleAsync(
    int id,
    int? customerId = null)
    {
        var query = _context.Vehicles
            .Where(v => v.Id == id);

        if (customerId.HasValue)
        {
            query = query.Where(v =>
                v.CustomerId == customerId.Value &&
                v.Status == "active" &&
                !v.IsDeleted);
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
                IsDeleted = v.IsDeleted,
                CustomerId = v.CustomerId,
                CreatedAt = v.CreatedAt,
                UpdatedAt = v.UpdatedAt
            })
            .FirstOrDefaultAsync();

        if (vehicle == null)
        {
            throw new KeyNotFoundException(
                "Vehicle not found."
            );
        }

        return vehicle;
    }

    public async Task<VehicleDto> CreateVehicleAsync(
        CreateVehicleDto dto,
        int customerId)
    {
        var customerExists = await _context.Customers
            .AnyAsync(c => c.Id == customerId);

        if (!customerExists)
        {
            throw new KeyNotFoundException(
                "Customer not found."
            );
        }

        var plateNumber = dto.PlateNumber
            .Trim()
            .ToUpperInvariant();

        var exists = await _context.Vehicles
            .AnyAsync(v =>
                v.PlateNumber == plateNumber &&
                !v.IsDeleted &&
                v.Status == "active");

        if (exists)
        {
            throw new ConflictException(
                "A vehicle with this plate number already exists.");
        }

        var vehicle = new Vehicle
        {
            PlateNumber = plateNumber,
            Make = dto.Make.Trim().ToUpperInvariant(),
            Model = dto.Model.Trim().ToUpperInvariant(),
            Year = dto.Year,
            CustomerId = customerId,
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
            IsDeleted = vehicle.IsDeleted,
            CustomerId = vehicle.CustomerId,
            CreatedAt = vehicle.CreatedAt,
            UpdatedAt = vehicle.UpdatedAt
        };
    }

    public async Task UpdateVehicleAsync(
    int id,
    UpdateVehicleDto dto,
    int? customerId = null,
    bool isAdmin = false)
    {
        var vehicle = await _context.Vehicles
            .FirstOrDefaultAsync(v =>
                v.Id == id &&
                !v.IsDeleted);

        if (vehicle == null)
        {
            throw new KeyNotFoundException(
                "Vehicle not found.");
        }

        if (customerId.HasValue &&
            vehicle.CustomerId != customerId.Value)
        {
            throw new UnauthorizedAccessException(
                "You are not allowed to update this vehicle.");
        }

        if (vehicle.Status == "archived" && !isAdmin)
        {
            throw new InvalidOperationException(
                "Archived vehicles cannot be updated.");
        }

        var plateNumber = dto.PlateNumber
            .Trim()
            .ToUpperInvariant();

        var exists = await _context.Vehicles
            .AnyAsync(v =>
                v.PlateNumber == plateNumber &&
                v.Id != id &&
                !v.IsDeleted &&
                v.Status == "active");

        if (exists)
        {
            throw new ConflictException(
                "A vehicle with this plate number already exists.");
        }

        vehicle.PlateNumber = plateNumber;
        vehicle.Make = dto.Make
            .Trim()
            .ToUpperInvariant();
        vehicle.Model = dto.Model
            .Trim()
            .ToUpperInvariant();
        vehicle.Year = dto.Year;
        vehicle.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteVehicleAsync(
        int id,
        int? customerId = null,
        bool isAdmin = false)
    {
        var vehicle = await _context.Vehicles
            .FirstOrDefaultAsync(v =>
                v.Id == id &&
                !v.IsDeleted);

        if (vehicle == null)
        {
            throw new KeyNotFoundException(
                "Vehicle not found."
            );
        }

        if (customerId.HasValue &&
        vehicle.CustomerId != customerId.Value)
        {
            throw new UnauthorizedAccessException(
                "You are not allowed to delete this vehicle."
            );
        }

        if (isAdmin)
        {
            vehicle.IsDeleted = true;
            vehicle.Status = "archived";
        }
        else
        {
            vehicle.Status = "archived";
        }

        vehicle.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    public async Task<VehicleDto> AdminCreateVehicleAsync(
    AdminCreateVehicleDto dto)
    {
        var customerExists = await _context.Customers
            .AnyAsync(c => c.Id == dto.CustomerId);

        if (!customerExists)
        {
            throw new KeyNotFoundException(
                "Customer not found."
            );
        }

        var plateNumber = dto.PlateNumber
            .Trim()
            .ToUpperInvariant();

        var exists = await _context.Vehicles
            .AnyAsync(v =>
                v.PlateNumber == plateNumber &&
                !v.IsDeleted &&
                v.Status == "active");

        if (exists)
        {
            throw new ConflictException(
                "A vehicle with this plate number already exists.");
        }

        var vehicle = new Vehicle
        {
            PlateNumber = plateNumber,
            Make = dto.Make
                .Trim()
                .ToUpperInvariant(),
            Model = dto.Model
                .Trim()
                .ToUpperInvariant(),
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
}