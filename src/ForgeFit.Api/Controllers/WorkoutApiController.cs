using ForgeFit.Application.DTOs.Workout;
using ForgeFit.Domain.Enums.WorkoutEnums;
using ForgeFit.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ForgeFit.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkoutApiController(IWorkoutApiService workoutApiService) : ControllerBase
{
    [HttpGet("search")]
    [ProducesResponseType(typeof(List<WorkoutExerciseSearchDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<WorkoutExerciseSearchDto>>> SearchAsync(
        [FromQuery] string query,
        [FromQuery] List<Muscle>? muscles,
        [FromQuery] List<BodyPart>? bodyParts,
        [FromQuery] List<Equipment>? equipment,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await workoutApiService.SearchAsync(
            query,
            muscles,
            bodyParts,
            equipment,
            pageNumber,
            pageSize);
        
        return Ok(result);
    }
    
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(WorkoutExerciseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WorkoutExerciseDto>> GetByIdAsync(string id)
    {
        var result = await workoutApiService.GetByIdAsync(id);
        
        return Ok(result);
    }
}