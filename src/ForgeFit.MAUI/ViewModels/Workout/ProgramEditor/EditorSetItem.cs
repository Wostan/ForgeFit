using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ForgeFit.MAUI.Models.DTOs.Workout;
using ForgeFit.MAUI.Models.Enums.ProfileEnums;

namespace ForgeFit.MAUI.ViewModels.Workout.ProgramEditor;

public partial class EditorSetItem : ObservableObject
{
    public Guid Id { get; }

    [ObservableProperty] private int _order;

    [ObservableProperty] private int _reps;

    partial void OnRepsChanged(int value)
    {
        Reps = Math.Clamp(value, 0, 100);
    }

    [ObservableProperty] private double _weight;

    partial void OnWeightChanged(double value)
    {
        Weight = Math.Clamp(value, 0, 1500);
    }

    [ObservableProperty] private TimeSpan _restTime;

    [ObservableProperty] private WeightUnit _weightUnit;

    public IRelayCommand DeleteCommand { get; }

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

    public WorkoutSetDto ToDto()
    {
        return new WorkoutSetDto(Id, Order, Reps, RestTime, Weight, WeightUnit);
    }
}