using Microsoft.EntityFrameworkCore;
using PrimeKare.Api.Data;
using PrimeKare.Api.DTOs.Customers;
using PrimeKare.Api.Models;

namespace PrimeKare.Api.Services;

public class CustomerService : ICustomerService
{
    private readonly AppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CustomerService(
        AppDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<IEnumerable<CustomerDto>>
        GetCustomersAsync()
    {
        return await _context.Customers
            .Select(customer => new CustomerDto
            {
                Id = customer.Id,
                Name = customer.Name,
                Email = customer.Email,
                Phone = customer.Phone ?? "",
                Status = customer.Status,
                CreatedAt = customer.CreatedAt,
                UpdatedAt = customer.UpdatedAt
            })
            .ToListAsync();
    }

    public async Task<CustomerDto>
        GetCustomerAsync(int id)
    {
        var customer =
            await _context.Customers
                .FirstOrDefaultAsync(
                    c => c.Id == id
                );

        if (customer == null)
        {
            throw new KeyNotFoundException(
                "Customer not found."
            );
        }

        return new CustomerDto
        {
            Id = customer.Id,
            Name = customer.Name,
            Email = customer.Email,
            Phone = customer.Phone ?? "",
            Status = customer.Status,
            CreatedAt = customer.CreatedAt,
            UpdatedAt = customer.UpdatedAt
        };
    }

    public async Task<CustomerDto>
        CreateCustomerAsync(
            CreateCustomerDto dto)
    {
        var customer = new Customer
        {
            Name = dto.Name.Trim(),
            Email = dto.Email
                .Trim()
                .ToLowerInvariant(),
            Phone = dto.Phone?.Trim(),
            Status = "active",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Customers.Add(customer);

        await _context.SaveChangesAsync();

        return new CustomerDto
        {
            Id = customer.Id,
            Name = customer.Name,
            Email = customer.Email,
            Phone = customer.Phone ?? "",
            Status = customer.Status,
            CreatedAt = customer.CreatedAt,
            UpdatedAt = customer.UpdatedAt
        };
    }

    public async Task UpdateCustomerAsync(
        int id,
        UpdateCustomerDto dto)
    {
        if (_currentUser.IsCustomer)
        {
            var customerId =
                _currentUser.CustomerId;

            if (!customerId.HasValue)
            {
                throw new InvalidOperationException(
                    "Customer account is not properly configured."
                );
            }

            if (customerId.Value != id)
            {
                throw new UnauthorizedAccessException();
            }
        }

        var customer =
            await _context.Customers
                .FirstOrDefaultAsync(
                    c => c.Id == id
                );

        if (customer == null)
        {
            throw new KeyNotFoundException(
                "Customer not found."
            );
        }

        customer.Name = dto.Name.Trim();
        customer.Email = dto.Email
            .Trim()
            .ToLowerInvariant();
        customer.Phone =
            string.IsNullOrWhiteSpace(dto.Phone)
                ? null
                : dto.Phone.Trim();

        customer.Status = dto.Status;

        customer.UpdatedAt =
            DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteCustomerAsync(
        int id)
    {
        var customer =
            await _context.Customers
                .FirstOrDefaultAsync(
                    c => c.Id == id
                );

        if (customer == null)
        {
            throw new KeyNotFoundException(
                "Customer not found."
            );
        }

        customer.Status = "inactive";
        customer.UpdatedAt =
            DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }
}