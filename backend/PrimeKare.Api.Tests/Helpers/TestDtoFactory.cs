using PrimeKare.Api.DTOs.Vehicles;

namespace PrimeKare.Api.Tests.Helpers;

public static class TestDtoFactory
{
    public static CreateVehicleDto CreateVehicle(
        string plateNumber = "SAA1234A",
        string make = "Toyota",
        string model = "Corolla",
        int year = 2024)
    {
        return new CreateVehicleDto
        {
            PlateNumber = plateNumber,
            Make = make,
            Model = model,
            Year = year
        };
    }

    public static UpdateVehicleDto UpdateVehicle(
    string plateNumber = "SLB5678B",
    string make = "Honda",
    string model = "Civic",
    int year = 2025)
    {
        return new UpdateVehicleDto
        {
            PlateNumber = plateNumber,
            Make = make,
            Model = model,
            Year = year
        };
    }
}