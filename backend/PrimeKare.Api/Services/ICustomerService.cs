using PrimeKare.Api.DTOs.Customers;

namespace PrimeKare.Api.Services;

public interface ICustomerService
{
    Task<IEnumerable<CustomerDto>> GetCustomersAsync();

    Task<CustomerDto?> GetCustomerAsync(int id);

    Task<CustomerDto> CreateCustomerAsync(
        CreateCustomerDto dto);

    Task<bool> UpdateCustomerAsync(
        int id,
        UpdateCustomerDto dto);

    Task<bool> DeleteCustomerAsync(int id);
}