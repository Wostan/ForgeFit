using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ForgeFit.MAUI.Constants;
using ForgeFit.MAUI.Models.DTOs.Food;
using ForgeFit.MAUI.Services.Interfaces;
using ForgeFit.MAUI.ViewModels.Core;
using ForgeFit.MAUI.ViewModels.Diary.FoodSearch;
using LocalizationResourceManager.Maui;

namespace ForgeFit.MAUI.ViewModels.Diary.Recipes;

public partial class RecipesViewModel(
    PopupManagerViewModel popupManager,
    IRecipeService recipeService,
    IAlertService alertService,
    ILocalizationResourceManager localizationManager,
    FoodDiaryIntegrationViewModel diaryVM,
    FoodDetailsViewModel detailsVM,
    CreateRecipeViewModel createRecipeVM) : ObservableObject
{
    private List<RecipeDto> _allRecipes = [];
    private CancellationTokenSource? _searchCts;

    [ObservableProperty] private string _searchText = string.Empty;
    [ObservableProperty] private bool _isLoading;

    public ObservableCollection<RecipeItemViewModel> SearchResults { get; } = [];
    public CreateRecipeViewModel CreateRecipeVM => createRecipeVM;

    public async Task LoadRecipesAsync(CancellationToken token = default)
    {
        IsLoading = true;
        try
        {
            var result = await recipeService.GetAllAsync(token);
            if (result is { Success: true, Data: not null })
            {
                _allRecipes = result.Data;
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
        var filtered = _allRecipes.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var searchLower = SearchText.ToLowerInvariant();
            filtered = filtered.Where(r =>
                r.Name.Contains(searchLower, StringComparison.InvariantCultureIgnoreCase) ||
                (r.Description?.ToLowerInvariant().Contains(searchLower, StringComparison.InvariantCultureIgnoreCase) ?? false));
        }

        SearchResults.Clear();

        foreach (var recipe in filtered)
        {
            var vm = new RecipeItemViewModel(recipe);
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
    private void OpenCreateRecipePopup()
    {
        createRecipeVM.Name = string.Empty;
        createRecipeVM.Description = null;
        createRecipeVM.Ingredients.Clear();
        popupManager.OpenCreateRecipePopup();
    }

    [RelayCommand]
    private void EditRecipe(RecipeItemViewModel? recipeVm)
    {
    }

    [RelayCommand]
    private void DeleteRecipe(RecipeItemViewModel? recipeVm)
    {
    }
}
