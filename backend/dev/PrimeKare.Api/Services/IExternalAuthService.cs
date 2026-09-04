using System.Security.Claims;
using PrimeKare.Api.DTOs.Auth;

namespace PrimeKare.Api.Services;

public interface IExternalAuthService
{
    Task<string> HandleGoogleLoginAsync(
        ClaimsPrincipal principal);

    Task<SignInResponseDto?> ExchangeCodeAsync(
        string code);
}