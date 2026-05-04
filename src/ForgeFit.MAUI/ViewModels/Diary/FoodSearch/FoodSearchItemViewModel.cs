using CommunityToolkit.Mvvm.ComponentModel;
using ForgeFit.MAUI.Models.DTOs.Food;

namespace ForgeFit.MAUI.ViewModels.Diary.FoodSearch;

public partial class FoodSearchItemViewModel(FoodSearchResponse data) : ObservableObject
{
    [ObservableProperty] private bool _isAdded;

    [ObservableProperty] private bool _isAdding;
    public FoodSearchResponse Data { get; } = data;

    public string Label => Data.Label;
    public string BrandName => Data.BrandName ?? "";
    public string ServingUnit => Data.Serving;
    public double Calories => Data.Calories;
    public double Carbs => Data.Carbs;
    public double Protein => Data.Protein;
    public double Fat => Data.Fat;
    public double Fiber => Data.Fiber;
    public double Sugar => Data.Sugar;
    public double SaturatedFat => Data.SaturatedFat;
    public double Sodium => Data.Sodium;
}
