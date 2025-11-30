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
        
        return CreatedAtRoute("GetWorkoutEntry", new { entryId = result.Id }, result);
    }
    
    [Authorize]
    [HttpPut("{entryId:guid}")]
    [ProducesResponseType(typeof(WorkoutEntryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WorkoutEntryDto>> UpdateEntryAsync(
        Guid entryId,
        [FromBody] WorkoutEntryDto entryDto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await workoutTrackingService.UpdateEntryAsync(userId, entryId, entryDto);
        
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
    [HttpGet("{entryId:guid}", Name = "GetWorkoutEntry")]
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
    [HttpGet("by-date")]
    [ProducesResponseType(typeof(List<WorkoutEntryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<WorkoutEntryDto>>> GetEntriesByDateAsync(
        [FromQuery] DateTime? date,
        [FromQuery] DateTime? from, 
        [FromQuery] DateTime? to)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        if (date.HasValue)
            return Ok(await workoutTrackingService.GetEntriesByDateAsync(userId, date.Value));
        if (from.HasValue && to.HasValue)
            return Ok(await workoutTrackingService.GetEntriesByDateAsync(userId, from.Value, to.Value));
        
        return Ok(await workoutTrackingService.GetEntriesByDateAsync(userId, DateTime.Today));
    }
}