using Microsoft.EntityFrameworkCore;
using PrimeKare.Api.Data;
using PrimeKare.Api.DTOs.Services;
using PrimeKare.Api.Models;

namespace PrimeKare.Api.Services;

public class ServiceService : IServiceService
{
    private readonly AppDbContext _context;
    private readonly IFirebaseStorageService _firebaseStorageService;

    public ServiceService(
        AppDbContext context,
        IFirebaseStorageService firebaseStorageService)
    {
        _context = context;
        _firebaseStorageService = firebaseStorageService;
    }

    public async Task<List<ServiceDto>> GetServicesAsync()
    {
        return await _context.Services
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

    public async Task<ServiceDto?> GetServiceAsync(int id)
    {
        return await _context.Services
            .Where(service => service.Id == id)
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
    }

    public async Task<ServiceDto> CreateServiceAsync(
        CreateServiceDto dto)
    {
        string? imageUrl = null;

        if (dto.Image != null)
        {
            await using var stream =
                dto.Image.OpenReadStream();

            imageUrl = await _firebaseStorageService
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

    public async Task<bool> UpdateServiceAsync(
        int id,
        UpdateServiceDto dto)
    {
        var service = await _context.Services
            .FirstOrDefaultAsync(s => s.Id == id);

        if (service == null)
        {
            return false;
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
                await _firebaseStorageService
                    .DeleteImageAsync(service.ImageUrl);
            }

            await using var stream =
                dto.Image.OpenReadStream();

            service.ImageUrl =
                await _firebaseStorageService.UploadImageAsync(
                    stream,
                    dto.Image.FileName,
                    dto.Image.ContentType);
        }

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteServiceAsync(int id)
    {
        var service = await _context.Services
            .FirstOrDefaultAsync(s => s.Id == id);

        if (service == null)
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(service.ImageUrl))
        {
            await _firebaseStorageService
                .DeleteImageAsync(service.ImageUrl);
        }

        _context.Services.Remove(service);

        await _context.SaveChangesAsync();

        return true;
    }
}