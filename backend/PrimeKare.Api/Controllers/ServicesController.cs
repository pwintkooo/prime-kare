using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
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

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ServiceDto>> GetService(int id)
    {
        try
        {
            var service = await _serviceService
            .GetServiceAsync(id);

            return service;
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                message = ex.Message
            });
        }
    }

    [HttpGet("{slug}")]
    public async Task<ActionResult<ServiceDto>> GetServiceBySlug(string slug)
    {
        try
        {
            var service = await _serviceService
            .GetServiceBySlugAsync(slug);

            return service;
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                message = ex.Message
            });
        }
    }

    [Authorize(Roles = "Admin")]
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

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult>
    UpdateService(
        int id,
        [FromForm] UpdateServiceDto dto)
    {
        try
        {
            await _serviceService
                .UpdateServiceAsync(id, dto);

            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                message = ex.Message
            });
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult>
    DeleteService(int id)
    {
        try
        {
            await _serviceService
                .DeleteServiceAsync(id);

            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                message = ex.Message
            });
        }
    }
}