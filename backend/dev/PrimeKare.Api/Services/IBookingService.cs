using PrimeKare.Api.DTOs.Bookings;

namespace PrimeKare.Api.Services;

public interface IBookingService
{
    Task<IEnumerable<BookingDto>> GetBookingsAsync(
        int? customerId = null,
        bool isAdmin = false);

    Task<BookingDto?> GetBookingAsync(
        int id,
        int? customerId = null,
        bool isAdmin = false);

    Task<BookingDto> CreateBookingAsync(
        CreateBookingDto dto);

    Task<bool> UpdateBookingAsync(
        int id,
        UpdateBookingDto dto,
        int? customerId = null,
        bool isAdmin = false);

    Task<bool> DeleteBookingAsync(int id,
        int? customerId = null,
        bool isAdmin = false);
}