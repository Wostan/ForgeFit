using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ForgeFit.MAUI.Models.DTOs.Workout;
using ForgeFit.MAUI.Services.Interfaces;
using ForgeFit.MAUI.ViewModels.Core;
using ForgeFit.MAUI.Views.Workout;
using LocalizationResourceManager.Maui;

namespace ForgeFit.MAUI.ViewModels.Workout.ProgramEditor;

public partial class WorkoutProgramManagerViewModel(
    IWorkoutProgramService workoutProgramService,
    IAlertService alertService,
    ILocalizationResourceManager localizationManager)
    : BaseViewModel
{
    [ObservableProperty] private ObservableCollection<WorkoutProgramItem> _programs = [];

    public async Task LoadProgramsAsync(CancellationToken token = default)
    {
        try
        {
            var programsTask = workoutProgramService.GetAllProgramsAsync();
            await programsTask;

            if (token.IsCancellationRequested) return;

            if (programsTask.Result is { Success: true, Data: not null })
            {
                var viewModels = programsTask.Result.Data
                    .Select(dto => new WorkoutProgramItem(dto));
                Programs = new ObservableCollection<WorkoutProgramItem>(viewModels);
            }
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception)
        {
            if (!token.IsCancellationRequested)
                throw;
        }
    }

    public void ResetPrograms()
    {
        Programs.Clear();
    }

    [RelayCommand]
    private async Task CreateProgramAsync(string programName)
    {
        if (string.IsNullOrWhiteSpace(programName)) return;

        if (programName.Length > 50)
        {
            await alertService.ShowToastAsync(localizationManager["Error_ProgramNameTooLong"]);
            return;
        }

        IsLoading = true;

        try
        {
            var request = new WorkoutProgramRequest(programName, null, []);
            var result = await workoutProgramService.CreateProgramAsync(request);

            if (result is { Success: true, Data: not null })
            {
                Programs.Insert(0, new WorkoutProgramItem(result.Data));
                await Shell.Current.GoToAsync($"{nameof(WorkoutProgramEditorPageView)}?ProgramId={result.Data.Id}");
            }
            else
            {
                var errorMsg = new LocalizedString(() => result.Message);
                await alertService.ShowToastAsync(errorMsg.Localized);
            }
        }
        catch (Exception)
        {
            await alertService.ShowToastAsync(localizationManager["UnexpectedErrorMessage"]);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task DeleteProgramAsync(WorkoutProgramItem item)
    {
        IsLoading = true;
        try
        {
            var result = await workoutProgramService.DeleteProgramAsync(item.Program.Id);
            if (result.Success)
            {
                Programs.Remove(item);
            }
            else
            {
                var errorMsg = new LocalizedString(() => result.Message);
                await alertService.ShowToastAsync(errorMsg.Localized);
            }
        }
        catch (Exception)
        {
            await alertService.ShowToastAsync(localizationManager["UnexpectedErrorMessage"]);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task GoToEditProgramAsync(WorkoutProgramItem item)
    {
        await Shell.Current.GoToAsync($"{nameof(WorkoutProgramEditorPageView)}?ProgramId={item.Program.Id}");
    }

    [RelayCommand]
    private async Task StartProgramAsync(WorkoutProgramItem item)
    {
        await Shell.Current.GoToAsync($"{nameof(ActiveWorkoutPageView)}?ProgramId={item.Program.Id}");
    }
}

public class WorkoutProgramItem(WorkoutProgramResponse program)
{
    public WorkoutProgramResponse Program { get; } = program;

    public IEnumerable<WorkoutExercisePlanDto> Top3Exercises =>
        Program.WorkoutExercisePlans.Take(3);

    public int RemainingCount =>
        Math.Max(0, Program.WorkoutExercisePlans.Count - 3);

    public bool HasMore => RemainingCount > 0;
}