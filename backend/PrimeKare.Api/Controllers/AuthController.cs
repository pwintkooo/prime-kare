using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using PrimeKare.Api.DTOs.Auth;
using PrimeKare.Api.Services;
using PrimeKare.Api.Services.Exceptions;

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
            return Conflict(new
            {
                message = ex.Message
            });
        }
    }

    [HttpPost("sign-in")]
    public async Task<IActionResult> SignIn(
        SignInDto request)
    {

        try
        {
            var response = await _authService.SignInAsync(request);

            return Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
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
        catch (AccountInactiveException ex)
        {
            return BadRequest(new
            {
                message = ex.Message,
                code = "ACCOUNT_INACTIVE"
            });
        }
    }

    [HttpPost("reactivate")]
    public async Task<ActionResult<SignInResponseDto>>
    ReactivateAccount(
        ReactivateAccountDto request)
    {
        try
        {
            var response =
                await _authService
                    .ReactivateAccountAsync(
                        request
                    );

            return Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
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