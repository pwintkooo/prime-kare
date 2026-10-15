using PrimeKare.Api.DTOs.Auth;

namespace PrimeKare.Api.DTOs.Auth;

public sealed class AuthSessionResult
{
    public required SignInResponseDto Response { get; init; }

    public required string RefreshToken { get; init; }
}
