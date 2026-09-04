using PrimeKare.Api.DTOs.Auth;
using PrimeKare.Api.Models;

namespace PrimeKare.Api.Services;

public interface IAuthService
{
    Task<SignUpResponseDto> SignUpAsync(SignUpDto request);

    Task<SignInResponseDto?> SignInAsync(SignInDto request);

    SignInResponseDto CreateSignInResponse(User user);
}