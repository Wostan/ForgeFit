using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ForgeFit.MAUI.Constants;
using ForgeFit.MAUI.Models.DTOs.Food;
using ForgeFit.MAUI.Services.Interfaces;
using ForgeFit.MAUI.ViewModels.Diary.FoodSearch;
using LocalizationResourceManager.Maui;

namespace ForgeFit.MAUI.ViewModels.Diary.Recipes;

public partial class RecipeIngredientSearchViewModel(
    IFoodService foodService,
    ICustomFoodService customFoodService,
    IAlertService alertService) : ObservableObject
{
    [ObservableProperty] private bool _isLoading;
    private CancellationTokenSource? _searchCts;

    [ObservableProperty] private string _searchText = string.Empty;

    public Func<FoodItemDto, Task>? IngredientSelectedCallback;
    public Func<FoodProductResponse, FoodSearchResponse, Task>? OpenDetailsCallback;

    public ObservableCollection<FoodSearchItemViewModel> SearchResults { get; } = [];

    partial void OnSearchTextChanged(string value)
    {
        _searchCts?.Cancel();
        _searchCts?.Dispose();
        _searchCts = null;

        if (string.IsNullOrWhiteSpace(value))
        {
            MainThread.BeginInvokeOnMainThread(() => SearchResults.Clear());
            IsLoading = false;
            return;
        }

        _searchCts = new CancellationTokenSource();
        var token = _searchCts.Token;

        _ = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(AppConstants.SearchConfig.DebounceDelayMs, token);
                if (token.IsCancellationRequested) return;
                MainThread.BeginInvokeOnMainThread(async void () => await PerformSearchAsync(value, token));
            }
            catch (TaskCanceledException)
            {
            }
        }, token);
    }

    private async Task PerformSearchAsync(string query, CancellationToken token)
    {
        IsLoading = true;
        SearchResults.Clear();

        try
        {
            var result = await foodService.SearchFoodAsync(query);

            if (token.IsCancellationRequested) return;

            if (result is not { Success: true, Data: not null })
            {
                if (result is { Success: false })
                {
                    var errorMsg = new LocalizedString(() => result.Message);
                    await alertService.ShowToastAsync(errorMsg.Localized);
                }

                return;
            }

            foreach (var item in result.Data) SearchResults.Add(new FoodSearchItemViewModel(item));
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task ToggleItem(FoodSearchItemViewModel? itemVm)
    {
        if (itemVm == null || itemVm.IsAdding || itemVm.IsAdded) return;

        itemVm.IsAdding = true;
        try
        {
            var product = await FetchProductAsync(itemVm.Data.ExternalId);
            if (product == null) return;

            var targetServing = product.Servings.FirstOrDefault();
            var amount = targetServing?.MetricAmount ?? AppConstants.FoodDefaults.DefaultServingAmount;
            var unit = targetServing?.MetricUnit ?? "g";
            var baseAmount = targetServing?.MetricAmount ?? 1;
            var ratio = amount / baseAmount;

            var newItem = new FoodItemDto(
                product.ExternalId, product.Label,
                (targetServing?.Calories ?? itemVm.Calories) * ratio,
                (targetServing?.Carbs ?? itemVm.Carbs) * ratio,
                (targetServing?.Protein ?? itemVm.Protein) * ratio,
                (targetServing?.Fat ?? itemVm.Fat) * ratio,
                (targetServing?.Fiber ?? 0) * ratio,
                (targetServing?.Sugar ?? 0) * ratio,
                (targetServing?.SaturatedFat ?? 0) * ratio,
                (targetServing?.Sodium ?? 0) * ratio,
                unit, amount
            );

            if (IngredientSelectedCallback != null)
                await IngredientSelectedCallback(newItem);

            itemVm.IsAdded = true;
        }
        catch (Exception ex)
        {
            await alertService.ShowToastAsync(new LocalizedString(() => ex.Message).Localized);
        }
        finally
        {
            itemVm.IsAdding = false;
        }
    }

    [RelayCommand]
    private async Task OpenFoodDetails(FoodSearchItemViewModel? itemVm)
    {
        if (itemVm == null || itemVm.IsAdding || itemVm.IsAdded) return;

        itemVm.IsAdding = true;
        try
        {
            var product = await FetchProductAsync(itemVm.Data.ExternalId);
            if (product == null) return;

            if (OpenDetailsCallback != null)
                await OpenDetailsCallback(product, itemVm.Data);
        }
        catch (Exception ex)
        {
            await alertService.ShowToastAsync(new LocalizedString(() => ex.Message).Localized);
        }
        finally
        {
            itemVm.IsAdding = false;
        }
    }

    private async Task<FoodProductResponse?> FetchProductAsync(string externalId)
    {
        if (Guid.TryParse(externalId, out var customId))
        {
            var customResult = await customFoodService.GetByIdAsync(customId);
            if (customResult is { Success: true, Data: not null })
            {
                var cf = customResult.Data;
                return new FoodProductResponse(
                    cf.Id.ToString(), cf.Name, cf.Brand,
                    [
                        new FoodServingDto("Custom", cf.ServingSize, cf.ServingUnit, cf.Calories, cf.Carbs, cf.Protein,
                            cf.Fat, cf.Fiber, cf.Sugar, cf.SaturatedFat, cf.Sodium)
                    ]
                );
            }

            await alertService.ShowToastAsync(new LocalizedString(() => customResult.Message).Localized);
            return null;
        }

        var productResult = await foodService.GetProductByIdAsync(externalId);
        if (productResult is { Success: true, Data: not null }) return productResult.Data;
        await alertService.ShowToastAsync(new LocalizedString(() => productResult.Message).Localized);
        return null;
    }


    public void ResetState()
    {
        SearchText = string.Empty;
        SearchResults.Clear();
        _searchCts?.Cancel();
        _searchCts?.Dispose();
        _searchCts = null;
        IsLoading = false;
    }
}
