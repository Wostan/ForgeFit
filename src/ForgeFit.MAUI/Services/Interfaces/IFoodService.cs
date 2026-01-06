using ForgeFit.MAUI.Models;
using ForgeFit.MAUI.Models.DTOs.Food;

namespace ForgeFit.MAUI.Services.Interfaces;

public interface IFoodService
{
    Task<ServiceResponse<List<FoodSearchResponse>>> SearchFoodAsync(string query, int pageNumber = 1, int pageSize = 20);
    Task<ServiceResponse<FoodProductResponse>> GetProductByIdAsync(string externalId);
    Task<ServiceResponse<FoodProductResponse>> GetProductByBarcodeAsync(string barcode);
    Task<ServiceResponse<List<FoodProductResponse>>> RecognizeFoodFromImageAsync(FileResult file);
}
