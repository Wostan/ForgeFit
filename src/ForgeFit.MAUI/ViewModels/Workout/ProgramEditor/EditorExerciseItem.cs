using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ForgeFit.MAUI.Models.DTOs.Workout;

namespace ForgeFit.MAUI.ViewModels.Workout.ProgramEditor;

public class EditorExerciseItem : ObservableObject
{
    public EditorExerciseItem(
        WorkoutExercisePlanDto dto,
        IRelayCommand deleteSetCommand,
        IRelayCommand deleteExerciseCommand,
        IRelayCommand addSetCommand)
    {
        Id = dto.Id;
        WorkoutExercise = dto.WorkoutExercise;
        DeleteExerciseCommand = deleteExerciseCommand;
        AddSetCommand = addSetCommand;

        Sets = new ObservableCollection<EditorSetItem>(
            dto.WorkoutSets.OrderBy(s => s.Order)
                .Select(s => new EditorSetItem(s, deleteSetCommand))
        );
    }

    public Guid Id { get; }
    public WorkoutExerciseDto WorkoutExercise { get; }
    public ObservableCollection<EditorSetItem> Sets { get; }

    public IRelayCommand DeleteExerciseCommand { get; }
    public IRelayCommand AddSetCommand { get; }
}
