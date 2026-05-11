using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ForgeFit.MAUI.Constants;
using ForgeFit.MAUI.Models.DTOs.Food;
using ForgeFit.MAUI.Services.Interfaces;
using ForgeFit.MAUI.ViewModels.Core;
using ForgeFit.MAUI.ViewModels.Diary.FoodSearch;
using LocalizationResourceManager.Maui;

namespace ForgeFit.MAUI.ViewModels.Diary.Recipes;

public partial class CreateRecipeViewModel(
    PopupManagerViewModel popupManager,
    IRecipeService recipeService,
    IFoodService foodService,
    ICustomFoodService customFoodService,
    IAlertService alertService,
    ILocalizationResourceManager localizationManager) : ObservableObject
{
    [ObservableProperty] private string? _description;

    private FoodItemDto? _editingIngredient;

    private Guid? _editingRecipeId;
    [ObservableProperty] private string _name = string.Empty;
    [ObservableProperty] private double _totalCalories;
    [ObservableProperty] private double _totalCarbs;
    [ObservableProperty] private double _totalFat;
    [ObservableProperty] private double _totalFiber;
    [ObservableProperty] private double _totalProtein;
    [ObservableProperty] private double _totalSaturatedFat;
    [ObservableProperty] private double _totalSodium;
    [ObservableProperty] private double _totalSugar;

    public Func<RecipeDto, Task>? RecipeCreatedCallback;
    public Func<RecipeDto, Task>? RecipeUpdatedCallback;
    public FoodDetailsViewModel IngredientDetailsVM { get; } = new(alertService);

    public ObservableCollection<FoodItemDto> Ingredients { get; } = [];

    public RecipeIngredientSearchViewModel IngredientSearchVM { get; } =
        new(foodService, customFoodService, alertService);

    private void SetupCallbacks()
    {
        IngredientSearchVM.IngredientSelectedCallback = async newItem =>
        {
            Ingredients.Add(newItem);
            RecalculateMacros();
        };

        IngredientSearchVM.OpenDetailsCallback = async (product, searchItem) =>
        {
            _editingIngredient = null;
            await IngredientDetailsVM.OpenFoodDetailsInternal(product, searchItem, false);
        };

        IngredientDetailsVM.SaveFoodCallback = async newItem =>
        {
            if (_editingIngredient != null)
            {
                var index = Ingredients.IndexOf(_editingIngredient);
                if (index >= 0) Ingredients[index] = newItem;
                _editingIngredient = null;
            }
            else
            {
                Ingredients.Add(newItem);
            }

            RecalculateMacros();
            IngredientDetailsVM.IsFoodDetailsVisible = false;
        };

        IngredientDetailsVM.CloseFoodDetailsCallback = () =>
        {
            IngredientDetailsVM.IsFoodDetailsVisible = false;
            return Task.CompletedTask;
        };
    }

    public void InitializeForCreate(IEnumerable<FoodItemDto>? initialIngredients = null)
    {
        Name = string.Empty;
        Description = null;
        _editingRecipeId = null;
        Ingredients.Clear();

        if (initialIngredients != null)
            foreach (var ingredient in initialIngredients)
                Ingredients.Add(ingredient);

        SetupCallbacks();
        RecalculateMacros();
    }

    public void InitializeForEdit(RecipeDto recipe)
    {
        Name = recipe.Name;
        Description = recipe.Description;
        _editingRecipeId = recipe.Id;
        Ingredients.Clear();

        foreach (var ingredient in recipe.Ingredients) Ingredients.Add(ingredient);

        SetupCallbacks();
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

            if (Ingredients.Count == 0)
            {
                await alertService.ShowToastAsync(localizationManager["Error_RecipeNoIngredients"]);
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

                    if (RecipeUpdatedCallback != null) await RecipeUpdatedCallback(updateResult.Data);
                }
                else
                {
                    await alertService.ShowToastAsync(updateResult.Message);
                    Debug.WriteLine(updateResult.Message);
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

                    if (RecipeCreatedCallback != null) await RecipeCreatedCallback(createResult.Data);
                }
                else
                {
                    await alertService.ShowToastAsync(createResult.Message);
                    Debug.WriteLine(createResult.Message);
                }
            }
        }
        catch
        {
            await alertService.ShowToastAsync(localizationManager["UnexpectedErrorMessage"]);
        }
    }

    [RelayCommand]
    private void AddIngredient()
    {
        IngredientSearchVM.ResetState();
        popupManager.OpenRecipeIngredientSearchPopup();
    }

    [RelayCommand]
    private void RemoveIngredient(FoodItemDto ingredient)
    {
        Ingredients.Remove(ingredient);
        RecalculateMacros();
    }

    [RelayCommand]
    private async Task EditIngredient(FoodItemDto ingredient)
    {
        try
        {
            FoodProductResponse? product;

            if (Guid.TryParse(ingredient.ExternalId, out var customId))
            {
                var customResult = await customFoodService.GetByIdAsync(customId);
                if (customResult is { Success: true, Data: not null })
                {
                    var cf = customResult.Data;
                    product = new FoodProductResponse(
                        cf.Id.ToString(), cf.Name, cf.Brand,
                        [
                            new FoodServingDto("Custom", cf.ServingSize, cf.ServingUnit, cf.Calories, cf.Carbs,
                                cf.Protein, cf.Fat, cf.Fiber, cf.Sugar, cf.SaturatedFat, cf.Sodium)
                        ]
                    );
                }
                else
                {
                    await alertService.ShowToastAsync(new LocalizedString(() => customResult.Message).Localized);
                    return;
                }
            }
            else
            {
                var productResult = await foodService.GetProductByIdAsync(ingredient.ExternalId);
                if (productResult is { Success: true, Data: not null })
                {
                    product = productResult.Data;
                }
                else
                {
                    await alertService.ShowToastAsync(new LocalizedString(() => productResult.Message).Localized);
                    return;
                }
            }

            _editingIngredient = ingredient;
            IngredientDetailsVM.OpenFoodDetailsInternal(product, ingredient);
        }
        catch (Exception ex)
        {
            await alertService.ShowToastAsync(new LocalizedString(() => ex.Message).Localized);
        }
    }
}
