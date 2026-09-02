using PrimeKare.Api.DTOs.Vehicles;

namespace PrimeKare.Api.Services;

public interface IVehicleService
{
    Task<List<VehicleDto>> GetVehiclesAsync(
        int? customerId = null);

    Task<VehicleDto?> GetVehicleAsync(
        int id,
        int? customerId = null);

    Task<VehicleDto?> CreateVehicleAsync(
        CreateVehicleDto dto,
        int customerId);

    Task<bool> UpdateVehicleAsync(
        int id,
        UpdateVehicleDto dto,
        int? customerId = null,
        bool isAdmin = false);

    Task<bool> DeleteVehicleAsync(
        int id,
        int? customerId = null,
        bool isAdmin = false);

    Task<VehicleDto?> AdminCreateVehicleAsync(
        AdminCreateVehicleDto dto);
}