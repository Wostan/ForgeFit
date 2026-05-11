using System.Security.Claims;
using ForgeFit.Application.Common.Interfaces.Services;
using ForgeFit.Application.DTOs.Food;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ForgeFit.Api.Controllers;

[ApiController]
[Route("api/custom-food")]
public class CustomFoodController(ICustomFoodService customFoodService) : ControllerBase
{
    [Authorize]
    [HttpGet]
    [ProducesResponseType(typeof(List<CustomFoodDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<CustomFoodDto>>> GetAllAsync()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await customFoodService.GetAllForUserAsync(userId);

        return Ok(result);
    }

    [Authorize]
    [HttpGet("{id:guid}", Name = "GetCustomFood")]
    [ProducesResponseType(typeof(CustomFoodDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomFoodDto>> GetByIdAsync(Guid id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await customFoodService.GetByIdAsync(userId, id);

        return Ok(result);
    }

    [Authorize]
    [HttpPost]
    [ProducesResponseType(typeof(CustomFoodDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<CustomFoodDto>> CreateAsync([FromBody] CustomFoodCreateRequest request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await customFoodService.CreateAsync(userId, request);

        return CreatedAtRoute("GetCustomFood", new { id = result.Id }, result);
    }

    [Authorize]
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(CustomFoodDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomFoodDto>> UpdateAsync(
        Guid id,
        [FromBody] CustomFoodUpdateRequest request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await customFoodService.UpdateAsync(userId, id, request);

        return Ok(result);
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteAsync(Guid id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await customFoodService.DeleteAsync(userId, id);

        return NoContent();
    }
}
