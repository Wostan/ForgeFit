using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ForgeFit.MAUI.Constants;
using ForgeFit.MAUI.Models.DTOs.Food;
using ForgeFit.MAUI.Services.Interfaces;
using ForgeFit.MAUI.ViewModels.Core;
using LocalizationResourceManager.Maui;

namespace ForgeFit.MAUI.ViewModels.Diary.Recipes;

public partial class CreateRecipeViewModel(
    PopupManagerViewModel popupManager,
    IRecipeService recipeService,
    IAlertService alertService,
    ILocalizationResourceManager localizationManager) : ObservableObject
{
    [ObservableProperty] private string _name = string.Empty;
    [ObservableProperty] private string? _description;
    [ObservableProperty] private double _totalCalories;
    [ObservableProperty] private double _totalCarbs;
    [ObservableProperty] private double _totalProtein;
    [ObservableProperty] private double _totalFat;
    [ObservableProperty] private double _totalFiber;
    [ObservableProperty] private double _totalSugar;
    [ObservableProperty] private double _totalSaturatedFat;
    [ObservableProperty] private double _totalSodium;

    private Guid? _editingRecipeId;

    public ObservableCollection<FoodItemDto> Ingredients { get; } = [];

    public Func<RecipeDto, Task>? RecipeCreatedCallback;
    public Func<RecipeDto, Task>? RecipeUpdatedCallback;

    public void InitializeForCreate(IEnumerable<FoodItemDto>? initialIngredients = null)
    {
        Name = string.Empty;
        Description = null;
        _editingRecipeId = null;
        Ingredients.Clear();

        if (initialIngredients != null)
        {
            foreach (var ingredient in initialIngredients)
            {
                Ingredients.Add(ingredient);
            }
        }

        RecalculateMacros();
    }

    public void InitializeForEdit(RecipeDto recipe)
    {
        Name = recipe.Name;
        Description = recipe.Description;
        _editingRecipeId = recipe.Id;
        Ingredients.Clear();

        foreach (var ingredient in recipe.Ingredients)
        {
            Ingredients.Add(ingredient);
        }

        RecalculateMacros();
    }

    private void RecalculateMacros()
    {
        TotalCalories = Ingredients.Sum(i => i.Calories);
        TotalCarbs = Ingredients.Sum(i => i.Carbs);
        TotalProtein = Ingredients.Sum(i => i.Protein);
        TotalFat = Ingredients.Sum(i => i.Fat);
        TotalFiber = Ingredients.Sum(i => i.Fiber);
        TotalSugar = Ingredients.Sum(i => i.Sugar);
        TotalSaturatedFat = Ingredients.Sum(i => i.SaturatedFat);
        TotalSodium = Ingredients.Sum(i => i.Sodium);
    }

    [RelayCommand]
    private void Close()
    {
        popupManager.CloseCreateRecipePopupCommand.Execute(null);
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                await alertService.ShowToastAsync(localizationManager["Error_RecipeNameRequired"]);
                return;
            }

            if (Name.Length > AppConstants.ValidationLimits.MaxFoodLabelLength)
            {
                await alertService.ShowToastAsync(localizationManager["Error_RecipeNameTooLong"]);
                return;
            }

            if (_editingRecipeId.HasValue)
            {
                var updateRequest = new RecipeUpdateRequest(
                    Name,
                    Description,
                    Ingredients.ToList()
                );

                var updateResult = await recipeService.UpdateAsync(_editingRecipeId.Value, updateRequest);

                if (updateResult is { Success: true, Data: not null })
                {
                    popupManager.CloseCreateRecipePopup();
                    await alertService.ShowToastAsync(localizationManager["Success_RecipeUpdated"]);

                    if (RecipeUpdatedCallback != null)
                    {
                        await RecipeUpdatedCallback(updateResult.Data);
                    }
                }
                else
                {
                    await alertService.ShowToastAsync(updateResult.Message);
                }
            }
            else
            {
                var createRequest = new RecipeCreateRequest(
                    Name,
                    Description,
                    Ingredients.ToList()
                );

                var createResult = await recipeService.CreateAsync(createRequest);

                if (createResult is { Success: true, Data: not null })
                {
                    popupManager.CloseCreateRecipePopup();
                    await alertService.ShowToastAsync(localizationManager["Success_RecipeCreated"]);

                    if (RecipeCreatedCallback != null)
                    {
                        await RecipeCreatedCallback(createResult.Data);
                    }
                }
                else
                {
                    await alertService.ShowToastAsync(createResult.Message);
                }
            }
        }
        catch
        {
            await alertService.ShowToastAsync(localizationManager["UnexpectedErrorMessage"]);
        }
    }

    [RelayCommand]
    private Task AddIngredient()
    {
        return Task.CompletedTask;
    }

    [RelayCommand]
    private void RemoveIngredient(FoodItemDto ingredient)
    {
        Ingredients.Remove(ingredient);
        RecalculateMacros();
    }

    [RelayCommand]
    private Task EditIngredient(FoodItemDto ingredient)
    {
        return Task.CompletedTask;
    }
}
