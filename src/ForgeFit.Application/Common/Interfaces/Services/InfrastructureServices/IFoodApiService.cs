using ForgeFit.Application.DTOs.Food;

namespace ForgeFit.Application.Common.Interfaces.Services.InfrastructureServices;

public interface IFoodApiService
{
    Task<List<FoodSearchResponse>> SearchByQueryAsync(string query, int pageNumber = 1, int pageSize = 20);
    Task<FoodProductResponse> SearchByBarcodeAsync(string barcode);
    Task<List<FoodProductResponse>> RecognizeByPhotoAsync(string imageBase64);
    Task<FoodProductResponse> GetByIdAsync(string id);
}
