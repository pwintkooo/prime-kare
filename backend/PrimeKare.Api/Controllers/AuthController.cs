
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FluentValidation;
using PrimeKare.Api.DTOs.Auth;
using PrimeKare.Api.Services;
using PrimeKare.Api.Services.Exceptions;

namespace PrimeKare.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private const string RefreshCookieName = "primekare_refresh";

    private static readonly TimeSpan RefreshTokenLifetime =
        TimeSpan.FromDays(7);

    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IAuthService authService,
        ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    private void SetRefreshTokenCookie(string refreshToken)
    {
        Response.Cookies.Append(
            RefreshCookieName,
            refreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = Request.IsHttps,
                SameSite = SameSiteMode.Lax,
                Path = "/api",
                Expires = DateTimeOffset.UtcNow.Add(
                    RefreshTokenLifetime)
            });
    }

    private void ClearRefreshTokenCookie()
    {
        Response.Cookies.Delete(
            RefreshCookieName,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = Request.IsHttps,
                SameSite = SameSiteMode.Lax,
                Path = "/api"
            });
    }

    [AllowAnonymous]
    [HttpPost("sign-up")]
    public async Task<IActionResult> SignUp(SignUpDto request)
    {
        try
        {
            var user = await _authService.SignUpAsync(request);

            return Created("", new
            {
                user.Id,
                user.Email,
                user.Role,
                user.Status,
                user.CreatedAt,
                user.CustomerId
            });
        }
        catch (ValidationException ex)
        {
            return BadRequest(new
            {
                message = "Validation failed.",
                errors = ex.Errors.Select(e => e.ErrorMessage)
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [AllowAnonymous]
    [HttpPost("sign-in")]
    public async Task<IActionResult> SignIn(SignInDto request)
    {
        try
        {
            var result = await _authService.SignInAsync(request);

            SetRefreshTokenCookie(result.RefreshToken);

            return Ok(result.Response);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (AccountInactiveException ex)
        {
            return BadRequest(new
            {
                message = ex.Message,
                code = "ACCOUNT_INACTIVE"
            });
        }
    }

    [AllowAnonymous]
    [HttpPost("reactivate")]
    public async Task<ActionResult<SignInResponseDto>>
        ReactivateAccount(ReactivateAccountDto request)
    {
        try
        {
            var result =
                await _authService.ReactivateAccountAsync(request);

            SetRefreshTokenCookie(result.RefreshToken);

            return Ok(result.Response);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        var refreshToken = Request.Cookies[RefreshCookieName];

        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            ClearRefreshTokenCookie();

            return Unauthorized(new
            {
                message = "Your session has expired. Please sign in again."
            });
        }

        try
        {
            var result =
                await _authService.RefreshSessionAsync(refreshToken);

            SetRefreshTokenCookie(result.RefreshToken);

            return Ok(result.Response);
        }
        catch (UnauthorizedAccessException)
        {
            ClearRefreshTokenCookie();

            return Unauthorized(new
            {
                message = "Your session has expired. Please sign in again."
            });
        }
    }

    [AllowAnonymous]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var refreshToken = Request.Cookies[RefreshCookieName];

        await _authService.RevokeRefreshTokenAsync(refreshToken);

        ClearRefreshTokenCookie();

        return Ok(new { message = "You have been signed out." });
    }

    [AllowAnonymous]
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(
        ForgotPasswordRequestDto dto)
    {
        try
        {
            await _authService.ForgotPasswordAsync(dto);

            return Ok(new
            {
                message =
                    "If an account exists for this email, " +
                    "a password reset link has been sent."
            });
        }
        catch (ValidationException ex)
        {
            return BadRequest(new
            {
                message = "Please check the information you entered.",
                errors = ex.Errors.Select(error => new
                {
                    field = error.PropertyName,
                    message = error.ErrorMessage
                })
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to process forgot password request.");

            return StatusCode(500, new
            {
                message =
                    "Unable to process your request. " +
                    "Please try again later."
            });
        }
    }

    [AllowAnonymous]
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(
        ResetPasswordRequestDto dto)
    {
        try
        {
            await _authService.ResetPasswordAsync(dto);

            ClearRefreshTokenCookie();

            return Ok(new
            {
                message =
                    "Your password has been reset successfully."
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to reset password.");

            return StatusCode(500, new
            {
                message =
                    "Unable to reset your password. " +
                    "Please try again later."
            });
        }
    }
}
