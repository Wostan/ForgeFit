using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ForgeFit.MAUI.Constants;
using ForgeFit.MAUI.Models.DTOs.Workout;
using ForgeFit.MAUI.Models.Enums.ProfileEnums;

namespace ForgeFit.MAUI.ViewModels.Workout.ActiveSession;

public partial class ActiveSetItem : ObservableObject
{
    private readonly Action<ActiveSetItem> _onCompletedChanged;

    [ObservableProperty] private bool _isCompleted;

    [ObservableProperty] private int _order;

    [ObservableProperty] private int _reps;

    [ObservableProperty] private TimeSpan _restTime;

    [ObservableProperty] private double _weight;

    [ObservableProperty] private WeightUnit _weightUnit;

    public ActiveSetItem(
        WorkoutSetDto dto,
        Action<ActiveSetItem> onCompletedChanged,
        IRelayCommand deleteCommand)
    {
        Id = dto.Id;
        Order = dto.Order;
        Reps = dto.Reps;
        Weight = dto.Weight;
        RestTime = dto.RestTime;
        WeightUnit = dto.WeightUnit;
        _onCompletedChanged = onCompletedChanged;
        DeleteCommand = deleteCommand;
    }

    public Guid Id { get; }

    public IRelayCommand DeleteCommand { get; }

    partial void OnRepsChanged(int value)
    {
        Reps = value switch
        {
            < 0 => 0,
            > AppConstants.ValidationLimits.MaxRepsPerSet => AppConstants.ValidationLimits.MaxRepsPerSet,
            _ => Reps
        };
    }

    partial void OnWeightChanged(double value)
    {
        Weight = value switch
        {
            < 0 => 0,
            > AppConstants.ValidationLimits.MaxWorkoutWeightKg => AppConstants.ValidationLimits.MaxWorkoutWeightKg,
            _ => Weight
        };
    }

    partial void OnRestTimeChanged(TimeSpan value)
    {
        if (value.TotalMinutes >= AppConstants.ValidationLimits.MaxRestTimeMinutes) 
            RestTime = TimeSpan.FromMinutes(AppConstants.ValidationLimits.MaxRestTimeMinutes - 1).Add(TimeSpan.FromSeconds(59));
    }

    partial void OnIsCompletedChanged(bool value)
    {
        _onCompletedChanged(this);
    }

    [RelayCommand]
    private void ToggleComplete()
    {
        IsCompleted = !IsCompleted;
    }

    public PerformedSetDto ToPerformedSetDto()
    {
        return new PerformedSetDto(Order, Reps, Weight, WeightUnit, IsCompleted);
    }

    public WorkoutSetDto ToDto()
    {
        return new WorkoutSetDto(Id, Order, Reps, RestTime, Weight, WeightUnit);
    }
}