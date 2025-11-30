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
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserSignUpResponse>> SignUp([FromBody] UserSignUpRequest request)
    {
        try
        {
            var response = await authService.SignUpAsync(request);
            return Created(string.Empty, response);
        }
        catch (EmailAlreadyExistsException e)
        {
            return Conflict(e.Message);
        }
    }
    
    [HttpPost("sign-in")]
    [ProducesResponseType(typeof(UserSignInResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserSignInResponse>> SignIn([FromBody] UserSignInRequest request)
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
    }
    
    [HttpPost("check-email")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CheckEmailResponse>> CheckEmail([FromBody] CheckEmailRequest request)
    {
        return Ok(await authService.CheckEmailAsync(request));
    }
    
    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(UserSignInResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserSignInResponse>> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        try
        {
            var response = await authService.RefreshTokenAsync(request);
            return Ok(response);
        }
        catch (InvalidCredentialsException e)
        {
            return Unauthorized(e.Message);
        }
    }
}