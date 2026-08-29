using Microsoft.AspNetCore.Mvc;
using PrimeKare.Api.DTOs.Services;
using PrimeKare.Api.Services;

namespace PrimeKare.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServicesController : ControllerBase
{
    private readonly IServiceService _serviceService;

    public ServicesController(
        IServiceService serviceService)
    {
        _serviceService = serviceService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ServiceDto>>> GetServices()
    {
        var services = await _serviceService
            .GetServicesAsync();

        return services;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ServiceDto>> GetService(int id)
    {
        var service = await _serviceService
            .GetServiceAsync(id);

        if (service == null)
        {
            return NotFound();
        }

        return service;
    }

    [HttpPost]
    public async Task<ActionResult<ServiceDto>> CreateService(
        [FromForm] CreateServiceDto dto)
    {
        var service = await _serviceService
            .CreateServiceAsync(dto);

        return CreatedAtAction(
            nameof(GetService),
            new { id = service.Id },
            service
        );
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateService(
        int id,
        [FromForm] UpdateServiceDto dto)
    {
        var updated = await _serviceService
            .UpdateServiceAsync(id, dto);

        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteService(int id)
    {
        var deleted = await _serviceService
            .DeleteServiceAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}