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
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasOne(u => u.Customer)
            .WithMany()
            .HasForeignKey(u => u.CustomerId)
            .OnDelete(DeleteBehavior.SetNull);

        var seedDate = new DateTime(
            2026, 8, 20, 0, 0, 0,
            DateTimeKind.Utc
        );

        modelBuilder.Entity<Service>().HasData(
            new Service
            {
                Id = 1,
                Name = "Oil Change",
                Slug = "oil-change",
                Description = "Engine oil and oil filter replacement.",
                Price = 89.90m,
                EstimatedMinutes = 45,
                IsActive = true,
                ImageUrl = null,
                CreatedAt = seedDate,
                UpdatedAt = seedDate
            },
            new Service
            {
                Id = 2,
                Name = "Brake Service",
                Slug = "brake-service",
                Description = "Brake inspection, repair and replacement.",
                Price = 150.00m,
                EstimatedMinutes = 90,
                IsActive = true,
                ImageUrl = null,
                CreatedAt = seedDate,
                UpdatedAt = seedDate
            },
            new Service
            {
                Id = 3,
                Name = "Engine Diagnostics",
                Slug = "engine-diagnostics",
                Description = "Computerized engine diagnostics and inspection.",
                Price = 120.00m,
                EstimatedMinutes = 60,
                IsActive = true,
                ImageUrl = null,
                CreatedAt = seedDate,
                UpdatedAt = seedDate
            },
            new Service
            {
                Id = 4,
                Name = "Tyre Service",
                Slug = "tyre-service",
                Description = "Tyre inspection, replacement and balancing.",
                Price = 80.00m,
                EstimatedMinutes = 45,
                IsActive = true,
                ImageUrl = null,
                CreatedAt = seedDate,
                UpdatedAt = seedDate
            },
            new Service
            {
                Id = 5,
                Name = "Battery Replacement",
                Slug = "battery-replacement",
                Description = "Battery testing and replacement service.",
                Price = 180.00m,
                EstimatedMinutes = 30,
                IsActive = true,
                ImageUrl = null,
                CreatedAt = seedDate,
                UpdatedAt = seedDate
            },
            new Service
            {
                Id = 6,
                Name = "Air Conditioning",
                Slug = "air-conditioning",
                Description = "Air conditioning inspection and servicing.",
                Price = 100.00m,
                EstimatedMinutes = 60,
                IsActive = true,
                ImageUrl = null,
                CreatedAt = seedDate,
                UpdatedAt = seedDate
            }
        );
    }
}