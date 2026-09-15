using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using PrimeKare.Api.Services;
using PrimeKare.Api.DTOs.Auth;
using PrimeKare.Api.Services.Exceptions;

namespace PrimeKare.Api.Controllers;

[ApiController]
[Route("api/external-auth")]
public class ExternalAuthController : ControllerBase
{
    private readonly IExternalAuthService _externalAuthService;
    private readonly IConfiguration _configuration;

    public ExternalAuthController(
        IExternalAuthService externalAuthService,
        IConfiguration configuration)
    {
        _externalAuthService = externalAuthService;
        _configuration = configuration;
    }

    [HttpGet("google")]
    public IActionResult GoogleLogin()
    {
        var properties = new AuthenticationProperties
        {
            RedirectUri = "/api/external-auth/google/callback"
        };

        return Challenge(
            properties,
            GoogleDefaults.AuthenticationScheme
        );
    }

    [HttpGet("google/callback")]
    public async Task<IActionResult> GoogleCallback()
    {
        var result = await HttpContext.AuthenticateAsync(
            GoogleDefaults.AuthenticationScheme);

        if (!result.Succeeded ||
            result.Principal == null)
        {
            return Unauthorized(new
            {
                message =
                    "Google authentication failed."
            });
        }

        try
        {
            var authResult =
                await _externalAuthService.HandleGoogleLoginAsync(
                    result.Principal);

            var frontendUrl = _configuration["FrontendUrl"];

            if (string.IsNullOrWhiteSpace(frontendUrl))
            {
                throw new InvalidOperationException(
                    "FrontendUrl is not configured.");
            }

            return Redirect(
                $"{frontendUrl}/auth/google/callback" +
                $"?code={Uri.EscapeDataString(authResult.Code)}" +
                $"&type={Uri.EscapeDataString(authResult.Type)}"
            );
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new
            {
                message = ex.Message
            });
        }
    }

    [HttpPost("exchange")]
    public async Task<IActionResult> ExchangeCode(
    [FromBody] string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return BadRequest(new
            {
                message =
                    "Authorization code is required."
            });
        }

        try
        {
            var response =
                await _externalAuthService
                    .ExchangeCodeAsync(code);

            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    [HttpPost("link/verify")]
    public async Task<IActionResult>
    VerifyAndLinkGoogle(
        [FromBody]
        VerifyExternalLinkDto request)
    {
        if (string.IsNullOrWhiteSpace(
                request.Code) ||
            string.IsNullOrWhiteSpace(
                request.Password))
        {
            return BadRequest(new
            {
                message =
                    "Authorization code and password are required."
            });
        }

        try
        {
            var response =
                await _externalAuthService
                    .VerifyAndLinkGoogleAsync(
                        request.Code,
                        request.Password);

            return Ok(response);
        }
        catch (ConflictException ex)
        {
            return Conflict(new
            {
                message = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }
}