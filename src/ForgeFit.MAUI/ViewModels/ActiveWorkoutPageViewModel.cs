using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ForgeFit.MAUI.Services.Interfaces;
using ForgeFit.MAUI.ViewModels.Workout;
using ForgeFit.MAUI.Views.Workout;
using LocalizationResourceManager.Maui;

namespace ForgeFit.MAUI.ViewModels;

public partial class ActiveWorkoutPageViewModel : BaseViewModel, IQueryAttributable
{
    private readonly IWorkoutProgramService _workoutProgramService;
    private readonly IAlertService _alertService;
    private readonly ILocalizationResourceManager _localizationManager;

    private bool _isInitialized;
    private string _programName = string.Empty;
    private string? _programDescription;

    public WorkoutTimerViewModel TimerVM { get; }
    public ExerciseSessionViewModel ExerciseVM { get; }
    public PopupManagerViewModel PopupVM { get; }
    public WorkoutCompletionViewModel CompletionVM { get; }

    [ObservableProperty] private Guid _programId;

    public ActiveWorkoutPageViewModel(
        IWorkoutProgramService workoutProgramService,
        IWorkoutTrackingService workoutTrackingService,
        IAlertService alertService,
        ILocalizationResourceManager localizationManager)
    {
        _workoutProgramService = workoutProgramService;
        _alertService = alertService;
        _localizationManager = localizationManager;

        TimerVM = new WorkoutTimerViewModel();
        PopupVM = new PopupManagerViewModel(localizationManager);
        CompletionVM = new WorkoutCompletionViewModel(workoutTrackingService, workoutProgramService, alertService, localizationManager);
        ExerciseVM = new ExerciseSessionViewModel(alertService, localizationManager, ProgramId, OnSetCompleted);

        ExerciseVM.DeleteExerciseRequested += OnDeleteExerciseRequested;
        PopupVM.DurationValidationError += OnDurationValidationError;
        PopupVM.DurationCorrectionSaved += OnDurationCorrectionSaved;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (_isInitialized || !query.TryGetValue(nameof(ProgramId), out var id) || id is not string idStr ||
            !Guid.TryParse(idStr, out var guid)) return;
        
        ProgramId = guid;
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
            var result = await _workoutProgramService.GetProgramAsync(ProgramId);
            if (!result.Success || result.Data == null)
            {
                var errorMsg = new LocalizedString(() => result.Message);
                HandleError(errorMsg);
                return;
            }

            _programName = result.Data.Name;
            _programDescription = result.Data.Description;

            ExerciseVM.InitializeExercises(result.Data.WorkoutExercisePlans);

            TimerVM.StartTimer();
            _isInitialized = true;
        }
        catch (Exception)
        {
            var genericError = new LocalizedString(() => _localizationManager["UnexpectedErrorMessage"]);
            HandleError(genericError);
        }
        finally
        {
            IsLoading = false;
        }
    }

    
    private void OnSetCompleted(ActiveSetItem set)
    {
        if (set.IsCompleted)
        {
            TimerVM.SetRestTime(set.RestTime);
        }
        else
        {
            TimerVM.ClearRestTime();
        }
    }

    private void OnDeleteExerciseRequested(ActiveExerciseItem exercise)
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
    private async Task AskFinishWorkout()
    {
        var isValid = await CompletionVM.ValidateWorkoutCompletion(ExerciseVM, TimerVM);
        if (!isValid)
        {
            PopupVM.ShowDurationEntry(TimerVM.TotalWorkoutDuration);
            return;
        }

        ShowFinishConfirmation();
    }

    private void OnDurationValidationError(string errorKey)
    {
        _alertService.ShowToastAsync(_localizationManager[errorKey]);
    }

    private async void OnDurationCorrectionSaved(TimeSpan correctedDuration)
    {
        var isValid = await CompletionVM.ValidateDuration(correctedDuration);
        if (!isValid) return;
        
        TimerVM.SetTotalDuration(correctedDuration);
        await FinishWorkoutInternal();
    }

    private void ShowFinishConfirmation()
    {
        PopupVM.ShowConfirmation(
            "Title_FinishWorkout",
            "Msg_FinishWorkoutConfirm",
            FinishWorkoutInternal);
    }

    [RelayCommand]
    private async Task ConfirmAction()
    {
        await PopupVM.ConfirmActionCommand.ExecuteAsync(null);
    }

    [RelayCommand]
    private void CloseConfirmationPopup()
    {
        PopupVM.CloseConfirmationPopupCommand.Execute(null);
    }

    private async Task FinishWorkoutInternal()
    {
        TimerVM.StopTimer();
        var success = await CompletionVM.FinishWorkout(ProgramId, _programName, _programDescription, ExerciseVM, TimerVM);
        
        if (!success)
        {
            TimerVM.StartTimer();
        }
    }

    [RelayCommand]
    private async Task AddExercise()
    {
        await Shell.Current.GoToAsync($"{nameof(ExerciseSearchPageView)}?ProgramName={_programName}");
    }

    [RelayCommand]
    private void AskCancelWorkout()
    {
        PopupVM.ShowConfirmation(
            "Title_CancelWorkout",
            "Msg_CancelWorkoutConfirm",
            CancelWorkoutInternal);
    }

    private async Task CancelWorkoutInternal()
    {
        TimerVM.StopTimer();
        await Shell.Current.GoToAsync("..");
    }

    private void HandleError(LocalizedString errorMsg)
    {
        if (ExerciseVM.HasExercises())
            _alertService.ShowToastAsync(errorMsg.Localized);
        else
            Error = errorMsg;
    }
}

