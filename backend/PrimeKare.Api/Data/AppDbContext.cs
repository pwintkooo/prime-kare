using Microsoft.EntityFrameworkCore;
using PrimeKare.Api.Models;

namespace PrimeKare.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
    : base(options)
    {
        
    }
    public DbSet<Service> Services {get; set;}
}