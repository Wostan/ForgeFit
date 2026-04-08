using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ForgeFit.MAUI.Messages;
using ForgeFit.MAUI.Models.DTOs.Goal;
using ForgeFit.MAUI.Services.Interfaces;
using ForgeFit.MAUI.ViewModels.Core;
using LocalizationResourceManager.Maui;

namespace ForgeFit.MAUI.ViewModels.Profile.Goals;

public partial class NutritionGoalViewModel(
    IGoalService goalService,
    IAlertService alertService,
    ILocalizationResourceManager localizationManager)
    : BaseViewModel
{
    private NutritionGoalResponse? _currentNutritionGoal;
    [ObservableProperty] private string? _editCalories;
    [ObservableProperty] private string? _editCarbs;
    [ObservableProperty] private string? _editFat;
    [ObservableProperty] private string? _editProtein;
    [ObservableProperty] private string? _editWater;

    [ObservableProperty] private bool _isEditNutritionGoalPopupVisible;
    [ObservableProperty] private string _nutritionGoalCalories = string.Empty;
    [ObservableProperty] private string _nutritionGoalCarbs = string.Empty;
    [ObservableProperty] private string _nutritionGoalFat = string.Empty;
    [ObservableProperty] private string _nutritionGoalProtein = string.Empty;
    [ObservableProperty] private string _nutritionGoalVolumeMl = string.Empty;

    public void UpdateState(NutritionGoalResponse goal)
    {
        _currentNutritionGoal = goal;
        NutritionGoalCalories = goal.Calories.ToString();
        NutritionGoalCarbs = goal.Carbs.ToString();
        NutritionGoalProtein = goal.Protein.ToString();
        NutritionGoalFat = goal.Fat.ToString();
        NutritionGoalVolumeMl = goal.WaterGoalMl.ToString();
    }

    [RelayCommand]
    private void OpenEditNutritionGoal()
    {
        if (_currentNutritionGoal == null) return;
        EditCalories = _currentNutritionGoal.Calories.ToString();
        EditCarbs = _currentNutritionGoal.Carbs.ToString();
        EditProtein = _currentNutritionGoal.Protein.ToString();
        EditFat = _currentNutritionGoal.Fat.ToString();
        EditWater = _currentNutritionGoal.WaterGoalMl.ToString();
        IsEditNutritionGoalPopupVisible = true;
    }

    [RelayCommand]
    private void CloseEditNutrition()
    {
        IsEditNutritionGoalPopupVisible = false;
    }

    [RelayCommand]
    private async Task SaveNutrition()
    {
        if (!int.TryParse(EditCalories, out var cal) || !int.TryParse(EditCarbs, out var carbs) ||
            !int.TryParse(EditProtein, out var prot) || !int.TryParse(EditFat, out var fat) ||
            !int.TryParse(EditWater, out var water))
        {
            await alertService.ShowToastAsync(localizationManager["Error_InvalidInput"]);
            return;
        }

        if (cal is < 500 or > 10000)
        {
            await alertService.ShowToastAsync(localizationManager["Error_CaloriesRange"]);
            return;
        }

        if (carbs < 0 || prot < 0 || fat < 0)
        {
            await alertService.ShowToastAsync(localizationManager["Error_MacrosPositive"]);
            return;
        }

        if (water is < 1000 or > 10000)
        {
            await alertService.ShowToastAsync(localizationManager["Error_WaterRange"]);
            return;
        }

        IsEditNutritionGoalPopupVisible = false;
        IsLoading = true;

        try
        {
            var request = new NutritionGoalCreateRequest(cal, carbs, prot, fat, water);
            var result = await goalService.UpdateNutritionGoal(request);

            if (result is { Success: true, Data: not null })
            {
                UpdateState(result.Data);
                await alertService.ShowToastAsync(localizationManager["Success_NutritionUpdated"]);
                OnGoalUpdated?.Invoke();
                WeakReferenceMessenger.Default.Send(new NutritionGoalChangedMessage(nameof(NutritionGoalViewModel)));
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

    public void SetCurrentGoal(NutritionGoalResponse goal)
    {
        _currentNutritionGoal = goal;
    }

    public event Action? OnGoalUpdated;
}