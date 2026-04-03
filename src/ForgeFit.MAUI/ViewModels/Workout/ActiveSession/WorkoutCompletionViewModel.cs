using CommunityToolkit.Mvvm.ComponentModel;
using ForgeFit.MAUI.Models.DTOs.Workout;
using ForgeFit.MAUI.Services.Interfaces;
using LocalizationResourceManager.Maui;

namespace ForgeFit.MAUI.ViewModels.Workout.ActiveSession;

public partial class WorkoutCompletionViewModel(
    IWorkoutTrackingService workoutTrackingService,
    IWorkoutProgramService workoutProgramService,
    IAlertService alertService,
    ILocalizationResourceManager localizationManager)
    : ObservableObject
{
    [ObservableProperty] private bool _isLoading;

    public async Task<bool> ValidateWorkoutCompletion(ExerciseSessionViewModel exerciseVM,
        WorkoutTimerViewModel timerVM)
    {
        if (!exerciseVM.HasExercises())
        {
            await alertService.ShowToastAsync(localizationManager["Error_NoExercisesInWorkout"]);
            return false;
        }

        if (exerciseVM.HasExercisesWithoutSets())
        {
            await alertService.ShowToastAsync(localizationManager["Error_ExerciseWithoutSets"]);
            return false;
        }

        var totalMinutes = timerVM.TotalWorkoutDuration.TotalMinutes;

        return totalMinutes is >= 10 and <= 300;
    }

    public async Task<bool> FinishWorkout(
        Guid programId,
        string programName,
        string? programDescription,
        ExerciseSessionViewModel exerciseVM,
        WorkoutTimerViewModel timerVM)
    {
        if (IsLoading) return false;

        IsLoading = true;

        try
        {
            var endTime = TimeOnly.FromDateTime(DateTime.Now);
            var startTime = endTime.Add(-timerVM.TotalWorkoutDuration);

            var workoutEntry = new WorkoutEntryDto(
                Guid.NewGuid(),
                programId,
                startTime,
                endTime,
                exerciseVM.GetPerformedExercises(),
                exerciseVM.CalculateTotalVolume(),
                exerciseVM.CalculateTotalReps()
            );

            var updatedPlans = exerciseVM.GetUpdatedPlans();

            var programUpdateRequest = new WorkoutProgramRequest(
                programName,
                programDescription,
                updatedPlans
            );

            var logTask = workoutTrackingService.LogEntryAsync(workoutEntry);
            var updateTask = workoutProgramService.UpdateProgramAsync(programId, programUpdateRequest);

            await Task.WhenAll(logTask, updateTask);

            string? errorMessage = null;

            if (!logTask.Result.Success)
                errorMessage = logTask.Result.Message;
            else if (!updateTask.Result.Success)
                errorMessage = updateTask.Result.Message;

            if (errorMessage != null)
            {
                var errorMsg = new LocalizedString(() => errorMessage);
                await alertService.ShowToastAsync(errorMsg.Localized);
                return false;
            }

            await Shell.Current.GoToAsync("..");
            return true;
        }
        catch (Exception)
        {
            var genericError = new LocalizedString(() => localizationManager["UnexpectedErrorMessage"]);
            await alertService.ShowToastAsync(genericError.Localized);
            return false;
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task<bool> ValidateDuration(TimeSpan duration)
    {
        var minutes = duration.TotalMinutes;

        if (minutes is < 10 or > 300)
        {
            var errorKey = minutes < 10 ? "Error_DurationTooShort" : "Error_DurationTooLong";
            await alertService.ShowToastAsync(localizationManager[errorKey]);
            return false;
        }

        return true;
    }
}