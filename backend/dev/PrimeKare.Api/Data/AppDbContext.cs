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
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<ExternalLogin> ExternalLogins { get; set; }
    public DbSet<ExternalAuthCode> ExternalAuthCodes { get; set; }
    public DbSet<ExternalLinkRequest> ExternalLinkRequests { get; set; }

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

        modelBuilder.Entity<Booking>()
            .HasOne(b => b.Customer)
            .WithMany(c => c.Bookings)
            .HasForeignKey(b => b.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Booking>()
            .HasOne(b => b.Vehicle)
            .WithMany(v => v.Bookings)
            .HasForeignKey(b => b.VehicleId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Booking>()
            .HasOne(b => b.Service)
            .WithMany(s => s.Bookings)
            .HasForeignKey(b => b.ServiceId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ExternalLogin>()
            .HasOne(e => e.User)
            .WithMany(u => u.ExternalLogins)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        //prevent dup Google account
        modelBuilder.Entity<ExternalLogin>()
            .HasIndex(e => new
            {
                e.Provider,
                e.ProviderUserId
            })
            .IsUnique();

        modelBuilder.Entity<ExternalAuthCode>()
            .HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ExternalAuthCode>()
            .HasIndex(e => e.Code)
            .IsUnique();

        modelBuilder.Entity<ExternalLinkRequest>()
            .HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ExternalLinkRequest>()
            .HasIndex(e => e.Code)
            .IsUnique();

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