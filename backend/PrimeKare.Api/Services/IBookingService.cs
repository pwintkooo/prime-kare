using PrimeKare.Api.DTOs.Bookings;

namespace PrimeKare.Api.Services;

public interface IBookingService
{
    Task<IEnumerable<BookingDto>> GetBookingsAsync();

    Task<BookingDto> GetBookingAsync(int id);

    Task<BookingDto> CreateBookingAsync(
        CreateBookingDto dto);

    Task<bool> UpdateBookingAsync(
        int id,
        UpdateBookingDto dto);

    Task<bool> UpdateBookingStatusAsync(
        int id,
        UpdateBookingStatusDto dto);

    Task<List<string>> GetAvailableTimesAsync(
        int serviceId,
        DateOnly date);
}