using System.Security.Claims;
using ForgeFit.Application.Common.Interfaces.Services;
using ForgeFit.Application.DTOs.Food;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ForgeFit.Api.Controllers;

[ApiController]
[Route("api/[controller]/entries/")]
public class DrinkTrackingController(IDrinkTrackingService drinkTrackingService) : ControllerBase
{
    [Authorize]
    [HttpPost]
    [ProducesResponseType(typeof(DrinkEntryResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<DrinkEntryResponse>> LogDrinkEntryAsync(DrinkEntryCreateRequest entryDto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await drinkTrackingService.LogDrinkEntryAsync(userId, entryDto);
        
        return CreatedAtRoute("GetDrinkEntry", new { entryId = result.Id }, result);
    }
    
    [Authorize]
    [HttpPut("{entryId:guid}")]
    [ProducesResponseType(typeof(DrinkEntryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DrinkEntryResponse>> UpdateDrinkEntryAsync(
        Guid entryId,
        [FromBody] DrinkEntryCreateRequest entryDto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await drinkTrackingService.UpdateDrinkEntryAsync(userId, entryId, entryDto);
        
        return Ok(result);
    }
    
    [Authorize]
    [HttpDelete("{entryId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteDrinkEntryAsync(Guid entryId)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await drinkTrackingService.DeleteDrinkEntryAsync(userId, entryId);
        
        return NoContent();
    }
    
    [Authorize]
    [HttpGet("{entryId:guid}", Name = "GetDrinkEntry")]
    [ProducesResponseType(typeof(DrinkEntryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DrinkEntryResponse>> GetDrinkEntryAsync(Guid entryId)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await drinkTrackingService.GetDrinkEntryAsync(userId, entryId);
        
        return Ok(result);
    }
    
    [Authorize]
    [HttpGet("by-date")]
    [ProducesResponseType(typeof(List<DrinkEntryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<DrinkEntryResponse>>> GetDrinkEntriesByDateAsync(
        [FromQuery] DateTime? date,
        [FromQuery] DateTime? from, 
        [FromQuery] DateTime? to)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        if (date.HasValue)
            return Ok(await drinkTrackingService.GetDrinkEntriesByDateAsync(userId, date.Value));
        if (from.HasValue && to.HasValue)
            return Ok(await drinkTrackingService.GetDrinkEntriesByDateAsync(userId, from.Value, to.Value));
        
        return Ok(await drinkTrackingService.GetDrinkEntriesByDateAsync(userId, DateTime.Today));
    }
}