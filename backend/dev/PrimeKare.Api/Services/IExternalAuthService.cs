using System.Security.Claims;
using PrimeKare.Api.DTOs.Auth;

namespace PrimeKare.Api.Services;

public interface IExternalAuthService
{
    Task<SignInResponseDto> HandleGoogleLoginAsync(
        ClaimsPrincipal principal);
}