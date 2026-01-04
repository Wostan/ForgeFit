using ForgeFit.MAUI.Models;
using ForgeFit.MAUI.Models.DTOs.Food;
using ForgeFit.MAUI.Services.Interfaces;

namespace ForgeFit.MAUI.Services;

public class FoodService(IApiService apiService) : IFoodService
{
    public async Task<ServiceResponse<List<FoodSearchResponse>>> SearchFoodAsync(string query, int pageNumber = 1, int pageSize = 20)
    {
        var url = $"/api/food-api/search?query={Uri.EscapeDataString(query)}&pageNumber={pageNumber}&pageSize={pageSize}";
        return await apiService.GetAsync<List<FoodSearchResponse>>(url);
    }

    public async Task<ServiceResponse<FoodProductResponse>> GetProductByIdAsync(string id)
    {
        return await apiService.GetAsync<FoodProductResponse>($"/api/food-api/{id}");
    }

    public async Task<ServiceResponse<FoodProductResponse>> GetProductByBarcodeAsync(string barcode)
    {
        return await apiService.GetAsync<FoodProductResponse>($"/api/food-api/barcode?barcode={barcode}");
    }
}
