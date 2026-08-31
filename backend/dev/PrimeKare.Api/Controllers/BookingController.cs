using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrimeKare.Api.DTOs.Bookings;
using PrimeKare.Api.Services;

namespace PrimeKare.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;
    private readonly ICurrentUserService _currentUser;

    public BookingsController(
        IBookingService bookingService,
        ICurrentUserService currentUser)
    {
        _bookingService = bookingService;
        _currentUser = currentUser;
    }

    [Authorize(Roles = "Admin,Receptionist,Mechanic,Customer")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookingDto>>> GetBookings()
    {
        var customerId = _currentUser.IsCustomer
            ? _currentUser.CustomerId
            : null;

        if (_currentUser.IsCustomer && customerId == null)
        {
            return NotFound();
        }

        var bookings = await _bookingService
            .GetBookingsAsync(
                customerId,
                _currentUser.IsAdmin);

        return Ok(bookings);
    }

    [Authorize(Roles = "Admin,Receptionist,Mechanic,Customer")]
    [HttpGet("{id}")]
    public async Task<ActionResult<BookingDto>> GetBooking(int id)
    {
        var customerId = _currentUser.IsCustomer
            ? _currentUser.CustomerId
            : null;

        if (_currentUser.IsCustomer && customerId == null)
        {
            return NotFound();
        }

        var booking = await _bookingService
            .GetBookingAsync(
                id,
                customerId,
                _currentUser.IsAdmin);

        if (booking == null)
        {
            return NotFound();
        }

        return Ok(booking);
    }

    [Authorize(Roles = "Admin,Receptionist,Customer")]
    [HttpPost]
    public async Task<ActionResult<BookingDto>> CreateBooking(
        CreateBookingDto dto)
    {
        if (_currentUser.IsCustomer)
        {
            var customerId = _currentUser.CustomerId;

            if (customerId == null)
            {
                return NotFound();
            }

            // Customer can only create a booking
            // for themselves.
            if (dto.CustomerId != customerId.Value)
            {
                return Forbid();
            }
        }

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
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = "Admin,Receptionist,Customer")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBooking(
        int id,
        UpdateBookingDto dto)
    {
        var customerId = _currentUser.IsCustomer
            ? _currentUser.CustomerId
            : null;

        if (_currentUser.IsCustomer && customerId == null)
        {
            return NotFound();
        }

        try
        {
            var updated = await _bookingService
                .UpdateBookingAsync(
                    id,
                    dto,
                    customerId,
                    _currentUser.IsAdmin);

            if (!updated)
            {
                return Forbid();
            }

            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [Authorize(Roles = "Admin,Receptionist,Customer")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBooking(int id)
    {
        var customerId = _currentUser.IsCustomer
            ? _currentUser.CustomerId
            : null;

        if (_currentUser.IsCustomer && customerId == null)
        {
            return NotFound();
        }

        var deleted = await _bookingService
            .DeleteBookingAsync(
                id,
                customerId,
                _currentUser.IsAdmin);

        if (!deleted)
        {
            return Forbid();
        }

        return NoContent();
    }
}