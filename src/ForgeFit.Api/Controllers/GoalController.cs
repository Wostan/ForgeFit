using System.Security.Claims;
using ForgeFit.Application.Common.Exceptions;
using ForgeFit.Application.Common.Interfaces.Services;
using ForgeFit.Application.DTOs.Goal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ForgeFit.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GoalController(IGoalService goalService) : ControllerBase
{
    [Authorize]
    [HttpGet("body")]
    [ProducesResponseType(typeof(BodyGoalDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetBodyGoal()
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var goal = await goalService.GetBodyGoalAsync(userId);
            return Ok(goal);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
    
    [Authorize]
    [HttpGet("nutrition")]
    [ProducesResponseType(typeof(NutritionGoalDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetNutritionGoal()
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var goal = await goalService.GetNutritionGoalAsync(userId);
            return Ok(goal);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
    
    [Authorize]
    [HttpGet("workout")]
    [ProducesResponseType(typeof(WorkoutGoalDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetWorkoutGoal()
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var goal = await goalService.GetWorkoutGoalAsync(userId);
            return Ok(goal);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
    
    [Authorize]
    [HttpPut("body")]
    [ProducesResponseType(typeof(BodyGoalDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateBodyGoal([FromBody] BodyGoalDto goal)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var updatedGoal = await goalService.UpdateBodyGoalAsync(userId, goal);
            return Ok(updatedGoal);
        }
        catch (BadRequestException e)
        {
            return BadRequest(e.Message);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
    
    [Authorize]
    [HttpPut("nutrition")]
    [ProducesResponseType(typeof(NutritionGoalDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateNutritionGoal([FromBody] NutritionGoalDto goal)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var updatedGoal = await goalService.UpdateNutritionGoalAsync(userId, goal);
            return Ok(updatedGoal);
        }
        catch (BadRequestException e)
        {
            return BadRequest(e.Message);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
    
    [Authorize]
    [HttpPut("workout")]
    [ProducesResponseType(typeof(WorkoutGoalDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateWorkoutGoal([FromBody] WorkoutGoalDto goal)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var updatedGoal = await goalService.UpdateWorkoutGoalAsync(userId, goal);
            return Ok(updatedGoal);
        }
        catch (BadRequestException e)
        {
            return BadRequest(e.Message);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
}