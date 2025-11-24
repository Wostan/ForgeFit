using ForgeFit.Application.Common.Interfaces.Services;
using ForgeFit.Application.Common.Interfaces.Services.InfrastructureServices;
using ForgeFit.Application.DTOs.Food;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ForgeFit.Api.Controllers;

[Route("api/[controller]")]
public class FoodApiController(IFoodApiService foodApiService) : ControllerBase
{
    [Authorize]
    [HttpGet("search")]
    [ProducesResponseType(typeof(List<FoodProductResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<List<FoodProductResponse>>> SearchByQueryAsync(
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

    [Authorize]
    [HttpGet("barcode")]
    [ProducesResponseType(typeof(List<FoodProductResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<FoodProductResponse>>> SearchByBarcodeAsync([FromQuery] string barcode)
    {
        var result = await foodApiService.SearchByBarcodeAsync(barcode);
        
        return Ok(result);
    }

    [Authorize]
    [HttpPost("by-photo")]
    [ProducesResponseType(typeof(List<FoodProductResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<List<FoodProductResponse>>> RecognizeByPhotoAsync([FromBody] RecognizeByPhotoRequest request)
    {
        var result = await foodApiService.RecognizeByPhotoAsync(request.ImageBase64);
        
        return Ok(result);
    }
    
    [Authorize]
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(FoodProductResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FoodProductResponse>> GetByIdAsync(string id)
    {
        var result = await foodApiService.GetByIdAsync(id);
        
        return Ok(result);
    }
}