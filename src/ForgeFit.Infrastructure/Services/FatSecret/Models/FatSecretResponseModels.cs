using System.Text.Json.Serialization;
using ForgeFit.Infrastructure.Services.FatSecret.Converters;

namespace ForgeFit.Infrastructure.Services.FatSecret.Models;

internal record FatSecretSearchRoot(
    [property: JsonPropertyName("foods_search")] FatSecretSearchResponse? SearchResponse
);

internal record FatSecretSearchResponse(
    [property: JsonPropertyName("results")] FatSecretSearchResults? Results
);

internal record FatSecretSearchResults(
    [property: JsonPropertyName("food")]
    [property: JsonConverter(typeof(SingleOrArrayConverter<FatSecretSearchFoodItem>))]
    List<FatSecretSearchFoodItem>? Food
);

internal record FatSecretSearchFoodItem(
    [property: JsonPropertyName("food_id")]
    string FoodId,
    [property: JsonPropertyName("food_name")]
    string FoodName,
    [property: JsonPropertyName("brand_name")]
    string? BrandName,
    [property: JsonPropertyName("servings")] 
    FatSecretSearchServingsContainer? Servings
);

internal record FatSecretSearchServingsContainer(
    [property: JsonPropertyName("serving")]
    [property: JsonConverter(typeof(SingleOrArrayConverter<FatSecretServing>))]
    List<FatSecretServing>? Serving
);

internal record FatSecretGetResponse(
    [property: JsonPropertyName("food")] FatSecretDetailedFood? Food
);

internal record FatSecretDetailedFood(
    [property: JsonPropertyName("food_id")]
    string FoodId,
    [property: JsonPropertyName("food_name")]
    string FoodName,
    [property: JsonPropertyName("brand_name")]
    string? BrandName,
    [property: JsonPropertyName("servings")]
    FatSecretServingsContainer Servings
);

internal record FatSecretServingsContainer(
    [property: JsonPropertyName("serving")]
    [property: JsonConverter(typeof(SingleOrArrayConverter<FatSecretServing>))]
    List<FatSecretServing> Serving
);

internal record FatSecretServing(
    [property: JsonPropertyName("serving_id")]
    string ServingId,
    [property: JsonPropertyName("metric_serving_amount")]
    string MetricServingAmount,
    [property: JsonPropertyName("metric_serving_unit")]
    string MetricServingUnit,
    [property: JsonPropertyName("calories")]
    string Calories,
    [property: JsonPropertyName("carbohydrate")]
    string Carbohydrate,
    [property: JsonPropertyName("protein")]
    string Protein,
    [property: JsonPropertyName("fat")] string Fat
);
