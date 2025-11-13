using ForgeFit.Application.Common.Exceptions;
using ForgeFit.Application.Common.Interfaces.Services;
using ForgeFit.Application.DTOs.Food;
using Microsoft.AspNetCore.Mvc;

namespace ForgeFit.Api.Controllers;

[Route("api/[controller]")]
public class FoodApiController(IFoodApiService foodApiService) : ControllerBase
{
    [HttpGet("search/{query}")]
    [ProducesResponseType(typeof(List<FoodItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<FoodItemDto>>> SearchByQueryAsync(
        string query,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            var result = await foodApiService.SearchByQueryAsync(
                query,
                pageNumber,
                pageSize);
            return Ok(result);
        }
        catch (BadRequestException e)
        {
            return BadRequest(e.Message);
        }
        catch (ServiceUnavailableException e)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, e.Message);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpGet("barcode/{barcode}")]
    [ProducesResponseType(typeof(List<FoodItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<FoodItemDto>>> SearchByBarcodeAsync(
        string barcode,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            var result = await foodApiService.SearchByBarcodeAsync(
                barcode,
                pageNumber,
                pageSize);
            return Ok(result);
        }
        catch (BadRequestException e)
        {
            return BadRequest(e.Message);
        }
        catch (ServiceUnavailableException e)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, e.Message);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpPost("by-photo")]
    [ProducesResponseType(typeof(List<FoodItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<List<FoodItemDto>>> RecognizeByPhotoAsync([FromBody] RecognizeByPhotoRequest request)
    {
        try
        {
            var result = await foodApiService.RecognizeByPhotoAsync(request.ImageBase64);
            return Ok(result);
        }
        catch (BadRequestException e)
        {
            return BadRequest(e.Message);
        }
        catch (ServiceUnavailableException e)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, e.Message);
        }
    }
    
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(FoodItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FoodItemDto>> GetByIdAsync(string id)
    {
        try
        {
            var result = await foodApiService.GetByIdAsync(id);
            return Ok(result);
        }
        catch (BadRequestException e)
        {
            return BadRequest(e.Message);
        }
        catch (ServiceUnavailableException e)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, e.Message);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
}