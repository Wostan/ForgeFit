using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ForgeFit.MAUI.Constants;
using ForgeFit.MAUI.Models.DTOs.Food;
using ForgeFit.MAUI.Services.Interfaces;

namespace ForgeFit.MAUI.ViewModels.Diary.FoodSearch;

public partial class FoodDetailsViewModel(
    IAlertService alertService) : ObservableObject
{
    [NotifyCanExecuteChangedFor(nameof(SaveFoodCommand))] [ObservableProperty]
    private string? _inputAmount;

    [ObservableProperty] private bool _isFoodDetailsVisible;
    [ObservableProperty] private FoodProductResponse? _selectedFoodDetail;
    [ObservableProperty] private FoodServingDto? _selectedServing;

    public double CurrentCalories => CalculateNutrient(s => s.Calories);
    public double CurrentCarbs => CalculateNutrient(s => s.Carbs);
    public double CurrentProtein => CalculateNutrient(s => s.Protein);
    public double CurrentFat => CalculateNutrient(s => s.Fat);

    public Func<FoodProductResponse, FoodSearchResponse, Task>? OpenFoodDetailsCallback { get; set; }
    public Func<Task>? CloseFoodDetailsCallback { get; set; }
    public Func<FoodItemDto, Task>? SaveFoodCallback { get; set; }

    [RelayCommand]
    private void CloseFoodDetails()
    {
        IsFoodDetailsVisible = false;
        CloseFoodDetailsCallback?.Invoke();
    }

    [RelayCommand(CanExecute = nameof(CanSaveFood))]
    private async Task SaveFood()
    {
        if (SelectedFoodDetail == null || SelectedServing == null || string.IsNullOrEmpty(InputAmount) ||
            SaveFoodCallback == null)
            return;

        var product = SelectedFoodDetail;
        var serving = SelectedServing;
        var normalizedInput = InputAmount.Replace(',', '.');

        if (!double.TryParse((string?)normalizedInput, NumberStyles.Any, CultureInfo.InvariantCulture,
                out var amount) ||
            amount <= 0)
        {
            await alertService.ShowToastAsync("Invalid amount");
            return;
        }

        IsFoodDetailsVisible = false;

        try
        {
            var ratio = amount / serving.MetricAmount;
            var newItem = new FoodItemDto(
                product.ExternalId, product.Label,
                serving.Calories * ratio, serving.Carbs * ratio,
                serving.Protein * ratio, serving.Fat * ratio,
                serving.Fiber * ratio, serving.Sugar * ratio,
                serving.SaturatedFat * ratio, serving.Sodium * ratio,
                serving.MetricUnit, amount
            );
            await SaveFoodCallback(newItem);
        }
        catch (Exception ex)
        {
            await alertService.ShowToastAsync(ex.Message);
        }
    }

    private bool CanSaveFood()
    {
        if (string.IsNullOrEmpty(InputAmount))
            return false;

        var normalizedInput = InputAmount.Replace(',', '.');
        return double.TryParse((string?)normalizedInput, NumberStyles.Any, CultureInfo.InvariantCulture,
                   out var amount) &&
               amount is > 0 and <= AppConstants.ValidationLimits.MaxFoodAmount;
    }

    partial void OnInputAmountChanged(string? value)
    {
        NotifyPopupUpdates();
        SaveFoodCommand.NotifyCanExecuteChanged();
    }

    partial void OnSelectedServingChanged(FoodServingDto? value)
    {
        NotifyPopupUpdates();
    }

    private void NotifyPopupUpdates()
    {
        OnPropertyChanged(nameof(CurrentCalories));
        OnPropertyChanged(nameof(CurrentCarbs));
        OnPropertyChanged(nameof(CurrentProtein));
        OnPropertyChanged(nameof(CurrentFat));
    }

    private double CalculateNutrient(Func<FoodServingDto, double> selector)
    {
        if (string.IsNullOrEmpty(InputAmount))
            return 0;

        var normalizedInput = InputAmount.Replace(',', '.');
        if (!double.TryParse((string?)normalizedInput, NumberStyles.Any, CultureInfo.InvariantCulture, out var val) ||
            val <= 0)
            return 0;

        if (SelectedServing == null || SelectedServing.MetricAmount == 0)
            return 0;

        return val * selector(SelectedServing) / SelectedServing.MetricAmount;
    }

    public void ResetPopupState()
    {
        SelectedFoodDetail = null;
        SelectedServing = null;
        InputAmount = null;
        NotifyPopupUpdates();
    }

    public async Task OpenFoodDetailsInternal(FoodProductResponse productDetails, FoodSearchResponse sourceItem,
        bool isShowingRecent)
    {
        var uniqueServings = productDetails.Servings
            .GroupBy(s => s.MetricUnit)
            .Select(g => g.First())
            .ToList();

        productDetails = productDetails with { Servings = uniqueServings };
        SelectedFoodDetail = productDetails;

        var parts = sourceItem.Serving.Split(' ');
        FoodServingDto? targetServing = null;

        if (isShowingRecent && parts.Length > 1)
        {
            var historyUnit = parts.Last();
            targetServing = uniqueServings.FirstOrDefault(s =>
                string.Equals(s.MetricUnit, historyUnit, StringComparison.OrdinalIgnoreCase));
        }

        targetServing ??= uniqueServings.FirstOrDefault();
        SelectedServing = targetServing;

        if (targetServing == null)
        {
            InputAmount = "100";
            IsFoodDetailsVisible = true;
            return;
        }

        if (isShowingRecent && sourceItem.Calories > 0 && targetServing.Calories > 0)
        {
            var ratio = sourceItem.Calories / targetServing.Calories;
            InputAmount = Math.Round(targetServing.MetricAmount * ratio, 2).ToString(CultureInfo.InvariantCulture);
        }
        else
        {
            InputAmount = targetServing.MetricAmount.ToString(CultureInfo.InvariantCulture);
        }

        IsFoodDetailsVisible = true;
    }

    public void OpenFoodDetailsInternal(FoodProductResponse productDetails, FoodItemDto existingItem)
    {
        var uniqueServings = productDetails.Servings.GroupBy(s => s.MetricUnit).Select(g => g.First()).ToList();
        productDetails = productDetails with { Servings = uniqueServings };
        SelectedFoodDetail = productDetails;

        SelectedServing = uniqueServings.FirstOrDefault(s =>
                              string.Equals(s.MetricUnit, existingItem.ServingUnit, StringComparison.OrdinalIgnoreCase))
                          ?? uniqueServings.FirstOrDefault();

        InputAmount = existingItem.Amount.ToString(CultureInfo.InvariantCulture);
        IsFoodDetailsVisible = true;
    }
}