using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ForgeFit.MAUI.Models.DTOs.Goal;
using ForgeFit.MAUI.Models.Enums.WorkoutEnums;
using ForgeFit.MAUI.Services.Interfaces;
using ForgeFit.MAUI.ViewModels.Core;
using LocalizationResourceManager.Maui;

namespace ForgeFit.MAUI.ViewModels.Profile.Goals;

public partial class WorkoutGoalViewModel(
    IGoalService goalService,
    IAlertService alertService,
    ILocalizationResourceManager localizationManager)
    : BaseViewModel
{
    private WorkoutGoalResponse? _currentWorkoutGoal;
    [ObservableProperty] private string? _editDurationMinutes;
    [ObservableProperty] private string? _editWorkoutsPerWeek;
    [ObservableProperty] private WorkoutTypeOption _editWorkoutType = new();

    [ObservableProperty] private bool _isEditWorkoutGoalPopupVisible;
    [ObservableProperty] private string _workoutGoalDurationMinutes = string.Empty;

    [ObservableProperty] private string _workoutGoalPerWeek = string.Empty;
    [ObservableProperty] private string _workoutGoalType = string.Empty;

    public ObservableCollection<WorkoutTypeOption> WorkoutTypes { get; } = [];

    public void InitializeWorkoutTypes()
    {
        WorkoutTypes.Clear();
        foreach (var type in Enum.GetValues<WorkoutType>())
            WorkoutTypes.Add(new WorkoutTypeOption
            {
                Value = type,
                DisplayName = localizationManager[$"WorkoutType_{type}"]
            });
    }

    public void UpdateState(WorkoutGoalResponse goal)
    {
        _currentWorkoutGoal = goal;
        WorkoutGoalPerWeek = goal.WorkoutsPerWeek.ToString();
        WorkoutGoalDurationMinutes = goal.Duration.TotalMinutes.ToString("F0");
        WorkoutGoalType = localizationManager[$"WorkoutType_{goal.WorkoutType}"];
    }

    [RelayCommand]
    private void OpenEditWorkoutGoal()
    {
        if (_currentWorkoutGoal == null) return;
        EditWorkoutsPerWeek = _currentWorkoutGoal.WorkoutsPerWeek.ToString();
        EditDurationMinutes = _currentWorkoutGoal.Duration.TotalMinutes.ToString("F0");
        EditWorkoutType = WorkoutTypes.FirstOrDefault(x => x.Value == _currentWorkoutGoal.WorkoutType) ??
                          new WorkoutTypeOption();
        IsEditWorkoutGoalPopupVisible = true;
    }

    [RelayCommand]
    private void CloseEditWorkout()
    {
        IsEditWorkoutGoalPopupVisible = false;
    }

    [RelayCommand]
    private async Task SaveWorkout()
    {
        if (!int.TryParse(EditWorkoutsPerWeek, out var freq) ||
            !double.TryParse(EditDurationMinutes, CultureInfo.InvariantCulture, out var duration))
        {
            await alertService.ShowToastAsync(localizationManager["Error_InvalidInput"]);
            return;
        }

        if (freq is < 1 or > 7)
        {
            await alertService.ShowToastAsync(localizationManager["Error_WorkoutFrequency"]);
            return;
        }

        if (duration is < 5 or > 300)
        {
            await alertService.ShowToastAsync(localizationManager["Error_WorkoutDuration"]);
            return;
        }

        IsEditWorkoutGoalPopupVisible = false;
        IsLoading = true;

        try
        {
            var request = new WorkoutGoalCreateRequest(freq, TimeSpan.FromMinutes(duration), EditWorkoutType.Value);
            var result = await goalService.UpdateWorkoutGoal(request, CancellationToken.None);

            if (result is { Success: true, Data: not null })
            {
                UpdateState(result.Data);
                await alertService.ShowToastAsync(localizationManager["Success_WorkoutUpdated"]);
                OnGoalUpdated?.Invoke();
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

    public event Action? OnGoalUpdated;
}

public class WorkoutTypeOption
{
    public WorkoutType Value { get; set; }
    public string DisplayName { get; set; } = string.Empty;
}