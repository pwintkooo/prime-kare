using Microsoft.EntityFrameworkCore;
using FluentValidation;
using System.Security.Cryptography;
using PrimeKare.Api.Data;
using PrimeKare.Api.DTOs.Bookings;
using PrimeKare.Api.Models;
using PrimeKare.Api.Services.Exceptions;
using PrimeKare.Api.Templates;
using PrimeKare.Api.DTOs.Email;

namespace PrimeKare.Api.Services;

public class BookingService : IBookingService
{
    private readonly AppDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IValidator<CreateBookingDto> _createBookingValidator;
    private readonly IValidator<UpdateBookingDto> _updateBookingValidator;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<BookingService> _logger;
    private readonly IBookingDocumentService _bookingDocumentService;

    public BookingService(
        AppDbContext context,
        ICurrentUserService currentUser,
        IValidator<CreateBookingDto> createBookingValidator,
        IValidator<UpdateBookingDto> updateBookingValidator,
        IEmailService emailService,
        IConfiguration configuration,
        ILogger<BookingService> logger,
        IBookingDocumentService bookingDocumentService
        )
    {
        _context = context;
        _currentUser = currentUser;
        _createBookingValidator = createBookingValidator;
        _updateBookingValidator = updateBookingValidator;
        _emailService = emailService;
        _configuration = configuration;
        _logger = logger;
        _bookingDocumentService = bookingDocumentService;
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
                    b.Status == "in-progress"
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

    private static string GenerateReferenceNumber()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";

        var randomPart = new string(
            Enumerable.Range(0, 6)
                .Select(_ => chars[Random.Shared.Next(chars.Length)])
                .ToArray()
        );

        return $"PK-{DateTime.UtcNow:yyyyMMdd}-{randomPart}";
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
                ReferenceNumber = b.ReferenceNumber,

                CustomerId = b.CustomerId,
                CustomerName = b.CustomerName,
                CustomerEmail = b.CustomerEmail,
                CustomerPhone = b.CustomerPhone,

                VehicleId = b.VehicleId,
                VehiclePlateNumber = b.VehiclePlateNumber,
                VehicleMake = b.VehicleMake,
                VehicleModel = b.VehicleModel,

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

    public async Task<BookingDto> GetBookingAsync(int id)
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
                throw new KeyNotFoundException(
                    "Customer account not found.");
            }

            query = query.Where(b =>
                b.CustomerId == customerId.Value);
        }

        var booking = await query
            .Select(b => new BookingDto
            {
                Id = b.Id,
                ReferenceNumber = b.ReferenceNumber,

                CustomerId = b.CustomerId,
                CustomerName = b.CustomerName,
                CustomerEmail = b.CustomerEmail,
                CustomerPhone = b.CustomerPhone,

                VehicleId = b.VehicleId,
                VehiclePlateNumber = b.VehiclePlateNumber,
                VehicleMake = b.VehicleMake,
                VehicleModel = b.VehicleModel,

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

        if (booking == null)
        {
            throw new KeyNotFoundException(
                "Booking not found.");
        }

        return booking;
    }

    public async Task<CreateBookingResponseDto> CreateBookingAsync(
    CreateBookingDto dto)
    {
        await _createBookingValidator.ValidateAndThrowAsync(dto);

        int? customerId = null;
        int? vehicleId = null;

        string customerName;
        string customerEmail;
        string customerPhone;

        string vehiclePlateNumber;
        string vehicleMake;
        string vehicleModel;

        string? guestAccessToken = null;

        // Registered customer
        if (_currentUser.IsCustomer)
        {
            customerId = _currentUser.CustomerId;

            if (!customerId.HasValue)
            {
                throw new UnauthorizedAccessException(
                    "Customer account is not associated with a customer.");
            }

            if (!dto.VehicleId.HasValue)
            {
                throw new InvalidOperationException(
                    "Vehicle is required.");
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c =>
                    c.Id == customerId.Value &&
                    !c.IsDeleted);

            if (customer == null)
            {
                throw new KeyNotFoundException(
                    "Customer not found.");
            }

            var vehicle = await _context.Vehicles
                .FirstOrDefaultAsync(v =>
                    v.Id == dto.VehicleId.Value &&
                    v.CustomerId == customerId.Value &&
                    v.Status == "active" &&
                    !v.IsDeleted);

            if (vehicle == null)
            {
                throw new KeyNotFoundException(
                    "Vehicle not found.");
            }

            vehicleId = vehicle.Id;

            if (string.IsNullOrWhiteSpace(customer.Phone))
            {
                throw new InvalidOperationException(
                    "Please add a phone number to your profile before creating a booking.");
            }

            // Snapshot customer information
            customerName = customer.Name;
            customerEmail = customer.Email;
            customerPhone = customer.Phone;

            // Snapshot vehicle information
            vehiclePlateNumber = vehicle.PlateNumber;
            vehicleMake = vehicle.Make;
            vehicleModel = vehicle.Model;
        }

        // Staff should not create bookings through
        // the public/customer booking flow
        else if (
            _currentUser.IsAdmin ||
            _currentUser.IsReceptionist ||
            _currentUser.IsMechanic)
        {
            throw new UnauthorizedAccessException(
                "Only customers and guests can create bookings.");
        }

        // Guest
        else
        {
            if (string.IsNullOrWhiteSpace(dto.CustomerName) ||
                string.IsNullOrWhiteSpace(dto.CustomerEmail) ||
                string.IsNullOrWhiteSpace(dto.CustomerPhone))
            {
                throw new InvalidOperationException(
                    "Guest contact information is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.VehiclePlateNumber) ||
                string.IsNullOrWhiteSpace(dto.VehicleMake) ||
                string.IsNullOrWhiteSpace(dto.VehicleModel))
            {
                throw new InvalidOperationException(
                    "Guest vehicle information is required.");
            }

            guestAccessToken = GenerateGuestAccessToken();

            customerName = dto.CustomerName.Trim();

            customerEmail = dto.CustomerEmail
                .Trim()
                .ToLowerInvariant();

            customerPhone = dto.CustomerPhone.Trim();

            vehiclePlateNumber = dto.VehiclePlateNumber
                .Trim()
                .ToUpperInvariant();

            vehicleMake = dto.VehicleMake.Trim();
            vehicleModel = dto.VehicleModel.Trim();
        }

        // Validate service
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

        // Singapore current date
        var singaporeTimeZone =
            TimeZoneInfo.FindSystemTimeZoneById(
                "Singapore Standard Time");

        var today = DateOnly.FromDateTime(
            TimeZoneInfo.ConvertTimeFromUtc(
                DateTime.UtcNow,
                singaporeTimeZone));

        if (dto.BookingDate < today)
        {
            throw new InvalidOperationException(
                "Booking date cannot be in the past.");
        }

        if (dto.BookingDate.DayOfWeek == DayOfWeek.Sunday)
        {
            throw new InvalidOperationException(
                "The workshop is closed on Sundays.");
        }

        // Check booking conflict
        var hasConflict = await HasBookingConflictAsync(
            dto.BookingDate,
            dto.BookingTime,
            dto.ServiceId);

        if (hasConflict)
        {
            throw new ConflictException(
                "The selected time slot is no longer available.");
        }

        // Create booking
        var booking = new Booking
        {
            ReferenceNumber = GenerateReferenceNumber(),

            CustomerId = customerId,
            VehicleId = vehicleId,

            ServiceId = dto.ServiceId,

            CustomerName = customerName,
            CustomerEmail = customerEmail,
            CustomerPhone = customerPhone,

            VehiclePlateNumber = vehiclePlateNumber,
            VehicleMake = vehicleMake,
            VehicleModel = vehicleModel,

            BookingDate = dto.BookingDate,
            BookingTime = dto.BookingTime,

            Status = "pending",
            Notes = dto.Notes,
            GuestAccessToken = guestAccessToken,

            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        _context.Bookings.Add(booking);

        await _context.SaveChangesAsync();

        var bookingDto = new BookingDto
        {
            Id = booking.Id,
            ReferenceNumber = booking.ReferenceNumber,

            CustomerId = booking.CustomerId,
            CustomerName = booking.CustomerName,
            CustomerEmail = booking.CustomerEmail,
            CustomerPhone = booking.CustomerPhone,

            VehicleId = booking.VehicleId,
            VehiclePlateNumber = booking.VehiclePlateNumber,
            VehicleMake = booking.VehicleMake,
            VehicleModel = booking.VehicleModel,

            ServiceId = booking.ServiceId,
            ServiceName = service.Name,

            BookingDate = booking.BookingDate,
            BookingTime = booking.BookingTime,

            Status = booking.Status,
            IsDeleted = booking.IsDeleted,
            Notes = booking.Notes,

            CreatedAt = booking.CreatedAt,
            UpdatedAt = booking.UpdatedAt
        };

        try
        {
            var frontendUrl =
                _configuration["FrontendUrl"]
                ?? throw new InvalidOperationException(
                    "Frontend URL is not configured.");

            var emailBody =
                BookingEmailTemplate.BuildConfirmationEmail(
                    bookingDto,
                    frontendUrl);

            var pdfBytes =
                _bookingDocumentService.GenerateBookingConfirmation(
                    bookingDto);

            var attachment = new EmailFileAttachment
            {
                FileName =
                    $"PrimeKare-{bookingDto.ReferenceNumber}.pdf",

                Content = pdfBytes,

                ContentType = "application/pdf"
            };

            await _emailService.SendEmailAsync(
                bookingDto.CustomerEmail,
                $"Booking Received - {bookingDto.ReferenceNumber}",
                emailBody,
                attachment: attachment);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to send booking confirmation email for booking {ReferenceNumber}.",
                bookingDto.ReferenceNumber);
        }

        return new CreateBookingResponseDto
        {
            Booking = bookingDto,
            GuestAccessToken = guestAccessToken
        };
    }

    public async Task<bool> UpdateBookingAsync(
    int id,
    UpdateBookingDto dto)
    {
        await _updateBookingValidator.ValidateAndThrowAsync(dto);

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

        if (_currentUser.IsCustomer)
        {
            var customerId = _currentUser.CustomerId;

            if (!customerId.HasValue ||
                booking.CustomerId != customerId.Value)
            {
                throw new UnauthorizedAccessException(
                    "You are not allowed to modify this booking.");
            }

            if (booking.Status != "pending")
            {
                throw new InvalidOperationException(
                    "Only pending bookings can be modified.");
            }
        }

        if (!dto.VehicleId.HasValue)
        {
            throw new InvalidOperationException(
                "Vehicle is required.");
        }

        if (!booking.CustomerId.HasValue)
        {
            throw new InvalidOperationException(
                "Guest bookings cannot be modified through this endpoint.");
        }

        var vehicle = await _context.Vehicles
            .FirstOrDefaultAsync(v =>
                v.Id == dto.VehicleId.Value &&
                v.CustomerId == booking.CustomerId.Value &&
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

        var singaporeTimeZone =
            TimeZoneInfo.FindSystemTimeZoneById(
                "Singapore Standard Time");

        var today = DateOnly.FromDateTime(
            TimeZoneInfo.ConvertTimeFromUtc(
                DateTime.UtcNow,
                singaporeTimeZone));

        if (dto.BookingDate < today)
        {
            throw new InvalidOperationException(
                "Booking date cannot be in the past.");
        }

        if (dto.BookingDate.DayOfWeek == DayOfWeek.Sunday)
        {
            throw new InvalidOperationException(
                "The workshop is closed on Sundays.");
        }

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

        booking.VehicleId = vehicle.Id;

        // Update vehicle snapshot
        booking.VehiclePlateNumber = vehicle.PlateNumber;
        booking.VehicleMake = vehicle.Make;
        booking.VehicleModel = vehicle.Model;

        booking.ServiceId = service.Id;
        booking.BookingDate = dto.BookingDate;
        booking.BookingTime = dto.BookingTime;
        booking.Notes = dto.Notes?.Trim();

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

        var newStatus = dto.Status
            .Trim()
            .ToLower();

        if (string.IsNullOrWhiteSpace(newStatus))
        {
            throw new InvalidOperationException(
                "Invalid booking status.");
        }

        var currentStatus = booking.Status;

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

            if (currentStatus == "confirmed")
            {
                var bookingDateTime =
                    booking.BookingDate.ToDateTime(
                        TimeOnly.FromTimeSpan(booking.BookingTime));

                var cancellationDeadline =
                    bookingDateTime.AddHours(-2);

                if (DateTime.Now >= cancellationDeadline)
                {
                    throw new InvalidOperationException(
                        "Confirmed bookings must be cancelled at least 2 hours before the appointment.");
                }
            }
        }
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
        else if (_currentUser.IsMechanic)
        {
            if (currentStatus != "in-progress" ||
                newStatus != "completed")
            {
                throw new InvalidOperationException(
                    "Mechanics can only mark in-progress bookings as completed.");
            }
        }
        else if (_currentUser.IsAdmin)
        {
            var allowedStatuses = new[]
            {
                "pending",
                "confirmed",
                "in-progress",
                "completed",
                "cancelled",
                "no-show"
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

    public async Task<List<string>> GetAvailableTimesAsync(
        int serviceId,
        DateOnly date,
        int? bookingId = null)
    {
        var service = await _context.Services
            .FirstOrDefaultAsync(s =>
                s.Id == serviceId &&
                !s.IsDeleted &&
                s.IsActive);

        if (service == null)
        {
            throw new KeyNotFoundException(
                "Service not found.");
        }

        var bookings = await _context.Bookings
            .Where(b =>
                b.BookingDate == date &&
                !b.IsDeleted &&
                b.Id != bookingId &&
                (
                    b.Status == "pending" ||
                    b.Status == "confirmed" ||
                    b.Status == "in-progress"
                ))
            .Select(b => new
            {
                b.BookingTime,
                EstimatedMinutes = b.Service.EstimatedMinutes
            })
            .ToListAsync();

        var availableTimes = new List<string>();

        var openingTime = TimeSpan.FromHours(9);
        var closingTime = TimeSpan.FromHours(17);

        var lunchStart = TimeSpan.FromHours(12);
        var lunchEnd = TimeSpan.FromHours(13);

        var slotInterval = TimeSpan.FromHours(1);

        var serviceDuration =
            TimeSpan.FromMinutes(
                service.EstimatedMinutes);

        var singaporeTimeZone =
            TimeZoneInfo.FindSystemTimeZoneById(
                "Singapore Standard Time");

        var singaporeNow =
            TimeZoneInfo.ConvertTimeFromUtc(
                DateTime.UtcNow,
                singaporeTimeZone);

        var today =
            DateOnly.FromDateTime(singaporeNow);

        var currentTime =
            singaporeNow.TimeOfDay;

        if (date < today)
        {
            return [];
        }

        if (date.DayOfWeek == DayOfWeek.Sunday)
        {
            return [];
        }

        for (
            var startTime = openingTime;
            startTime + serviceDuration <= closingTime;
            startTime += slotInterval)
        {
            if (date == today && startTime <= currentTime)
            {
                continue;
            }

            var endTime =
                startTime + serviceDuration;

            var overlapsLunch =
                startTime < lunchEnd &&
                endTime > lunchStart;

            if (overlapsLunch)
            {
                continue;
            }

            var hasConflict = bookings.Any(existing =>
            {
                var existingStart =
                    existing.BookingTime;

                var existingEnd =
                    existingStart +
                    TimeSpan.FromMinutes(
                        existing.EstimatedMinutes);

                return startTime < existingEnd &&
                       endTime > existingStart;
            });

            if (!hasConflict)
            {
                availableTimes.Add(
                    startTime.ToString(@"hh\:mm"));
            }
        }

        return availableTimes;
    }

    public async Task<BookingDto> GetGuestBookingByTokenAsync(
    string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new KeyNotFoundException(
                "Booking not found.");
        }

        var booking = await _context.Bookings
            .AsNoTracking()
            .Include(b => b.Service)
            .FirstOrDefaultAsync(b =>
                b.GuestAccessToken == token &&
                b.CustomerId == null &&
                !b.IsDeleted);

        if (booking == null)
        {
            throw new KeyNotFoundException(
                "Booking not found.");
        }

        return new BookingDto
        {
            Id = booking.Id,
            ReferenceNumber = booking.ReferenceNumber,

            CustomerId = booking.CustomerId,
            CustomerName = booking.CustomerName,
            CustomerEmail = booking.CustomerEmail,
            CustomerPhone = booking.CustomerPhone,

            VehicleId = booking.VehicleId,
            VehiclePlateNumber = booking.VehiclePlateNumber,
            VehicleMake = booking.VehicleMake,
            VehicleModel = booking.VehicleModel,

            ServiceId = booking.ServiceId,
            ServiceName = booking.Service.Name,

            BookingDate = booking.BookingDate,
            BookingTime = booking.BookingTime,

            Status = booking.Status,
            Notes = booking.Notes,

            IsDeleted = booking.IsDeleted,
            CreatedAt = booking.CreatedAt,
            UpdatedAt = booking.UpdatedAt
        };
    }

    private static string GenerateGuestAccessToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);

        return Convert.ToHexString(bytes);
    }
}