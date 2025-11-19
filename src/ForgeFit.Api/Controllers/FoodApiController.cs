using ForgeFit.Application.DTOs.Food;
using ForgeFit.Infrastructure.Interfaces;
using ForgeFit.Infrastructure.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace ForgeFit.Api.Controllers;

[Route("api/[controller]")]
public class FoodApiController(IFoodApiService foodApiService) : ControllerBase
{
    [HttpGet("search")]
    [ProducesResponseType(typeof(List<FoodItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<FoodItemDto>>> SearchByQueryAsync(
        [FromQuery] string query,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await foodApiService.SearchByQueryAsync(
            query,
            pageNumber,
            pageSize);
        
        return Ok(result);
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
        var result = await foodApiService.SearchByBarcodeAsync(
            barcode,
            pageNumber,
            pageSize);
        
        return Ok(result);
    }

    [HttpPost("by-photo")]
    [ProducesResponseType(typeof(List<FoodItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<List<FoodItemDto>>> RecognizeByPhotoAsync([FromBody] RecognizeByPhotoRequest request)
    {
        var result = await foodApiService.RecognizeByPhotoAsync(request.ImageBase64);
        
        return Ok(result);
    }
    
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(FoodItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FoodItemDto>> GetByIdAsync(string id)
    {
        var result = await foodApiService.GetByIdAsync(id);
        
        return Ok(result);
    }
}