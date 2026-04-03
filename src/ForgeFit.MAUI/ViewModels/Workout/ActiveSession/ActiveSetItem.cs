using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ForgeFit.MAUI.Models.DTOs.Workout;
using ForgeFit.MAUI.Models.Enums.ProfileEnums;

namespace ForgeFit.MAUI.ViewModels.Workout.ActiveSession;

public partial class ActiveSetItem : ObservableObject
{
    private readonly Action<ActiveSetItem> _onCompletedChanged;

    public Guid Id { get; }

    [ObservableProperty] private int _order;

    [ObservableProperty] private int _reps;

    partial void OnRepsChanged(int value)
    {
        Reps = value switch
        {
            < 0 => 0,
            > 100 => 100,
            _ => Reps
        };
    }

    [ObservableProperty] private double _weight;

    partial void OnWeightChanged(double value)
    {
        Weight = value switch
        {
            < 0 => 0,
            > 1500 => 1500,
            _ => Weight
        };
    }

    [ObservableProperty] private TimeSpan _restTime;

    partial void OnRestTimeChanged(TimeSpan value)
    {
        if (value.TotalMinutes >= 10) RestTime = TimeSpan.FromMinutes(9).Add(TimeSpan.FromSeconds(59));
    }

    [ObservableProperty] private WeightUnit _weightUnit;

    [ObservableProperty] private bool _isCompleted;

    public IRelayCommand DeleteCommand { get; }

    partial void OnIsCompletedChanged(bool value)
    {
        _onCompletedChanged(this);
    }

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