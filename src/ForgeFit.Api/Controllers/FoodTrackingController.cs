using System.Security.Claims;
using ForgeFit.Application.Common.Interfaces.Services;
using ForgeFit.Application.DTOs.Food;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ForgeFit.Api.Controllers;

[ApiController]
[Route("api/[controller]/entries")]
public class FoodTrackingController(IFoodService foodTrackingService) : ControllerBase
{
    [Authorize]
    [HttpPost]
    [ProducesResponseType(typeof(FoodEntryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<FoodEntryDto>> LogEntryAsync([FromBody] List<FoodItemDto> foodItems)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await foodTrackingService.LogEntryAsync(userId, foodItems);
        
        return Ok(result);
    }
    
    [Authorize]
    [HttpPut("{entryId:guid}")]
    [ProducesResponseType(typeof(FoodEntryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FoodEntryDto>> UpdateEntryAsync(
        Guid entryId,
        [FromBody] List<FoodItemDto> foodItems)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await foodTrackingService.UpdateEntryAsync(userId, entryId, foodItems);
        
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
    [HttpGet("by-date/{date:datetime}")]
    [ProducesResponseType(typeof(List<FoodEntryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<FoodEntryDto>>> GetEntriesByDateAsync(DateTime date)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await foodTrackingService.GetEntriesByDateAsync(userId, date);
        
        return Ok(result);
    }
}