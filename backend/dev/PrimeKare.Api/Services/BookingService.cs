using Microsoft.EntityFrameworkCore;
using PrimeKare.Api.Data;
using PrimeKare.Api.DTOs.Bookings;
using PrimeKare.Api.Models;
using PrimeKare.Api.Services.Exceptions;

namespace PrimeKare.Api.Services;

public class BookingService : IBookingService
{
    private readonly AppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public BookingService(
        AppDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    private async Task<bool> HasBookingConflictAsync(
    DateOnly bookingDate,
    TimeSpan bookingTime,
    int serviceId,
    int? excludeBookingId = null)
    {
        var service = await _context.Services
            .FirstOrDefaultAsync(s =>
                s.Id == serviceId &&
                s.IsActive &&
                !s.IsDeleted);

        if (service == null)
        {
            throw new KeyNotFoundException(
                "Service not found.");
        }

        var newBookingEnd = bookingTime.Add(
            TimeSpan.FromMinutes(
                service.EstimatedMinutes));

        var bookings = await _context.Bookings
            .Where(b =>
                b.BookingDate == bookingDate &&
                !b.IsDeleted &&
                (
                    b.Status == "pending" ||
                    b.Status == "confirmed" ||
                    b.Status == "in_progress"
                ) &&
                (excludeBookingId == null ||
                 b.Id != excludeBookingId))
            .Select(b => new
            {
                b.Id,
                b.BookingTime,
                EstimatedMinutes = b.Service.EstimatedMinutes
            })
            .ToListAsync();

        return bookings.Any(b =>
        {
            var existingBookingEnd = b.BookingTime.Add(
                TimeSpan.FromMinutes(
                    b.EstimatedMinutes));

            return bookingTime < existingBookingEnd &&
                   newBookingEnd > b.BookingTime;
        });
    }

    public async Task<IEnumerable<BookingDto>> GetBookingsAsync()
    {
        var query = _context.Bookings
            .Where(b => !b.IsDeleted);

        if (_currentUser.IsCustomer)
        {
            var customerId = _currentUser.CustomerId;

            if (!customerId.HasValue)
            {
                return [];
            }

            query = query.Where(b =>
                b.CustomerId == customerId.Value);
        }

        return await query
            .Select(b => new BookingDto
            {
                Id = b.Id,

                CustomerId = b.CustomerId,
                CustomerName = b.Customer.Name,

                VehicleId = b.VehicleId,
                VehiclePlateNumber = b.Vehicle.PlateNumber,
                VehicleMake = b.Vehicle.Make,
                VehicleModel = b.Vehicle.Model,

                ServiceId = b.ServiceId,
                ServiceName = b.Service.Name,

                BookingDate = b.BookingDate,
                BookingTime = b.BookingTime,

                Status = b.Status,
                IsDeleted = b.IsDeleted,
                Notes = b.Notes,

                CreatedAt = b.CreatedAt,
                UpdatedAt = b.UpdatedAt
            })
            .ToListAsync();
    }

    public async Task<BookingDto?> GetBookingAsync(int id)
    {
        var query = _context.Bookings
            .Where(b =>
                b.Id == id &&
                !b.IsDeleted);

        if (_currentUser.IsCustomer)
        {
            var customerId = _currentUser.CustomerId;

            if (!customerId.HasValue)
            {
                return null;
            }

            query = query.Where(b =>
                b.CustomerId == customerId.Value);
        }

        return await query
            .Select(b => new BookingDto
            {
                Id = b.Id,

                CustomerId = b.CustomerId,
                CustomerName = b.Customer.Name,

                VehicleId = b.VehicleId,
                VehiclePlateNumber = b.Vehicle.PlateNumber,
                VehicleMake = b.Vehicle.Make,
                VehicleModel = b.Vehicle.Model,

                ServiceId = b.ServiceId,
                ServiceName = b.Service.Name,

                BookingDate = b.BookingDate,
                BookingTime = b.BookingTime,

                Status = b.Status,
                IsDeleted = b.IsDeleted,
                Notes = b.Notes,

                CreatedAt = b.CreatedAt,
                UpdatedAt = b.UpdatedAt
            })
            .FirstOrDefaultAsync();
    }

    public async Task<BookingDto> CreateBookingAsync(
    CreateBookingDto dto)
    {
        if (!_currentUser.IsCustomer)
        {
            throw new UnauthorizedAccessException(
                "Only customers can create bookings.");
        }

        var customerId = _currentUser.CustomerId;

        if (!customerId.HasValue)
        {
            throw new UnauthorizedAccessException(
                "Customer account is not associated with a customer.");
        }

        var vehicle = await _context.Vehicles
            .FirstOrDefaultAsync(v =>
                v.Id == dto.VehicleId &&
                v.Status == "active" &&
                !v.IsDeleted);

        if (vehicle == null)
        {
            throw new KeyNotFoundException(
                "Vehicle not found.");
        }

        var service = await _context.Services
            .FirstOrDefaultAsync(s =>
                s.Id == dto.ServiceId &&
                s.IsActive &&
                !s.IsDeleted);

        if (service == null)
        {
            throw new KeyNotFoundException(
                "Service not found.");
        }

        var hasConflict = await HasBookingConflictAsync(
            dto.BookingDate,
            dto.BookingTime,
            dto.ServiceId);

        if (hasConflict)
        {
            throw new ConflictException(
                "The selected time slot is no longer available.");
        }

        var booking = new Booking
        {
            CustomerId = customerId.Value,
            VehicleId = dto.VehicleId,
            ServiceId = dto.ServiceId,

            BookingDate = dto.BookingDate,
            BookingTime = dto.BookingTime,

            Status = "pending",
            Notes = dto.Notes,

            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Bookings.Add(booking);

        await _context.SaveChangesAsync();

        return (await GetBookingAsync(booking.Id))!;
    }

    public async Task<bool> UpdateBookingAsync(
    int id,
    UpdateBookingDto dto)
    {
        var booking = await _context.Bookings
            .FirstOrDefaultAsync(b =>
                b.Id == id &&
                !b.IsDeleted);

        if (booking == null)
        {
            throw new KeyNotFoundException(
                "Booking not found.");
        }

        if (!_currentUser.IsCustomer &&
        !_currentUser.IsReceptionist &&
        !_currentUser.IsAdmin)
        {
            throw new UnauthorizedAccessException(
                "You are not allowed to update this booking.");
        }

        // Customer can only update their own booking
        if (_currentUser.IsCustomer)
        {
            var customerId = _currentUser.CustomerId;

            if (!customerId.HasValue ||
                booking.CustomerId != customerId.Value)
            {
                throw new UnauthorizedAccessException(
                    "You are not allowed to modify this booking.");
            }

            // Customer can only modify pending bookings
            if (booking.Status != "pending")
            {
                throw new InvalidOperationException(
                    "Only pending bookings can be modified.");
            }
        }

        // Validate vehicle
        var vehicle = await _context.Vehicles
            .FirstOrDefaultAsync(v =>
                v.Id == dto.VehicleId &&
                v.Status == "active" &&
                !v.IsDeleted);

        if (vehicle == null)
        {
            throw new KeyNotFoundException(
                "Vehicle not found.");
        }

        // Validate service
        var serviceExists = await _context.Services
            .AnyAsync(s =>
                s.Id == dto.ServiceId &&
                s.IsActive &&
                !s.IsDeleted);

        if (!serviceExists)
        {
            throw new KeyNotFoundException(
                "Service not found.");
        }

        // Check appointment availability
        var hasConflict = await HasBookingConflictAsync(
            dto.BookingDate,
            dto.BookingTime,
            dto.ServiceId,
            id);

        if (hasConflict)
        {
            throw new ConflictException(
                "The selected time slot is no longer available.");
        }

        booking.VehicleId = dto.VehicleId;
        booking.ServiceId = dto.ServiceId;
        booking.BookingDate = dto.BookingDate;
        booking.BookingTime = dto.BookingTime;
        booking.Notes = dto.Notes;
        booking.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> UpdateBookingStatusAsync(
    int id,
    UpdateBookingStatusDto dto)
    {
        var booking = await _context.Bookings
            .FirstOrDefaultAsync(b =>
                b.Id == id &&
                !b.IsDeleted);

        if (booking == null)
        {
            throw new KeyNotFoundException(
                "Booking not found.");
        }

        var newStatus = dto.Status.Trim().ToLower();

        if (string.IsNullOrWhiteSpace(newStatus))
        {
            throw new InvalidOperationException(
                "Invalid booking status.");
        }

        var currentStatus = booking.Status;

        // Customer
        if (_currentUser.IsCustomer)
        {
            if (booking.CustomerId != _currentUser.CustomerId)
            {
                throw new UnauthorizedAccessException(
                    "You are not allowed to modify this booking.");
            }

            if (currentStatus != "pending" &&
                currentStatus != "confirmed")
            {
                throw new InvalidOperationException(
                    "This booking cannot be cancelled.");
            }

            if (newStatus != "cancelled")
            {
                throw new InvalidOperationException(
                    "Customers can only cancel bookings.");
            }
        }

        // Receptionist
        else if (_currentUser.IsReceptionist)
        {
            var allowed = currentStatus switch
            {
                "pending" =>
                    newStatus == "confirmed" ||
                    newStatus == "cancelled",

                "confirmed" =>
                    newStatus == "cancelled",

                _ => false
            };

            if (!allowed)
            {
                throw new InvalidOperationException(
                    "Invalid booking status transition.");
            }
        }

        // Mechanic
        else if (_currentUser.IsMechanic)
        {
            if (currentStatus != "in_progress" ||
                newStatus != "completed")
            {
                throw new InvalidOperationException(
                    "Mechanics can only mark in-progress bookings as completed.");
            }
        }

        // Admin
        else if (_currentUser.IsAdmin)
        {
            var allowedStatuses = new[]
            {
            "pending",
            "confirmed",
            "in_progress",
            "completed",
            "cancelled",
            "no_show"
        };

            if (!allowedStatuses.Contains(newStatus))
            {
                throw new InvalidOperationException(
                    "Invalid booking status.");
            }
        }

        else
        {
            throw new UnauthorizedAccessException(
                "You are not allowed to modify this booking.");
        }

        booking.Status = newStatus;
        booking.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return true;
    }
}