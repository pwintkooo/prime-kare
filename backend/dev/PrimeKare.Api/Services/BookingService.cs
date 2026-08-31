using Microsoft.EntityFrameworkCore;
using PrimeKare.Api.Data;
using PrimeKare.Api.DTOs.Bookings;
using PrimeKare.Api.Models;

namespace PrimeKare.Api.Services;

public class BookingService : IBookingService
{
    private readonly AppDbContext _context;

    public BookingService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<BookingDto>> GetBookingsAsync(
        int? customerId = null,
        bool isAdmin = false)
    {
        var query = _context.Bookings
            .AsQueryable();

        if (isAdmin)
        {
            query = query.Where(b => !b.IsDeleted);
        }
        else if (customerId.HasValue)
        {
            query = query.Where(b =>
                b.CustomerId == customerId.Value &&
                b.Status != "cancelled" &&
                !b.IsDeleted);
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

    public async Task<BookingDto?> GetBookingAsync(
        int id,
        int? customerId = null,
        bool isAdmin = false)
    {
        var query = _context.Bookings
            .Where(b => b.Id == id);

        if (isAdmin)
        {
            query = query.Where(b => !b.IsDeleted);
        }
        else if (customerId.HasValue)
        {
            query = query.Where(b =>
                b.CustomerId == customerId.Value &&
                b.Status != "cancelled" &&
                !b.IsDeleted);
        }

        return await query
            .Where(b => b.Id == id)
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
        var customerExists = await _context.Customers
            .AnyAsync(c => c.Id == dto.CustomerId);

        if (!customerExists)
        {
            throw new KeyNotFoundException(
                "Customer not found.");
        }

        var vehicle = await _context.Vehicles
            .FirstOrDefaultAsync(v =>
                v.Id == dto.VehicleId);

        if (vehicle == null)
        {
            throw new KeyNotFoundException(
                "Vehicle not found.");
        }

        if (vehicle.CustomerId != dto.CustomerId)
        {
            throw new InvalidOperationException(
                "Vehicle does not belong to the customer.");
        }

        var serviceExists = await _context.Services
            .AnyAsync(s => s.Id == dto.ServiceId);

        if (!serviceExists)
        {
            throw new KeyNotFoundException(
                "Service not found.");
        }

        var booking = new Booking
        {
            CustomerId = dto.CustomerId,
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
        UpdateBookingDto dto,
        int? customerId = null,
        bool isAdmin = false)
    {
        var booking = await _context.Bookings
            .FirstOrDefaultAsync(b => b.Id == id);

        if (booking == null)
        {
            throw new KeyNotFoundException("Booking not found.");
        }

        if (customerId.HasValue &&
            booking.CustomerId != customerId.Value)
        {
            return false;
        }

        if (booking.Status != "pending" && !isAdmin)
        {
            return false;
        }

        booking.VehicleId = dto.VehicleId;
        booking.ServiceId = dto.ServiceId;
        booking.BookingDate = dto.BookingDate;
        booking.BookingTime = dto.BookingTime;
        booking.Notes = dto.Notes;
        booking.Status = dto.Status;
        booking.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteBookingAsync(
        int id,
        int? customerId = null,
        bool isAdmin = false)
    {
        var booking = await _context.Bookings
            .FirstOrDefaultAsync(b => b.Id == id);

        if (booking == null)
        {
            return false;
        }

        if (customerId.HasValue &&
            booking.CustomerId != customerId.Value)
        {
            return false;
        }

        if (!isAdmin && booking.Status != "pending")
        {
            return false;
        }

        if (isAdmin)
        {
            booking.IsDeleted = true;
        }
        else
        {
            booking.Status = "cancelled";
        }

        booking.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return true;
    }
}