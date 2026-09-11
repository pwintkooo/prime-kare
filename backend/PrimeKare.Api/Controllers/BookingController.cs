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

    public BookingsController(
        IBookingService bookingService)
    {
        _bookingService = bookingService;
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
        var booking = await _bookingService
            .GetBookingAsync(id);

        if (booking == null)
        {
            return NotFound(new
            {
                message = "Booking not found."
            });
        }

        return Ok(booking);
    }

    [HttpPost]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> CreateBooking(
        CreateBookingDto dto)
    {
        try
        {
            var booking = await _bookingService
                .CreateBookingAsync(dto);

            return CreatedAtAction(
                nameof(GetBooking),
                new { id = booking.Id },
                booking);
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

    [HttpGet("availability")]
    public async Task<IActionResult> GetAvailability(
    [FromQuery] int serviceId,
    [FromQuery] DateOnly date)
    {
        try
        {
            var availableTimes =
                await _bookingService.GetAvailableTimesAsync(
                    serviceId,
                    date);

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
}