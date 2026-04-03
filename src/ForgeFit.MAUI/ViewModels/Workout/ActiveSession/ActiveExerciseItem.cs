using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ForgeFit.MAUI.Models.DTOs.Workout;

namespace ForgeFit.MAUI.ViewModels.Workout.ActiveSession;

public class ActiveExerciseItem : ObservableObject
{
    public Guid Id { get; }
    public WorkoutExerciseDto WorkoutExercise { get; }
    public ObservableCollection<ActiveSetItem> Sets { get; }

    public IRelayCommand DeleteExerciseCommand { get; }
    public IRelayCommand AddSetCommand { get; }

    public ActiveExerciseItem(
        WorkoutExercisePlanDto dto,
        Action<ActiveSetItem> onSetAction,
        IRelayCommand deleteSetCommand,
        IRelayCommand deleteExerciseCommand,
        IRelayCommand addSetCommand)
    {
        Id = dto.Id;
        WorkoutExercise = dto.WorkoutExercise;
        DeleteExerciseCommand = deleteExerciseCommand;
        AddSetCommand = addSetCommand;

        Sets = new ObservableCollection<ActiveSetItem>(
            dto.WorkoutSets.OrderBy(s => s.Order)
                .Select(s => new ActiveSetItem(s, onSetAction, deleteSetCommand))
        );
    }

    public PerformedExerciseDto ToPerformedExerciseDto()
    {
        return new PerformedExerciseDto(
            WorkoutExercise,
            Sets.Select(s => s.ToPerformedSetDto()).ToList()
        );
    }
}