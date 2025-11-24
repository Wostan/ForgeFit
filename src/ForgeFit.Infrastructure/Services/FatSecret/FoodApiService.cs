using System.Globalization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using ForgeFit.Application.Common.Exceptions;
using ForgeFit.Application.Common.Interfaces.Services.InfrastructureServices;
using ForgeFit.Application.DTOs.Food;
using ForgeFit.Infrastructure.Configurations;
using ForgeFit.Infrastructure.Services.FatSecret.Models;
using Microsoft.Extensions.Options;

namespace ForgeFit.Infrastructure.Services.FatSecret;

public class FoodApiService(
    HttpClient httpClient,
    IFatSecretTokenService fatSecretTokenService,
    IOptions<FoodApiSettings> settings) : IFoodApiService
{
    private readonly FoodApiSettings _settings = settings.Value;

    public async Task<List<FoodSearchResponse>> SearchByQueryAsync(string query, int pageNumber = 1, int pageSize = 20)
    {
        var parameters = new Dictionary<string, string>
        {
            { "method", "foods.search" },
            { "search_expression", query },
            { "page_number", (pageNumber - 1).ToString() },
            { "max_results", pageSize.ToString() },
            { "region", _settings.Region },
            { "language", _settings.Language },
            { "format", "json" }
        };

        var response = await ExecuteFatSecretRequestAsync<FatSecretSearchResponse>(parameters);

        return response?.FoodsContainer?.Food?
            .Select(f => new FoodSearchResponse(f.FoodId, f.FoodName, f.BrandName, f.FoodDescription))
            .ToList() ?? [];
    }

    public async Task<FoodProductResponse> SearchByBarcodeAsync(string barcode)
    {
        var parameters = new Dictionary<string, string>
        {
            { "method", "food.find_id_for_barcode.v2" },
            { "barcode", barcode },
            { "region", _settings.Region },
            { "language", _settings.Language },
            { "format", "json" }
        };

        return await GetFoodDetailsAsync(parameters, $"Food with barcode {barcode} not found");
    }

    public async Task<FoodProductResponse> GetByIdAsync(string id)
    {
        var parameters = new Dictionary<string, string>
        {
            { "method", "food.get.v5" },
            { "food_id", id },
            { "region", _settings.Region },
            { "language", _settings.Language },
            { "format", "json" }
        };

        return await GetFoodDetailsAsync(parameters, $"Food with id {id} not found");
    }

    public Task<List<FoodProductResponse>> RecognizeByPhotoAsync(string imageBase64)
    {
        throw new NotImplementedException();
    }

    private async Task<T?> ExecuteFatSecretRequestAsync<T>(Dictionary<string, string> parameters)
    {
        var token = await fatSecretTokenService.GetAccessTokenAsync();
        
        using var requestMessage = new HttpRequestMessage(HttpMethod.Post, _settings.BaseUrl);
        requestMessage.Content = new FormUrlEncodedContent(parameters);
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        using var response = await httpClient.SendAsync(requestMessage);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<T>();
    }

    private async Task<FoodProductResponse> GetFoodDetailsAsync(Dictionary<string, string> parameters, string notFoundMessage)
    {
        var responseData = await ExecuteFatSecretRequestAsync<FatSecretGetResponse>(parameters);

        if (responseData?.Food is null) 
            throw new NotFoundException(notFoundMessage);

        var food = responseData.Food;
        
        var servings = food.Servings.Serving.Select(s => new FoodServingDto(
            s.ServingId,
            ParseFatSecretDouble(s.MetricServingAmount),
            s.MetricServingUnit,
            ParseFatSecretDouble(s.Calories),
            ParseFatSecretDouble(s.Protein),
            ParseFatSecretDouble(s.Fat),
            ParseFatSecretDouble(s.Carbohydrate)
        )).ToList();

        return new FoodProductResponse(
            food.FoodId,
            food.FoodName,
            food.BrandName,
            servings
        );
    }

    private static double ParseFatSecretDouble(string? value)
    {
        if (string.IsNullOrEmpty(value)) return 0;
        return double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result) ? result : 0;
    }
}