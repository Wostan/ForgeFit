using System.Security.Claims;
using ForgeFit.Application.Common.Interfaces.Services;
using ForgeFit.Application.DTOs.Food;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ForgeFit.Api.Controllers;

[ApiController]
[Route("api/[controller]/entries")]
public class FoodTrackingController(IFoodTrackingService foodTrackingService) : ControllerBase
{
    [Authorize]
    [HttpPost]
    [ProducesResponseType(typeof(FoodEntryDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<FoodEntryDto>> LogEntryAsync([FromBody] FoodEntryDto entryDto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await foodTrackingService.LogEntryAsync(userId, entryDto);
        
        return CreatedAtAction(nameof(LogEntryAsync), new { entryId = result.Id }, result);
    }
    
    [Authorize]
    [HttpPut("{entryId:guid}")]
    [ProducesResponseType(typeof(FoodEntryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FoodEntryDto>> UpdateEntryAsync(
        Guid entryId,
        [FromBody] FoodEntryDto entryDto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await foodTrackingService.UpdateEntryAsync(userId, entryId, entryDto);
        
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
        await foodTrackingService.DeleteEntryAsync(userId, entryId);
        
        return NoContent();
    }
    
    [Authorize]
    [HttpGet("{entryId:guid}")]
    [ProducesResponseType(typeof(FoodEntryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FoodEntryDto>> GetEntryAsync(Guid entryId)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await foodTrackingService.GetEntryAsync(userId, entryId);
        
        return Ok(result);
    }
    
    [Authorize]
    [HttpGet("by-date")]
    [ProducesResponseType(typeof(List<FoodEntryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<FoodEntryDto>>> GetEntriesByDateAsync(
        [FromQuery] DateTime? date,
        [FromQuery] DateTime? from, 
        [FromQuery] DateTime? to)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        if (date.HasValue)
            return Ok(await foodTrackingService.GetEntriesByDateAsync(userId, date.Value));
        if (from.HasValue && to.HasValue)
            return Ok(await foodTrackingService.GetEntriesByDateAsync(userId, from.Value, to.Value));
        
        return Ok(await foodTrackingService.GetEntriesByDateAsync(userId, DateTime.Today));
    }
}