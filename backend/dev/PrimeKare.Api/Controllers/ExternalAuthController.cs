using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using PrimeKare.Api.Data;
using PrimeKare.Api.Services;
using PrimeKare.Api.Models;

namespace PrimeKare.Api.Controllers;

[ApiController]
[Route("api/external-auth")]
public class ExternalAuthController : ControllerBase
{
    private readonly IExternalAuthService _externalAuthService;

    public ExternalAuthController(
        IExternalAuthService ExternalAuthService
    )
    {
        _externalAuthService = ExternalAuthService;
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
            var response =
                await _externalAuthService.HandleGoogleLoginAsync(
                    result.Principal);

            return Ok(response);
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
}