using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrimeKare.Api.DTOs.Bookings;
using PrimeKare.Api.Services;
using PrimeKare.Api.Services.Exceptions;

namespace PrimeKare.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;
    private readonly IBookingDocumentService _bookingDocumentService;

    public BookingsController(
        IBookingService bookingService,
        IBookingDocumentService bookingDocumentService)
    {
        _bookingService = bookingService;
        _bookingDocumentService = bookingDocumentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetBookings()
    {
        var bookings = await _bookingService
            .GetBookingsAsync();

        return Ok(bookings);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBooking(int id)
    {
        try
        {
            var booking = await _bookingService
                .GetBookingAsync(id);

            return Ok(booking);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                message = ex.Message
            });
        }
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> CreateBooking(
        CreateBookingDto dto)
    {
        try
        {
            var result = await _bookingService
                .CreateBookingAsync(dto);

            return StatusCode(
                StatusCodes.Status201Created,
                result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                message = ex.Message
            });
        }
        catch (ConflictException ex)
        {
            return Conflict(new
            {
                message = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new
            {
                message = ex.Message
            });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBooking(
        int id,
        UpdateBookingDto dto)
    {
        try
        {
            await _bookingService.UpdateBookingAsync(
                id,
                dto);

            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                message = ex.Message
            });
        }
        catch (ConflictException ex)
        {
            return Conflict(new
            {
                message = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new
            {
                message = ex.Message
            });
        }
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateBookingStatus(
        int id,
        UpdateBookingStatusDto dto)
    {
        try
        {
            await _bookingService.UpdateBookingStatusAsync(
                id,
                dto);

            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                message = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new
            {
                message = ex.Message
            });
        }
    }

    [AllowAnonymous]
    [HttpGet("availability")]
    public async Task<IActionResult> GetAvailability(
        [FromQuery] int serviceId,
        [FromQuery] DateOnly date,
        [FromQuery] int? bookingId = null)
    {
        try
        {
            var availableTimes =
                await _bookingService.GetAvailableTimesAsync(
                    serviceId,
                    date,
                    bookingId);

            return Ok(new BookingAvailabilityDto
            {
                Date = date,
                ServiceId = serviceId,
                AvailableTimes = availableTimes
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                message = ex.Message
            });
        }
    }

    [HttpGet("{id}/confirmation")]
    public async Task<IActionResult> GetBookingConfirmation(
    int id)
    {
        try
        {
            var booking = await _bookingService
                .GetBookingAsync(id);

            var pdfBytes = _bookingDocumentService
                .GenerateBookingConfirmation(booking);

            return File(
                pdfBytes,
                "application/pdf",
                $"PrimeKare-{booking.ReferenceNumber}.pdf");
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                message = ex.Message
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new
            {
                message = ex.Message
            });
        }
    }

    [AllowAnonymous]
    [HttpGet("guest/confirmation")]
    public async Task<IActionResult> GetGuestBookingConfirmation(
    [FromQuery] string token)
    {
        try
        {
            var booking = await _bookingService
                .GetGuestBookingByTokenAsync(token);

            var pdfBytes = _bookingDocumentService
                .GenerateBookingConfirmation(booking);

            return File(
                pdfBytes,
                "application/pdf",
                $"PrimeKare-{booking.ReferenceNumber}.pdf");
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