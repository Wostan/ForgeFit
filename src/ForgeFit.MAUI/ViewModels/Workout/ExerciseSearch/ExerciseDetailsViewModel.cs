using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ForgeFit.MAUI.Models.DTOs.Workout;

namespace ForgeFit.MAUI.ViewModels.Workout.ExerciseSearch;

public partial class ExerciseDetailsViewModel : ObservableObject
{
    [ObservableProperty] private bool _isDetailsVisible;
    [ObservableProperty] private WorkoutExerciseDto? _selectedExerciseDetail;

    public Func<WorkoutExerciseDto, Task>? AddFromDetailsCallback { get; set; }

    [RelayCommand]
    private void CloseDetails()
    {
        IsDetailsVisible = false;
        SelectedExerciseDetail = null;
    }

    [RelayCommand]
    private async Task AddFromDetails()
    {
        if (SelectedExerciseDetail == null || AddFromDetailsCallback == null) return;

        IsDetailsVisible = false;
        await AddFromDetailsCallback(SelectedExerciseDetail);
    }

    public void OpenDetails(WorkoutExerciseDto exercise)
    {
        SelectedExerciseDetail = exercise;
        IsDetailsVisible = true;
    }

    public void ResetState()
    {
        IsDetailsVisible = false;
        SelectedExerciseDetail = null;
    }
}
