using System.Globalization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
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
            { "method", "foods.search.v5" },
            { "search_expression", query },
            { "page_number", (pageNumber - 1).ToString() },
            { "max_results", pageSize.ToString() },
            { "region", _settings.Region },
            { "language", _settings.Language },
            { "format", "json" }
        };

        var response = await ExecuteFatSecretRequestAsync<FatSecretSearchRoot>(parameters);

        var foods = response?.SearchResponse?.Results?.Food ?? [];

        return foods.Select(f =>
        {
            var serving = f.Servings?.Serving?.FirstOrDefault();

            var amount = ParseFatSecretDouble(serving?.MetricServingAmount);
            var unit = serving?.MetricServingUnit ?? "g";

            return new FoodSearchResponse(
                f.FoodId,
                f.FoodName,
                f.BrandName,
                ParseFatSecretDouble(serving?.Calories),
                ParseFatSecretDouble(serving?.Carbohydrate),
                ParseFatSecretDouble(serving?.Protein),
                ParseFatSecretDouble(serving?.Fat),
                ParseFatSecretDouble(serving?.Fiber),
                ParseFatSecretDouble(serving?.Sugar),
                ParseFatSecretDouble(serving?.SaturatedFat),
                ParseFatSecretDouble(serving?.Sodium),
                $"{amount:0.##} {unit}"
            );
        }).ToList();
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

    public async Task<List<FoodProductResponse>> RecognizeByPhotoAsync(string imageBase64)
    {
        if (imageBase64.Contains(','))
        {
            imageBase64 = imageBase64.Split(',')[1];
        }
        
        if (imageBase64.Length > 999_982)
            throw new BadRequestException("Image is too large. Limit is ~1MB characters.");

        var token = await fatSecretTokenService.GetAccessTokenAsync();

        var requestBody = new
        {
            image_b64 = imageBase64,
            include_food_data = true,
            region = _settings.Region,
            language = _settings.Language
        };

        using var requestMessage = new HttpRequestMessage(HttpMethod.Post, _settings.RecognitionUrl);
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        requestMessage.Content = JsonContent.Create(requestBody);

        try
        {
            using var response = await httpClient.SendAsync(requestMessage);

            if (!response.IsSuccessStatusCode)
            {
                var err = await response.Content.ReadAsStringAsync();
                throw new ServiceUnavailableException($"FatSecret Recognition Error ({response.StatusCode}): {err}");
            }

            var recognitionResult = await response.Content.ReadFromJsonAsync<FatSecretRecognitionRoot>();

            var resultList = new List<FoodProductResponse>();

            if (recognitionResult?.FoodResponse == null) return resultList;

            foreach (var (_, _, f) in recognitionResult.FoodResponse)
            {
                if (f == null) continue;

                var servings = f.Servings?.Serving?.Select(s => new FoodServingDto(
                    s.ServingId,
                    ParseFatSecretDouble(s.MetricServingAmount),
                    s.MetricServingUnit,
                    ParseFatSecretDouble(s.Calories),
                    ParseFatSecretDouble(s.Carbohydrate),
                    ParseFatSecretDouble(s.Protein),
                    ParseFatSecretDouble(s.Fat),
                    ParseFatSecretDouble(s.Fiber),
                    ParseFatSecretDouble(s.Sugar),
                    ParseFatSecretDouble(s.SaturatedFat),
                    ParseFatSecretDouble(s.Sodium)
                )).ToList() ?? [];

                resultList.Add(new FoodProductResponse(
                    f.FoodId,
                    f.FoodName,
                    f.BrandName,
                    servings
                ));
            }

            return resultList;
        }
        catch (HttpRequestException ex)
        {
            throw new ServiceUnavailableException($"FatSecret Recognition service unavailable: {ex.Message}");
        }
        catch (JsonException ex)
        {
            throw new ServiceUnavailableException($"FatSecret Recognition response parsing failed: {ex.Message}");
        }
    }

    private async Task<T?> ExecuteFatSecretRequestAsync<T>(Dictionary<string, string> parameters)
    {
        var token = await fatSecretTokenService.GetAccessTokenAsync();

        using var requestMessage = new HttpRequestMessage(HttpMethod.Post, _settings.BaseUrl);
        requestMessage.Content = new FormUrlEncodedContent(parameters);
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        try
        {
            using var response = await httpClient.SendAsync(requestMessage);
        
            if (!response.IsSuccessStatusCode)
            {
                var err = await response.Content.ReadAsStringAsync();
                throw new ServiceUnavailableException($"FatSecret API Error ({response.StatusCode}): {err}");
            }

            return await response.Content.ReadFromJsonAsync<T>();
        }
        catch (HttpRequestException ex)
        {
            throw new ServiceUnavailableException($"FatSecret API service unavailable: {ex.Message}");
        }
        catch (JsonException ex)
        {
            throw new ServiceUnavailableException($"FatSecret API response parsing failed: {ex.Message}");
        }
    }

    private async Task<FoodProductResponse> GetFoodDetailsAsync(
        Dictionary<string, string> parameters,
        string notFoundMessage)
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
            ParseFatSecretDouble(s.Carbohydrate),
            ParseFatSecretDouble(s.Protein),
            ParseFatSecretDouble(s.Fat),
            ParseFatSecretDouble(s.Fiber),
            ParseFatSecretDouble(s.Sugar),
            ParseFatSecretDouble(s.SaturatedFat),
            ParseFatSecretDouble(s.Sodium)
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
        return double.TryParse(
            value,
            NumberStyles.Any,
            CultureInfo.InvariantCulture,
            out var result)
            ? result
            : 0;
    }
}
