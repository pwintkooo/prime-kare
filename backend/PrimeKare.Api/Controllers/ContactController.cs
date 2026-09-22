using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using PrimeKare.Api.DTOs.Contact;
using PrimeKare.Api.Services;

namespace PrimeKare.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContactController : ControllerBase
{
    private readonly IContactService _contactService;
    private readonly ILogger<ContactController> _logger;

    public ContactController(
        IContactService contactService,
        ILogger<ContactController> logger)
    {
        _contactService = contactService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> SendMessage(
        ContactRequestDto dto)
    {
        try
        {
            await _contactService.SendContactMessageAsync(dto);

            return Ok(new
            {
                message = "Your message has been sent successfully."
            });
        }
        catch (ValidationException ex)
        {
            return BadRequest(new
            {
                message = "Please check the information you entered.",
                errors = ex.Errors.Select(error => new
                {
                    field = error.PropertyName,
                    message = error.ErrorMessage
                })
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to process contact message.");

            return StatusCode(500, new
            {
                message =
                    "Unable to send your message. Please try again later."
            });
        }
    }
}