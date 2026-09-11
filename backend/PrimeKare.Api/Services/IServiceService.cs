using PrimeKare.Api.DTOs.Services;

namespace PrimeKare.Api.Services;

public interface IServiceService
{
    Task<List<ServiceDto>> GetServicesAsync();

    Task<ServiceDto?> GetServiceAsync(int id);

    Task<ServiceDto?> GetServiceBySlugAsync(string slug);

    Task<ServiceDto> CreateServiceAsync(CreateServiceDto dto);

    Task<bool> UpdateServiceAsync(int id, UpdateServiceDto dto);

    Task<bool> DeleteServiceAsync(int id);
}