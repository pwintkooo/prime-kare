using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using PrimeKare.Api.Services;

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

        if (!result.Succeeded || result.Principal == null)
        {
            return Unauthorized();
        }

        try
        {
            var code =
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
                $"?code={Uri.EscapeDataString(code)}"
            );
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
    }

    [HttpPost("exchange")]
    public async Task<IActionResult> ExchangeCode(
        [FromBody] string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            return BadRequest(
                "Authorization code is required.");

        var response =
            await _externalAuthService.ExchangeCodeAsync(code);

        if (response == null)
        {
            return Unauthorized(
                "Invalid or expired authorization code.");
        }

        return Ok(response);
    }
}