using CommunityToolkit.Mvvm.ComponentModel;
using ForgeFit.MAUI.Models.DTOs.Food;

namespace ForgeFit.MAUI.ViewModels.Diary.Recipes;

public partial class RecipeItemViewModel(RecipeDto recipe) : ObservableObject
{
    [ObservableProperty] private bool _isAdded;
    [ObservableProperty] private bool _isAdding;

    public RecipeDto Recipe { get; } = recipe;

    public string Name => Recipe.Name;
    public string? Description => Recipe.Description;
    public double TotalCalories => Recipe.TotalCalories;
    public double TotalCarbs => Recipe.TotalCarbs;
    public double TotalProtein => Recipe.TotalProtein;
    public double TotalFat => Recipe.TotalFat;

    public List<FoodItemDto> Top3Ingredients => Recipe.Ingredients.Take(3).ToList();
    public int RemainingCount => Math.Max(0, Recipe.Ingredients.Count - 3);
    public bool HasMore => RemainingCount > 0;
}