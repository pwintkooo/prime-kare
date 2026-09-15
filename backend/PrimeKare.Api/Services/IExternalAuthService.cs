using System.Security.Claims;
using PrimeKare.Api.DTOs.Auth;

namespace PrimeKare.Api.Services;

public interface IExternalAuthService
{
    Task<ExternalAuthResult> HandleGoogleLoginAsync(
        ClaimsPrincipal principal);

    Task<SignInResponseDto> ExchangeCodeAsync(
        string code);

    Task<SignInResponseDto> VerifyAndLinkGoogleAsync(
        string code,
        string password
    );
}