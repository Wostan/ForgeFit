using System.Security.Claims;
using ForgeFit.Application.Common.Exceptions;
using ForgeFit.Application.Common.Interfaces.Services;
using ForgeFit.Application.DTOs.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ForgeFit.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(IUserService userService) : ControllerBase
{
    [Authorize]
    [HttpGet("get-profile")]
    [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Get()
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var profile = await userService.GetProfileByIdAsync(userId);
            return Ok(profile);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
    
    [Authorize]
    [HttpPut("update-profile")]
    [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Update([FromBody] UserProfileDto profile)
    {
        try
        { 
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var updatedProfile = await userService.UpdateProfileByIdAsync(userId, profile);
            return Ok(updatedProfile);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
    
    [Authorize]
    [HttpPut("change-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest password)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await userService.ChangePasswordByIdAsync(userId, password);
            return Ok("Password changed.");
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
}