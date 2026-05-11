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
    CreateRecipeViewModel createRecipeVM) : ObservableObject
{
    private List<RecipeDto> _allRecipes = [];
    [ObservableProperty] private bool _isLoading;
    private CancellationTokenSource? _searchCts;

    [ObservableProperty] private string _searchText = string.Empty;

    public ObservableCollection<RecipeItemViewModel> SearchResults { get; } = [];

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
                (r.Description?.ToLowerInvariant().Contains(searchLower, StringComparison.InvariantCultureIgnoreCase) ??
                 false));
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
        _searchCts = null;

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
    private async Task ToggleItem(RecipeItemViewModel? itemVm)
    {
        if (itemVm == null || itemVm.IsAdding || itemVm.IsAdded) return;

        itemVm.IsAdding = true;
        try
        {
            await diaryVM.QuickAddRecipeInternal(itemVm);
        }
        finally
        {
            itemVm.IsAdding = false;
        }
    }

    public async Task OnRecipeCreatedAsync(RecipeDto newRecipe)
    {
        _allRecipes.Add(newRecipe);
        SearchText = string.Empty;
        UpdateSearchResults();
    }

    public async Task OnRecipeUpdatedAsync(RecipeDto updatedRecipe)
    {
        var existingIndex = _allRecipes.FindIndex(r => r.Id == updatedRecipe.Id);
        if (existingIndex >= 0)
        {
            _allRecipes[existingIndex] = updatedRecipe;
            UpdateSearchResults();
        }
    }

    [RelayCommand]
    private void OpenCreateRecipePopup()
    {
        createRecipeVM.InitializeForCreate();
        popupManager.OpenCreateRecipePopup();
    }

    [RelayCommand]
    private void EditRecipe(RecipeItemViewModel? recipeVm)
    {
        if (recipeVm == null) return;
        var recipe = _allRecipes.FirstOrDefault(r => r.Id == recipeVm.Recipe.Id);
        if (recipe != null)
        {
            createRecipeVM.InitializeForEdit(recipe);
            popupManager.OpenCreateRecipePopup();
        }
    }

    [RelayCommand]
    private void DeleteRecipe(RecipeItemViewModel? recipeVm)
    {
        if (recipeVm == null) return;
        popupManager.ShowConfirmation(
            "Title_DeleteRecipe",
            "Message_ConfirmDeleteRecipe",
            async () =>
            {
                var deleteResult = await recipeService.DeleteAsync(recipeVm.Recipe.Id);
                if (deleteResult.Success)
                {
                    _allRecipes.RemoveAll(r => r.Id == recipeVm.Recipe.Id);
                    UpdateSearchResults();
                }
                else
                {
                    await alertService.ShowToastAsync(deleteResult.Message);
                }
            });
    }
}
