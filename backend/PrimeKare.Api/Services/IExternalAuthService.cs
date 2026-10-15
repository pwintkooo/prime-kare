using System.Security.Claims;
using PrimeKare.Api.DTOs.Auth;

namespace PrimeKare.Api.Services;

public interface IExternalAuthService
{
    Task<ExternalAuthResult> HandleGoogleLoginAsync(
        ClaimsPrincipal principal);

    Task<AuthSessionResult> ExchangeCodeAsync(
        string code);

    Task<AuthSessionResult> VerifyAndLinkGoogleAsync(
        string code,
        string password
    );

    Task<AuthSessionResult> ReactivateAccountAsync(
    string code
);
}