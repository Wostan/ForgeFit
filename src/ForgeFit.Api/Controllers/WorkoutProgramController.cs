using System.Security.Claims;
using ForgeFit.Application.Common.Interfaces.Services;
using ForgeFit.Application.DTOs.Workout;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ForgeFit.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkoutProgramController(IWorkoutProgramService workoutProgramService) : ControllerBase
{
    [Authorize]
    [HttpPost]
    [ProducesResponseType(typeof(WorkoutProgramResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<WorkoutProgramResponse>> CreateWorkoutProgramAsync([FromBody] WorkoutProgramRequest workoutProgramRequest)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await workoutProgramService.CreateWorkoutProgramAsync(userId, workoutProgramRequest);
        
        return CreatedAtAction(nameof(GetWorkoutProgramAsync), new { workoutProgramId = result.Id }, result);
    }
    
    [Authorize]
    [HttpPut("{programId:guid}")]
    [ProducesResponseType(typeof(WorkoutProgramResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<WorkoutProgramResponse>> UpdateWorkoutProgramAsync(
        Guid programId, 
        [FromBody] WorkoutProgramRequest workoutProgramRequest)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await workoutProgramService.UpdateWorkoutProgramAsync(userId, programId, workoutProgramRequest);
        
        return Ok(result);
    }
    
    [Authorize]
    [HttpDelete("{workoutProgramId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> DeleteWorkoutProgramAsync(Guid workoutProgramId)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await workoutProgramService.DeleteWorkoutProgramAsync(userId, workoutProgramId);
        
        return NoContent();
    }
    
    [Authorize]
    [HttpGet]
    [ProducesResponseType(typeof(List<WorkoutProgramResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<WorkoutProgramResponse>>> GetAllWorkoutProgramsAsync()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await workoutProgramService.GetAllWorkoutProgramsAsync(userId);
        
        return Ok(result);
    }
    
    [Authorize]
    [HttpGet("{workoutProgramId:guid}")]
    [ProducesResponseType(typeof(WorkoutProgramResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WorkoutProgramResponse>> GetWorkoutProgramAsync(Guid workoutProgramId)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await workoutProgramService.GetWorkoutProgramAsync(userId, workoutProgramId);
        
        return Ok(result);
    }
}