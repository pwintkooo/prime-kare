using PrimeKare.Api.DTOs.Auth;
using PrimeKare.Api.Models;

namespace PrimeKare.Api.Services;

public interface IAuthService
{
    Task<SignUpResponseDto> SignUpAsync(SignUpDto request);

    Task<AuthSessionResult> SignInAsync(SignInDto request);

    Task<AuthSessionResult> CreateSignInSessionAsync(User user);

    SignInResponseDto CreateSignInResponse(User user);

    Task<AuthSessionResult> ReactivateAccountAsync(ReactivateAccountDto request);

    Task ForgotPasswordAsync(ForgotPasswordRequestDto request);

    Task ResetPasswordAsync(ResetPasswordRequestDto request);

    Task<AuthSessionResult> RefreshSessionAsync(string rawRefreshToken);
    
    Task RevokeRefreshTokenAsync(string? rawRefreshToken);
}