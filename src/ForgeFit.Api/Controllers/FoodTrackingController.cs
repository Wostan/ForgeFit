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
    public async Task<ActionResult<FoodEntryDto>> LogEntryAsync([FromBody] FoodEntryCreateRequest entryDto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await foodTrackingService.LogFoodEntryAsync(userId, entryDto);
        
        return CreatedAtRoute("GetFoodEntry", new { entryId = result.Id }, result);
    }
    
    [Authorize]
    [HttpPost("drink")]
    [ProducesResponseType(typeof(DrinkEntryResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<DrinkEntryResponse>> LogDrinkEntryAsync(DrinkEntryCreateRequest entryDto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await foodTrackingService.LogDrinkEntryAsync(userId, entryDto.VolumeMl);
        
        return CreatedAtRoute("GetDrinkEntry", new { entryId = result.Id }, result);
    }
    
    [Authorize]
    [HttpPut("{entryId:guid}")]
    [ProducesResponseType(typeof(FoodEntryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FoodEntryDto>> UpdateEntryAsync(
        Guid entryId,
        [FromBody] FoodEntryCreateRequest entryDto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await foodTrackingService.UpdateFoodEntryAsync(userId, entryId, entryDto);
        
        return Ok(result);
    }
    
    [Authorize]
    [HttpPut("drink/{entryId:guid}")]
    [ProducesResponseType(typeof(DrinkEntryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DrinkEntryResponse>> UpdateDrinkEntryAsync(
        Guid entryId,
        [FromBody] DrinkEntryCreateRequest entryDto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await foodTrackingService.UpdateDrinkEntryAsync(userId, entryId, entryDto.VolumeMl);
        
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
        await foodTrackingService.DeleteFoodEntryAsync(userId, entryId);
        
        return NoContent();
    }
    
    [Authorize]
    [HttpDelete("drink/{entryId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteDrinkEntryAsync(Guid entryId)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await foodTrackingService.DeleteDrinkEntryAsync(userId, entryId);
        
        return NoContent();
    }
    
    [Authorize]
    [HttpGet("{entryId:guid}", Name = "GetFoodEntry")]
    [ProducesResponseType(typeof(FoodEntryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FoodEntryDto>> GetEntryAsync(Guid entryId)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await foodTrackingService.GetFoodEntryAsync(userId, entryId);
        
        return Ok(result);
    }
    
    [Authorize]
    [HttpGet("drink/{entryId:guid}", Name = "GetDrinkEntry")]
    [ProducesResponseType(typeof(DrinkEntryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DrinkEntryResponse>> GetDrinkEntryAsync(Guid entryId)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await foodTrackingService.GetDrinkEntryAsync(userId, entryId);
        
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
            return Ok(await foodTrackingService.GetFoodEntriesByDateAsync(userId, date.Value));
        if (from.HasValue && to.HasValue)
            return Ok(await foodTrackingService.GetFoodEntriesByDateAsync(userId, from.Value, to.Value));
        
        return Ok(await foodTrackingService.GetFoodEntriesByDateAsync(userId, DateTime.Today));
    }
    
    [Authorize]
    [HttpGet("drink/by-date")]
    [ProducesResponseType(typeof(List<DrinkEntryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<DrinkEntryResponse>>> GetDrinkEntriesByDateAsync(
        [FromQuery] DateTime? date,
        [FromQuery] DateTime? from, 
        [FromQuery] DateTime? to)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        if (date.HasValue)
            return Ok(await foodTrackingService.GetDrinkEntriesByDateAsync(userId, date.Value));
        if (from.HasValue && to.HasValue)
            return Ok(await foodTrackingService.GetDrinkEntriesByDateAsync(userId, from.Value, to.Value));
        
        return Ok(await foodTrackingService.GetDrinkEntriesByDateAsync(userId, DateTime.Today));
    }
}