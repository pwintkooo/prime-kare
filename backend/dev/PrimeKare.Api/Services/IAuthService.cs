using PrimeKare.Api.DTOs.Auth;

namespace PrimeKare.Api.Services;

public interface IAuthService
{
    Task<SignUpResponseDto> SignUpAsync(SignUpDto request);

    Task<SignInResponseDto?> SignInAsync(SignInDto request);
}