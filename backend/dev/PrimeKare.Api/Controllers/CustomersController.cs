using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrimeKare.Api.DTOs.Customers;
using PrimeKare.Api.Services;

namespace PrimeKare.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomersController(
        ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [Authorize(Roles = "Admin,Receptionist,Mechanic")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CustomerDto>>>
        GetCustomers()
    {
        var customers = await _customerService
            .GetCustomersAsync();

        return Ok(customers);
    }

    [Authorize(Roles = "Admin,Receptionist,Mechanic")]
    [HttpGet("{id}")]
    public async Task<ActionResult<CustomerDto>>
        GetCustomer(int id)
    {
        var customer = await _customerService
            .GetCustomerAsync(id);

        if (customer == null)
        {
            return NotFound();
        }

        return Ok(customer);
    }

    [Authorize(Roles = "Admin,Receptionist")]
    [HttpPost]
    public async Task<ActionResult<CustomerDto>>
        CreateCustomer(CreateCustomerDto dto)
    {
        var customer = await _customerService
            .CreateCustomerAsync(dto);

        return CreatedAtAction(
            nameof(GetCustomer),
            new { id = customer.Id },
            customer
        );
    }

    [Authorize(Roles = "Admin,Receptionist,Customer")]
    [HttpPut("{id}")]
    public async Task<IActionResult>
        UpdateCustomer(
            int id,
            UpdateCustomerDto dto)
    {
        var updated = await _customerService
            .UpdateCustomerAsync(id, dto);

        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult>
        DeleteCustomer(int id)
    {
        var deleted = await _customerService
            .DeleteCustomerAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}