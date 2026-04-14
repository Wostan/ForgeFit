using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ForgeFit.MAUI.Constants;
using ForgeFit.MAUI.Models.DTOs.Workout;
using ForgeFit.MAUI.Models.Enums.ProfileEnums;

namespace ForgeFit.MAUI.ViewModels.Workout.ProgramEditor;

public partial class EditorSetItem : ObservableObject
{
    [ObservableProperty] private int _order;

    [ObservableProperty] private int _reps;

    [ObservableProperty] private TimeSpan _restTime;

    [ObservableProperty] private double _weight;

    [ObservableProperty] private WeightUnit _weightUnit;

    public EditorSetItem(WorkoutSetDto dto, IRelayCommand deleteCommand)
    {
        Id = dto.Id;
        Order = dto.Order;
        Reps = dto.Reps;
        Weight = dto.Weight;
        RestTime = dto.RestTime;
        WeightUnit = dto.WeightUnit;
        DeleteCommand = deleteCommand;
    }

    public Guid Id { get; }

    public IRelayCommand DeleteCommand { get; }

    partial void OnRepsChanged(int value)
    {
        Reps = Math.Clamp(value, 0, AppConstants.ValidationLimits.MaxRepsPerSet);
    }

    partial void OnWeightChanged(double value)
    {
        Weight = Math.Clamp(value, 0, AppConstants.ValidationLimits.MaxWorkoutWeightKg);
    }

    public WorkoutSetDto ToDto()
    {
        return new WorkoutSetDto(Id, Order, Reps, RestTime, Weight, WeightUnit);
    }
}