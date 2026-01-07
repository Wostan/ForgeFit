using ForgeFit.MAUI.Models;
using ForgeFit.MAUI.Models.DTOs.Food;
using ForgeFit.MAUI.Services.Interfaces;

namespace ForgeFit.MAUI.Services;

public class FoodService(IApiService apiService) : IFoodService
{
    public async Task<ServiceResponse<List<FoodSearchResponse>>> SearchFoodAsync(string query, int pageNumber = 1,
        int pageSize = 20)
    {
        var url =
            $"/api/food-api/search?query={Uri.EscapeDataString(query)}&pageNumber={pageNumber}&pageSize={pageSize}";
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

    public async Task<ServiceResponse<List<FoodProductResponse>>> RecognizeFoodFromImageAsync(FileResult file)
    {
        try
        {
            string base64String;

            await using (var stream = await file.OpenReadAsync())
            {
                using (var memoryStream = new MemoryStream())
                {
                    await stream.CopyToAsync(memoryStream);
                    var imageBytes = memoryStream.ToArray();

                    base64String = Convert.ToBase64String(imageBytes);
                }
            }

            var requestDto = new RecognizeByPhotoRequest(base64String);

            return await apiService.PostAsync<RecognizeByPhotoRequest, List<FoodProductResponse>>(
                "/api/food-api/by-photo",
                requestDto
            );
        }
        catch (Exception ex)
        {
            return ServiceResponse<List<FoodProductResponse>>.Error($"Error processing image: {ex.Message}");
        }
    }
}
