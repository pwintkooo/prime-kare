using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrimeKare.Api.Data;
using PrimeKare.Api.Models;
using PrimeKare.Api.DTOs.Services;

namespace PrimeKare.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServicesController : ControllerBase
{
    private readonly AppDbContext _context;

    public ServicesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ServiceDto>>> GetServices()
    {
        var services = await _context.Services
        .Select(service => new ServiceDto
        {
            Id = service.Id,
            Name = service.Name,
            Description = service.Description,
            Price = service.Price,
            EstimatedMinutes = service.EstimatedMinutes,
            IsActive = service.IsActive
        })
        .ToListAsync();

        return services;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ServiceDto>> GetService(int id)
    {
        var service = await _context.Services.FindAsync(id);

        if (service == null)
        {
            return NotFound();
        }

        var serviceDto = new ServiceDto
        {
            Id = service.Id,
            Name = service.Name,
            Description = service.Description,
            Price = service.Price,
            EstimatedMinutes = service.EstimatedMinutes,
            IsActive = service.IsActive
        };

        return serviceDto;
    }

    [HttpPost]
    public async Task<ActionResult<ServiceDto>> CreateService(CreateServiceDto dto)
    {
        var service = new Service
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            EstimatedMinutes = dto.EstimatedMinutes,
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Services.Add(service);

        await _context.SaveChangesAsync();

        var serviceDto = new ServiceDto
        {
            Id = service.Id,
            Name = service.Name,
            Description = service.Description,
            Price = service.Price,
            EstimatedMinutes = service.EstimatedMinutes,
            IsActive = service.IsActive
        };

        return CreatedAtAction(
            nameof(GetService),
            new { id = service.Id },
            serviceDto
        );
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateService(int id, UpdateServiceDto dto)
    {
        var existingService = await _context.Services.FindAsync(id);

        if (existingService == null)
        {
            return NotFound();
        }

        existingService.Name = dto.Name;
        existingService.Description = dto.Description;
        existingService.Price = dto.Price;
        existingService.EstimatedMinutes = dto.EstimatedMinutes;
        existingService.IsActive = dto.IsActive;
        existingService.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteService(int id)
    {
        var service = await _context.Services.FindAsync(id);

        if (service == null)
        {
            return NotFound();
        }

        _context.Services.Remove(service);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}