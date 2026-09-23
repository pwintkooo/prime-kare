using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace PrimeKare.Api.Data;

public class AppDbContextFactory
    : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var environment =
            Environment.GetEnvironmentVariable(
                "ASPNETCORE_ENVIRONMENT"
            ) ?? "Development";

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddJsonFile(
                $"appsettings.{environment}.json",
                optional: true
            )
            .AddUserSecrets<Program>()
            .Build();

        var connectionName =
            args.Contains("--aiven")
                ? "AivenDevelopment"
                : "DefaultConnection";

        var connectionString =
            configuration.GetConnectionString(connectionName)
            ?? throw new InvalidOperationException(
                $"{connectionName} connection string is not configured."
            );

        var optionsBuilder =
            new DbContextOptionsBuilder<AppDbContext>();

        optionsBuilder.UseNpgsql(connectionString);

        return new AppDbContext(optionsBuilder.Options);
    }
}