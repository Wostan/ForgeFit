using ForgeFit.Application.DTOs.Food;

namespace ForgeFit.Application.Common.Interfaces.Services.InfrastructureServices;

public interface IFoodApiService
{
    Task<List<FoodSearchDto>> SearchByQueryAsync(string query, int pageNumber = 1, int pageSize = 20);
    Task<FoodProductDto> SearchByBarcodeAsync(string barcode);
    Task<List<FoodProductDto>> RecognizeByPhotoAsync(string imageBase64);
    Task<FoodProductDto> GetByIdAsync(string id);
}