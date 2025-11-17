using System.Security.Claims;
using ForgeFit.Application.Common.Interfaces.Services;
using ForgeFit.Application.DTOs.Workout;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ForgeFit.Api.Controllers;

[ApiController]
[Route("api/[controller]/entries")]
public class WorkoutTrackingController(IWorkoutTrackingService workoutTrackingService) : ControllerBase
{
    [Authorize]
    [HttpPost]
    [ProducesResponseType(typeof(WorkoutEntryDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<WorkoutEntryDto>> LogEntryAsync([FromBody] WorkoutEntryDto entryDto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await workoutTrackingService.LogEntryAsync(userId, entryDto);
        
        return CreatedAtAction(nameof(LogEntryAsync), new { entryId = result.Id }, result);
    }
    
    [Authorize]
    [HttpPut]
    [ProducesResponseType(typeof(WorkoutEntryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WorkoutEntryDto>> UpdateEntryAsync([FromBody] WorkoutEntryDto entryDto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await workoutTrackingService.UpdateEntryAsync(userId, entryDto);
        
        return Ok(result);
    }
    
    [Authorize]
    [HttpDelete("{entryId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteEntryAsync(Guid entryId)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await workoutTrackingService.DeleteEntryAsync(userId, entryId);
        
        return NoContent();
    }
    
    [Authorize]
    [HttpGet("{entryId:guid}")]
    [ProducesResponseType(typeof(WorkoutEntryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WorkoutEntryDto>> GetEntryAsync(Guid entryId)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await workoutTrackingService.GetEntryAsync(userId, entryId);
        
        return Ok(result);
    }
    
    [Authorize]
    [HttpGet("by-date/{date:datetime}")]
    [ProducesResponseType(typeof(List<WorkoutEntryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<WorkoutEntryDto>>> GetEntriesByDateAsync(DateTime date)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await workoutTrackingService.GetEntriesByDateAsync(userId, date);
        
        return Ok(result);
    }
    
    [Authorize]
    [HttpGet("by-date")]
    [ProducesResponseType(typeof(List<WorkoutEntryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<WorkoutEntryDto>>> GetEntriesByDateAsync(
        [FromQuery] DateTime from, 
        [FromQuery] DateTime to)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await workoutTrackingService.GetEntriesByDateAsync(
            userId,
            from,
            to);
        
        return Ok(result);
    }
}