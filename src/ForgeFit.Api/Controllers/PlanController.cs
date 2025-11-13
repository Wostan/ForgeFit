using System.Security.Claims;
using ForgeFit.Application.Common.Exceptions;
using ForgeFit.Application.Common.Interfaces.Services;
using ForgeFit.Application.DTOs.Plan;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ForgeFit.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlanController(IPlanService planService) : ControllerBase
{
    [Authorize]
    [HttpPost("generate")]
    [ProducesResponseType(typeof(PlanDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Generate([FromBody] GeneratePlanRequest request)
    {
        try
        {
            var plan = await planService.GeneratePlanAsync(request);
            return Ok(plan);
        }
        catch (BadRequestException e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize]
    [HttpPost("confirm")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Confirm([FromBody] PlanDto plan)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await planService.ConfirmPlanAsync(userId, plan);
            return Ok("Plan saved.");
        }
        catch (BadRequestException e)
        {
            return BadRequest(e.Message);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }

    [Authorize]
    [HttpGet("get")]
    [ProducesResponseType(typeof(PlanDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Get()
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var plan = await planService.GetPlanAsync(userId);
            return Ok(plan);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpPut("update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Update([FromBody] PlanDto plan)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await planService.UpdatePlanAsync(userId, plan);
            return Ok("Plan updated.");
        }
        catch (BadRequestException e)
        {
            return BadRequest(e.Message);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
}