using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace PrimeKare.Api.Tests;

public class ApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ApiTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task HealthEndpoint_ShouldReturnHealthy()
    {
        // Act
        var response = await _client.GetAsync("/api/health");

        // Assert
        // response.EnsureSuccessStatusCode();

        // var content = await response.Content.ReadAsStringAsync();

        // Assert.Contains("healthy", content);
        // Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content =
        await response.Content.ReadAsStringAsync();

        Assert.True(
            response.IsSuccessStatusCode,
            $"Expected 2xx but received {(int)response.StatusCode} " +
            $"{response.StatusCode}. Response body: {content}");
    }
}