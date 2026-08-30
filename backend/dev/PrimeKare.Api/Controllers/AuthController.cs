using Microsoft.AspNetCore.Mvc;
using PrimeKare.Api.DTOs.Auth;
using PrimeKare.Api.Services;

namespace PrimeKare.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("sign-up")]
    public async Task<IActionResult> SignUp(
        SignUpDto request)
    {
        try
        {
            var result = await _authService
                .SignUpAsync(request);

            return Created("", result);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    [HttpPost("sign-in")]
    public async Task<IActionResult> SignIn(
        SignInDto request)
    {
        var token = await _authService
            .SignInAsync(request);

        if (token == null)
        {
            return Unauthorized(
                "Invalid email or password.");
        }

        return Ok(new
        {
            token
        });
    }
}