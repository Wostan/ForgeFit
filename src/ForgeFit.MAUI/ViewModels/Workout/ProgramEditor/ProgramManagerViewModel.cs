using CommunityToolkit.Mvvm.ComponentModel;
using ForgeFit.MAUI.Constants;
using ForgeFit.MAUI.Models.DTOs.Workout;
using ForgeFit.MAUI.Services.Interfaces;
using ForgeFit.MAUI.ViewModels.Core;
using LocalizationResourceManager.Maui;

namespace ForgeFit.MAUI.ViewModels.Workout.ProgramEditor;

public partial class ProgramManagerViewModel(
    IWorkoutProgramService workoutProgramService,
    IAlertService alertService,
    ILocalizationResourceManager localizationManager)
    : BaseViewModel
{
    [ObservableProperty] private string? _programDescription;
    [ObservableProperty] private Guid _programId;
    [ObservableProperty] private string _programName = string.Empty;

    public async Task<WorkoutProgramResponse?> LoadProgramAsync(Guid programId)
    {
        ProgramId = programId;

        try
        {
            var result = await workoutProgramService.GetProgramAsync(ProgramId);
            if (!result.Success || result.Data == null)
            {
                HandleError(new LocalizedString(() => result.Message));
                return null;
            }

            ProgramName = result.Data.Name;
            ProgramDescription = result.Data.Description;
            return result.Data;
        }
        catch (Exception)
        {
            HandleError(new LocalizedString(() => localizationManager["UnexpectedErrorMessage"]));
            return null;
        }
    }

    public async Task<bool> SaveProgramAsync(List<WorkoutExercisePlanDto> exercisePlans)
    {
        if (string.IsNullOrWhiteSpace(ProgramName))
        {
            await alertService.ShowToastAsync(localizationManager["Error_NameRequired"]);
            return false;
        }

        if (ProgramName.Length > AppConstants.ValidationLimits.MaxWorkoutProgramNameLength)
        {
            await alertService.ShowToastAsync(localizationManager["Error_ProgramNameTooLong"]);
            return false;
        }

        if (ProgramDescription?.Length > AppConstants.ValidationLimits.MaxWorkoutProgramDescriptionLength)
        {
            await alertService.ShowToastAsync(localizationManager["Error_ProgramDescriptionTooLong"]);
            return false;
        }

        if (exercisePlans.Count > AppConstants.ValidationLimits.MaxExercisesPerProgram)
        {
            await alertService.ShowToastAsync(localizationManager["Error_TooManyExercises"]);
            return false;
        }

        IsLoading = true;
        try
        {
            var request = new WorkoutProgramRequest(ProgramName, ProgramDescription, exercisePlans);
            var result = await workoutProgramService.UpdateProgramAsync(ProgramId, request);

            if (result.Success)
            {
                return true;
            }
            else
            {
                await alertService.ShowToastAsync(result.Message);
                return false;
            }
        }
        catch (Exception)
        {
            await alertService.ShowToastAsync(localizationManager["UnexpectedErrorMessage"]);
            return false;
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task<bool> ValidateProgramNameAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return false;
        if (name.Length <= AppConstants.ValidationLimits.MaxWorkoutProgramNameLength) return true;

        await alertService.ShowToastAsync(localizationManager["Error_ProgramNameTooLong"]);
        return false;
    }

    public void UpdateProgramName(string name)
    {
        ProgramName = name;
    }

    private void HandleError(LocalizedString errorMsg)
    {
        Error = errorMsg;
    }
}