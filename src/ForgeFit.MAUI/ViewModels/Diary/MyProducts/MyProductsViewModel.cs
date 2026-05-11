using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ForgeFit.MAUI.Constants;
using ForgeFit.MAUI.Models.DTOs.Food;
using ForgeFit.MAUI.Services.Interfaces;
using ForgeFit.MAUI.ViewModels.Core;
using ForgeFit.MAUI.ViewModels.Diary.FoodSearch;
using LocalizationResourceManager.Maui;

namespace ForgeFit.MAUI.ViewModels.Diary.MyProducts;

public partial class MyProductsViewModel(
    PopupManagerViewModel popupManager,
    ICustomFoodService customFoodService,
    IAlertService alertService,
    ILocalizationResourceManager localizationManager,
    FoodDiaryIntegrationViewModel diaryVM,
    FoodDetailsViewModel detailsVM,
    CreateCustomFoodViewModel createCustomFoodVM) : ObservableObject
{
    private List<CustomFoodDto> _allCustomFoods = [];
    [ObservableProperty] private bool _isLoading;
    private CancellationTokenSource? _searchCts;

    [ObservableProperty] private string _searchText = string.Empty;

    public ObservableCollection<FoodSearchItemViewModel> SearchResults { get; } = [];

    [RelayCommand]
    private void OpenCreateFoodPopup()
    {
        createCustomFoodVM.InitializeForCreate();
        popupManager.OpenCreateFoodPopup();
    }

    public async Task LoadProductsAsync(CancellationToken token = default)
    {
        IsLoading = true;
        try
        {
            var result = await customFoodService.GetAllForUserAsync(token);
            if (result is { Success: true, Data: not null })
            {
                _allCustomFoods = result.Data;
                UpdateSearchResults();
            }
            else
            {
                await alertService.ShowToastAsync(result.Message);
            }
        }
        catch
        {
            await alertService.ShowToastAsync(localizationManager["UnexpectedErrorMessage"]);
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void UpdateSearchResults()
    {
        var filtered = _allCustomFoods.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var searchLower = SearchText.ToLowerInvariant();
            filtered = filtered.Where(f =>
                f.Name.Contains(searchLower, StringComparison.InvariantCultureIgnoreCase) ||
                (f.Brand?.ToLowerInvariant().Contains(searchLower, StringComparison.InvariantCultureIgnoreCase) ??
                 false));
        }

        SearchResults.Clear();

        foreach (var food in filtered)
        {
            var searchResponse = new FoodSearchResponse(
                food.Id.ToString(),
                food.Name,
                food.Brand,
                food.Calories,
                food.Carbs,
                food.Protein,
                food.Fat,
                food.Fiber,
                food.Sugar,
                food.SaturatedFat,
                food.Sodium,
                $"{food.ServingSize} {food.ServingUnit}"
            );

            var vm = new FoodSearchItemViewModel(searchResponse);
            if (diaryVM.IsProductAdded(searchResponse.ExternalId)) vm.IsAdded = true;

            SearchResults.Add(vm);
        }
    }

    partial void OnSearchTextChanged(string value)
    {
        _searchCts?.Cancel();
        _searchCts?.Dispose();

        if (string.IsNullOrWhiteSpace(value))
        {
            UpdateSearchResults();
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
                MainThread.BeginInvokeOnMainThread(UpdateSearchResults);
            }
            catch (TaskCanceledException)
            {
            }
        }, token);
    }

    [RelayCommand]
    private async Task ToggleItem(FoodSearchItemViewModel? itemVm)
    {
        if (itemVm == null || itemVm.IsAdding) return;

        itemVm.IsAdding = true;
        try
        {
            if (itemVm.IsAdded)
                await diaryVM.RemoveItemInternal(itemVm);
            else
                await diaryVM.QuickAddInternal(itemVm, false);
        }
        finally
        {
            itemVm.IsAdding = false;
        }
    }

    [RelayCommand]
    private async Task QuickAddItem(FoodSearchItemViewModel? itemVm)
    {
        await ToggleItem(itemVm);
    }

    [RelayCommand]
    private async Task RemoveItem(FoodSearchItemViewModel? itemVm)
    {
        await ToggleItem(itemVm);
    }

    [RelayCommand]
    private void OpenEditFoodPopup(FoodSearchItemViewModel itemVm)
    {
        var food = _allCustomFoods.FirstOrDefault(f => f.Id.ToString() == itemVm.Data.ExternalId);
        if (food != null)
        {
            createCustomFoodVM.InitializeForEdit(food);
            popupManager.OpenCreateFoodPopup();
        }
    }

    [RelayCommand]
    private void DeleteFood(FoodSearchItemViewModel itemVm)
    {
        popupManager.ShowConfirmation(
            "Title_DeleteFood",
            "Message_ConfirmDeleteFood",
            async () =>
            {
                var deleteResult = await customFoodService.DeleteAsync(Guid.Parse(itemVm.Data.ExternalId));
                if (deleteResult.Success)
                {
                    _allCustomFoods.RemoveAll(f => f.Id.ToString() == itemVm.Data.ExternalId);
                    UpdateSearchResults();
                }
                else
                {
                    await alertService.ShowToastAsync(deleteResult.Message);
                }
            });
    }

    [RelayCommand]
    private async Task OpenFoodDetails(FoodSearchItemViewModel? itemVm)
    {
        if (itemVm is null || IsLoading || itemVm.IsAdding || itemVm.IsAdded) return;

        itemVm.IsAdding = true;
        try
        {
            var customFood = _allCustomFoods.FirstOrDefault(f => f.Id.ToString() == itemVm.Data.ExternalId);

            if (customFood != null)
            {
                var mockServing = new FoodServingDto(
                    "Custom",
                    customFood.ServingSize,
                    customFood.ServingUnit,
                    customFood.Calories,
                    customFood.Carbs,
                    customFood.Protein,
                    customFood.Fat,
                    customFood.Fiber,
                    customFood.Sugar,
                    customFood.SaturatedFat,
                    customFood.Sodium
                );
                var productResponse = new FoodProductResponse(
                    customFood.Id.ToString(),
                    customFood.Name,
                    customFood.Brand,
                    [mockServing]
                );

                await detailsVM.OpenFoodDetailsInternal(productResponse, itemVm.Data, false);
            }
            else
            {
                await alertService.ShowToastAsync(localizationManager["UnexpectedErrorMessage"]);
            }
        }
        finally
        {
            itemVm.IsAdding = false;
        }
    }

    public async Task OnFoodCreatedAsync(CustomFoodDto newFood)
    {
        _allCustomFoods.Add(newFood);
        SearchText = string.Empty;
        UpdateSearchResults();
    }

    public async Task OnFoodUpdatedAsync(CustomFoodDto updatedFood)
    {
        var existingIndex = _allCustomFoods.FindIndex(f => f.Id == updatedFood.Id);
        if (existingIndex >= 0)
        {
            _allCustomFoods[existingIndex] = updatedFood;
            UpdateSearchResults();
        }
    }
}
