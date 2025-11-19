using System.Security.Claims;
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
    public async Task<ActionResult<BodyGoalDto>> GetBodyGoal()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var goal = await goalService.GetBodyGoalAsync(userId);
        
        return Ok(goal);
    }
    
    [Authorize]
    [HttpGet("nutrition")]
    [ProducesResponseType(typeof(NutritionGoalDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<NutritionGoalDto>> GetNutritionGoal()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var goal = await goalService.GetNutritionGoalAsync(userId);
        
        return Ok(goal);
    }
    
    [Authorize]
    [HttpGet("workout")]
    [ProducesResponseType(typeof(WorkoutGoalDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<WorkoutGoalDto>> GetWorkoutGoal()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var goal = await goalService.GetWorkoutGoalAsync(userId);
        
        return Ok(goal);
    }
    
    [Authorize]
    [HttpPut("body")]
    [ProducesResponseType(typeof(BodyGoalDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<BodyGoalDto>> UpdateBodyGoal([FromBody] BodyGoalDto goal)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var updatedGoal = await goalService.UpdateBodyGoalAsync(userId, goal);
        
        return Ok(updatedGoal);
    }
    
    [Authorize]
    [HttpPut("nutrition")]
    [ProducesResponseType(typeof(NutritionGoalDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<NutritionGoalDto>> UpdateNutritionGoal([FromBody] NutritionGoalDto goal)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var updatedGoal = await goalService.UpdateNutritionGoalAsync(userId, goal);
        
        return Ok(updatedGoal);
    }
    
    [Authorize]
    [HttpPut("workout")]
    [ProducesResponseType(typeof(WorkoutGoalDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<WorkoutGoalDto>> UpdateWorkoutGoal([FromBody] WorkoutGoalDto goal)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var updatedGoal = await goalService.UpdateWorkoutGoalAsync(userId, goal);
        
        return Ok(updatedGoal);
    }
}