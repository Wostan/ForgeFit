using ForgeFit.Application.DTOs.Food;

namespace ForgeFit.Application.Common.Interfaces.Services;

public interface IFoodApiService
{
    Task<List<FoodItemDto>> SearchByQueryAsync(string query, int pageNumber = 1, int pageSize = 20);
    Task<List<FoodItemDto>> SearchByBarcodeAsync(string barcode, int pageNumber = 1, int pageSize = 20);
    Task<List<FoodItemDto>> RecognizeByPhotoAsync(string imageBase64);
    Task<FoodItemDto> GetByIdAsync(string id);
}