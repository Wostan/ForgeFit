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
    [ProducesResponseType(typeof(BodyGoalResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<BodyGoalResponse>> GetBodyGoal()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var goal = await goalService.GetBodyGoalAsync(userId);
        
        return Ok(goal);
    }
    
    [Authorize]
    [HttpGet("nutrition")]
    [ProducesResponseType(typeof(NutritionGoalResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<NutritionGoalResponse>> GetNutritionGoal()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var goal = await goalService.GetNutritionGoalAsync(userId);
        
        return Ok(goal);
    }
    
    [Authorize]
    [HttpGet("workout")]
    [ProducesResponseType(typeof(WorkoutGoalResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<WorkoutGoalResponse>> GetWorkoutGoal()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var goal = await goalService.GetWorkoutGoalAsync(userId);
        
        return Ok(goal);
    }
    
    [Authorize]
    [HttpPut("body")]
    [ProducesResponseType(typeof(BodyGoalResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<BodyGoalResponse>> UpdateBodyGoal([FromBody] BodyGoalCreateRequest goal)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var updatedGoal = await goalService.UpdateBodyGoalAsync(userId, goal);
        
        return Ok(updatedGoal);
    }
    
    [Authorize]
    [HttpPut("nutrition")]
    [ProducesResponseType(typeof(NutritionGoalResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<NutritionGoalResponse>> UpdateNutritionGoal([FromBody] NutritionGoalCreateRequest goal)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var updatedGoal = await goalService.UpdateNutritionGoalAsync(userId, goal);
        
        return Ok(updatedGoal);
    }
    
    [Authorize]
    [HttpPut("workout")]
    [ProducesResponseType(typeof(WorkoutGoalResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<WorkoutGoalResponse>> UpdateWorkoutGoal([FromBody] WorkoutGoalCreateRequest goal)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var updatedGoal = await goalService.UpdateWorkoutGoalAsync(userId, goal);
        
        return Ok(updatedGoal);
    }
}