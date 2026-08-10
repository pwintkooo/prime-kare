using Microsoft.EntityFrameworkCore;
using PrimeKare.Api.Models;

namespace PrimeKare.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
    : base(options)
    {

    }
    public DbSet<Service> Services { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Service>().HasData(
            new Service
            {
                Id = 1,
                Name = "Oil Change",
                Description = "Engine oil and oil filter replacement.",
                Price = 89.90m,
                EstimatedMinutes = 45,
                IsActive = true
            },
            new Service
            {
                Id = 2,
                Name = "Brake Service",
                Description = "Brake inspection, repair and replacement.",
                Price = 150.00m,
                EstimatedMinutes = 90,
                IsActive = true
            },
            new Service
            {
                Id = 3,
                Name = "Engine Diagnostics",
                Description = "Computerized engine diagnostics and inspection.",
                Price = 120.00m,
                EstimatedMinutes = 60,
                IsActive = true
            },
            new Service
            {
                Id = 4,
                Name = "Tyre Service",
                Description = "Tyre inspection, replacement and balancing.",
                Price = 80.00m,
                EstimatedMinutes = 45,
                IsActive = true
            },
            new Service
            {
                Id = 5,
                Name = "Battery Replacement",
                Description = "Battery testing and replacement service.",
                Price = 180.00m,
                EstimatedMinutes = 30,
                IsActive = true
            },
            new Service
            {
                Id = 6,
                Name = "Air Conditioning",
                Description = "Air conditioning inspection and servicing.",
                Price = 100.00m,
                EstimatedMinutes = 60,
                IsActive = true
            }
        );
    }
}