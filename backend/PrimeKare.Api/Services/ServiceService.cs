using Microsoft.EntityFrameworkCore;
using PrimeKare.Api.Data;
using PrimeKare.Api.DTOs.Services;
using PrimeKare.Api.Models;

namespace PrimeKare.Api.Services;

public class ServiceService : IServiceService
{
    private readonly AppDbContext _context;
    private readonly IAzureBlobStorageService _azureBlobStorageService;

    public ServiceService(
        AppDbContext context,
        IAzureBlobStorageService azureBlobStorageService)
    {
        _context = context;
        _azureBlobStorageService = azureBlobStorageService;
    }

    public async Task<List<ServiceDto>> GetServicesAsync()
    {
        return await _context.Services
            .Where(service => service.IsActive && !service.IsDeleted)
            .Select(service => new ServiceDto
            {
                Id = service.Id,
                Name = service.Name,
                Slug = service.Slug,
                Description = service.Description,
                Price = service.Price,
                EstimatedMinutes = service.EstimatedMinutes,
                IsActive = service.IsActive,
                ImageUrl = service.ImageUrl,
                CreatedAt = service.CreatedAt,
                UpdatedAt = service.UpdatedAt
            })
            .ToListAsync();
    }

    public async Task<ServiceDto> GetServiceAsync(int id)
    {
        var service = await _context.Services
            .Where(service => service.Id == id && service.IsActive && !service.IsDeleted)
            .Select(service => new ServiceDto
            {
                Id = service.Id,
                Name = service.Name,
                Slug = service.Slug,
                Description = service.Description,
                Price = service.Price,
                EstimatedMinutes = service.EstimatedMinutes,
                IsActive = service.IsActive,
                ImageUrl = service.ImageUrl,
                CreatedAt = service.CreatedAt,
                UpdatedAt = service.UpdatedAt
            })
            .FirstOrDefaultAsync();

        if (service == null)
        {
            throw new KeyNotFoundException(
                "Service not found."
            );
        }

        return service;
    }

    public async Task<ServiceDto> GetServiceBySlugAsync(string slug)
    {
        var service = await _context.Services
            .Where(service => service.Slug == slug && service.IsActive && !service.IsDeleted)
            .Select(service => new ServiceDto
            {
                Id = service.Id,
                Name = service.Name,
                Slug = service.Slug,
                Description = service.Description,
                Price = service.Price,
                EstimatedMinutes = service.EstimatedMinutes,
                IsActive = service.IsActive,
                ImageUrl = service.ImageUrl,
                CreatedAt = service.CreatedAt,
                UpdatedAt = service.UpdatedAt
            })
            .FirstOrDefaultAsync();

        if (service == null)
        {
            throw new KeyNotFoundException(
                "Service not found."
            );
        }

        return service;
    }

    public async Task<ServiceDto> CreateServiceAsync(
        CreateServiceDto dto)
    {

        string? imageUrl = null;

        if (dto.Image != null)
        {
            await using var stream =
                dto.Image.OpenReadStream();

            imageUrl = await _azureBlobStorageService
                .UploadImageAsync(
                    stream,
                    dto.Image.FileName,
                    dto.Image.ContentType);
        }

        var service = new Service
        {
            Name = dto.Name,
            Slug = dto.Slug,
            Description = dto.Description,
            Price = dto.Price,
            EstimatedMinutes = dto.EstimatedMinutes,
            IsActive = dto.IsActive,
            ImageUrl = imageUrl,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Services.Add(service);

        await _context.SaveChangesAsync();

        return new ServiceDto
        {
            Id = service.Id,
            Name = service.Name,
            Slug = service.Slug,
            Description = service.Description,
            Price = service.Price,
            EstimatedMinutes = service.EstimatedMinutes,
            IsActive = service.IsActive,
            ImageUrl = service.ImageUrl,
            CreatedAt = service.CreatedAt,
            UpdatedAt = service.UpdatedAt
        };
    }

    public async Task UpdateServiceAsync(
        int id,
        UpdateServiceDto dto)
    {
        var service = await _context.Services
            .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);

        if (service == null)
        {
            throw new KeyNotFoundException(
                "Service not found."
            );
        }

        service.Name = dto.Name;
        service.Slug = dto.Slug;
        service.Description = dto.Description;
        service.Price = dto.Price;
        service.EstimatedMinutes = dto.EstimatedMinutes;
        service.IsActive = dto.IsActive;
        service.UpdatedAt = DateTime.UtcNow;

        if (dto.Image != null)
        {
            if (!string.IsNullOrWhiteSpace(service.ImageUrl))
            {
                await _azureBlobStorageService
                    .DeleteImageAsync(service.ImageUrl);
            }

            await using var stream =
                dto.Image.OpenReadStream();

            service.ImageUrl =
                await _azureBlobStorageService.UploadImageAsync(
                    stream,
                    dto.Image.FileName,
                    dto.Image.ContentType);
        }

        await _context.SaveChangesAsync();
    }

    public async Task DeleteServiceAsync(int id)
    {
        var service = await _context.Services
            .FirstOrDefaultAsync(s => s.Id == id);

        if (service == null)
        {
            throw new KeyNotFoundException(
                "Service not found."
            );
        }

        service.IsDeleted = true;
        service.IsActive = false;
        service.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }
}