using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ForgeFit.MAUI.Messages;
using ForgeFit.MAUI.Models.DTOs.Workout;
using ForgeFit.MAUI.Services.Interfaces;
using ForgeFit.MAUI.ViewModels.Core;
using ForgeFit.MAUI.Views.Workout;
using LocalizationResourceManager.Maui;

namespace ForgeFit.MAUI.ViewModels.Workout.ProgramEditor;

public partial class WorkoutProgramEditorPageViewModel : BaseViewModel, IQueryAttributable
{
    private readonly IAlertService _alertService;
    private readonly ILocalizationResourceManager _localizationManager;
    private readonly IWorkoutProgramService _workoutProgramService;
    [ObservableProperty] private ExerciseEditorViewModel _exerciseVM;

    private bool _isInitialized;

    public WorkoutProgramEditorPageViewModel(
        IWorkoutProgramService workoutProgramService,
        IAlertService alertService,
        ILocalizationResourceManager localizationManager)
    {
        _workoutProgramService = workoutProgramService;
        _alertService = alertService;
        _localizationManager = localizationManager;

        ProgramVM = new ProgramManagerViewModel(workoutProgramService, alertService, localizationManager);
        ExerciseVM = new ExerciseEditorViewModel(alertService, localizationManager, Guid.Empty);
        PopupVM = new PopupManagerViewModel(localizationManager);

        ExerciseVM.DeleteExerciseRequested += OnDeleteExerciseRequested;

        WeakReferenceMessenger.Default.Register<AddExerciseMessage>(this,
            (r, m) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    ((WorkoutProgramEditorPageViewModel)r).AddNewExercise(m.Value);
                });
            });
    }

    public ProgramManagerViewModel ProgramVM { get; }
    public PopupManagerViewModel PopupVM { get; }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (_isInitialized || !query.TryGetValue("ProgramId", out var id)
                           || id is not string idStr
                           || !Guid.TryParse(idStr, out var guid)) return;

        ProgramVM.ProgramId = guid;
        ExerciseVM = new ExerciseEditorViewModel(_alertService, _localizationManager, guid);
        ExerciseVM.DeleteExerciseRequested += OnDeleteExerciseRequested;
        InitializeCommand.Execute(null);
    }

    [RelayCommand]
    private async Task Initialize()
    {
        if (_isInitialized) return;

        IsLoading = true;
        Error = null;
        try
        {
            var program = await ProgramVM.LoadProgramAsync(ProgramVM.ProgramId);
            if (program != null)
            {
                ExerciseVM.InitializeExercises(program.WorkoutExercisePlans);
                _isInitialized = true;
            }
        }
        catch (Exception)
        {
            HandleError(new LocalizedString(() => _localizationManager["UnexpectedErrorMessage"]));
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void OpenRenamePopup()
    {
        PopupVM.ShowRenamePopup(ProgramVM.ProgramName);
    }

    [RelayCommand]
    private async Task ConfirmRename()
    {
        var isValid = await ProgramVM.ValidateProgramNameAsync(PopupVM.TempProgramName);
        if (!isValid) return;

        ProgramVM.UpdateProgramName(PopupVM.TempProgramName);
        PopupVM.CloseRenamePopupCommand.Execute(null);
    }


    private void AddNewExercise(WorkoutExerciseDto exerciseDto)
    {
        ExerciseVM.AddNewExercise(exerciseDto);
    }

    [RelayCommand]
    private async Task AddExercise()
    {
        await Shell.Current.GoToAsync($"{nameof(ExerciseSearchPageView)}?ProgramName={ProgramVM.ProgramName}");
    }


    private void OnDeleteExerciseRequested(EditorExerciseItem exercise)
    {
        PopupVM.ShowConfirmation(
            "Title_DeleteExercise",
            "Msg_DeleteExerciseConfirm",
            async () =>
            {
                ExerciseVM.Exercises.Remove(exercise);
                await Task.CompletedTask;
            });
    }

    [RelayCommand]
    private async Task SaveProgram()
    {
        var exercisePlans = ExerciseVM.GetExercisePlans();
        var success = await ProgramVM.SaveProgramAsync(exercisePlans);

        if (success)
        {
            WeakReferenceMessenger.Default.Send(new WorkoutProgramChangedMessage());
            await Shell.Current.GoToAsync("..");
        }
    }

    [RelayCommand]
    private void AskCancel()
    {
        PopupVM.ShowConfirmation(
            "Title_UnsavedChanges",
            "Msg_UnsavedChangesConfirm",
            async () => { await Shell.Current.GoToAsync(".."); });
    }


    private void HandleError(LocalizedString errorMsg)
    {
        Error = errorMsg;
    }
}