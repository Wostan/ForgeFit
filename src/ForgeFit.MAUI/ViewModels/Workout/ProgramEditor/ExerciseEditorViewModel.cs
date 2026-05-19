using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ForgeFit.MAUI.Constants;
using ForgeFit.MAUI.Models.DTOs.Workout;
using ForgeFit.MAUI.Models.Enums.ProfileEnums;
using ForgeFit.MAUI.Services.Interfaces;
using ForgeFit.MAUI.ViewModels.Core;
using LocalizationResourceManager.Maui;

namespace ForgeFit.MAUI.ViewModels.Workout.ProgramEditor;

public partial class ExerciseEditorViewModel(
    IAlertService alertService,
    ILocalizationResourceManager localizationManager,
    Guid programId)
    : BaseViewModel
{
    [ObservableProperty] private ObservableCollection<EditorExerciseItem> _exercises = [];

    public void InitializeExercises(IEnumerable<WorkoutExercisePlanDto> exercisePlans)
    {
        var mappedExercises = exercisePlans
            .Select(plan => new EditorExerciseItem(
                plan,
                DeleteSetCommand,
                AskDeleteExerciseCommand,
                AddSetCommand))
            .ToList();

        Exercises = new ObservableCollection<EditorExerciseItem>(mappedExercises);
    }

    public void AddNewExercise(WorkoutExerciseDto exerciseDto)
    {
        var defaultSet = new WorkoutSetDto(Guid.NewGuid(), 1, AppConstants.DefaultValues.DefaultReps,
            TimeSpan.FromMinutes(AppConstants.Time.DefaultRestTimeMinutes), 0, WeightUnit.Kg);
        var tempPlanDto = new WorkoutExercisePlanDto(Guid.NewGuid(), programId, exerciseDto, [defaultSet]);

        var item = new EditorExerciseItem(
            tempPlanDto,
            DeleteSetCommand,
            AskDeleteExerciseCommand,
            AddSetCommand);

        Exercises.Add(item);
    }

    [RelayCommand]
    private void AddSet(EditorExerciseItem exercise)
    {
        if (exercise.Sets.Count >= AppConstants.ValidationLimits.MaxSetsPerExercise)
        {
            alertService.ShowToastAsync(localizationManager["Error_MaxSetsReached"]);
            return;
        }

        var lastSet = exercise.Sets.LastOrDefault();
        var newOrder = (lastSet?.Order ?? 0) + 1;

        var newSet = new EditorSetItem(new WorkoutSetDto(
                Guid.NewGuid(),
                newOrder,
                lastSet?.Reps ?? AppConstants.DefaultValues.DefaultReps,
                lastSet?.RestTime ?? TimeSpan.FromMinutes(AppConstants.Time.DefaultRestTimeMinutes),
                lastSet?.Weight ?? 0,
                lastSet?.WeightUnit ?? WeightUnit.Kg),
            DeleteSetCommand);

        exercise.Sets.Add(newSet);
    }

    [RelayCommand]
    private void DeleteSet(EditorSetItem set)
    {
        var parentExercise = Exercises.FirstOrDefault(e => e.Sets.Contains(set));
        if (parentExercise == null) return;

        if (parentExercise.Sets.Count <= 1)
        {
            alertService.ShowToastAsync(localizationManager["Error_CannotDeleteLastSet"]);
            return;
        }

        parentExercise.Sets.Remove(set);

        for (var i = 0; i < parentExercise.Sets.Count; i++) parentExercise.Sets[i].Order = i + 1;
    }

    [RelayCommand]
    private void AskDeleteExercise(EditorExerciseItem exercise)
    {
        DeleteExerciseRequested?.Invoke(exercise);
    }

    public List<WorkoutExercisePlanDto> GetExercisePlans()
    {
        return Exercises.Select<EditorExerciseItem, WorkoutExercisePlanDto>(e => new WorkoutExercisePlanDto(
            e.Id,
            programId,
            e.WorkoutExercise,
            e.Sets.Select<EditorSetItem, WorkoutSetDto>(s => s.ToDto()).ToList()
        )).ToList();
    }

    public event Action<EditorExerciseItem>? DeleteExerciseRequested;
}
