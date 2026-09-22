using PrimeKare.Api.DTOs.Contact;

namespace PrimeKare.Api.Services;

public interface IContactService
{
    Task SendContactMessageAsync(ContactRequestDto dto);
}