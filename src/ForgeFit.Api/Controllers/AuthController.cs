using ForgeFit.Application.Common.Exceptions;
using ForgeFit.Application.Common.Exceptions.AuthExceptions;
using ForgeFit.Application.Common.Interfaces.Services;
using ForgeFit.Application.DTOs.Auth;
using Microsoft.AspNetCore.Mvc;

namespace ForgeFit.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("sign-up")]
    [ProducesResponseType(typeof(UserSignUpResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SignUp([FromBody] UserSignUpRequest request)
    {
        try
        {
            var response = await authService.SignUpAsync(request);
            return CreatedAtAction(nameof(SignUp), new { request.Email }, response);
        }
        catch (EmailAlreadyExistsException e)
        {
            return Conflict(e.Message);
        }
        catch (BadRequestException e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpPost("sign-in")]
    [ProducesResponseType(typeof(UserSignInResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SignIn([FromBody] UserSignInRequest request)
    {
        try
        {
            var response = await authService.SignInAsync(request);
            return Ok(response);
        }
        catch (InvalidCredentialsException e)
        {
            return Unauthorized(e.Message);
        }
        catch (BadRequestException e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpPost("check-email")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public async Task<IActionResult> CheckEmail([FromBody] CheckEmailRequest request)
    {
        try
        {
            return Ok(await authService.CheckEmailAsync(request));
        }
        catch (BadRequestException e)
        {
            return BadRequest(e.Message);
        }
    }
}