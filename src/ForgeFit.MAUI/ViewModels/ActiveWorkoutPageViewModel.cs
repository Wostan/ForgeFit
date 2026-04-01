using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ForgeFit.MAUI.Messages;
using ForgeFit.MAUI.Models.DTOs.Workout;
using ForgeFit.MAUI.Models.Enums.ProfileEnums;
using ForgeFit.MAUI.Services.Interfaces;
using ForgeFit.MAUI.Views.Workout;
using ForgeFit.MAUI.ViewModels.Workout;
using LocalizationResourceManager.Maui;

namespace ForgeFit.MAUI.ViewModels;

public partial class ActiveWorkoutPageViewModel : BaseViewModel, IQueryAttributable
{
    private bool _isInitialized;
    private string _programName = string.Empty;
    private string? _programDescription;

    private readonly IWorkoutProgramService _workoutProgramService;
    private readonly IWorkoutTrackingService _workoutTrackingService;
    private readonly IAlertService _alertService;
    private readonly ILocalizationResourceManager _localizationManager;

    public WorkoutTimerViewModel TimerVM { get; }
    public ExerciseSessionViewModel ExerciseVM { get; }
    public PopupManagerViewModel PopupVM { get; }

    [ObservableProperty] private Guid _programId;

    public ActiveWorkoutPageViewModel(
        IWorkoutProgramService workoutProgramService,
        IWorkoutTrackingService workoutTrackingService,
        IAlertService alertService,
        ILocalizationResourceManager localizationManager)
    {
        _workoutProgramService = workoutProgramService;
        _workoutTrackingService = workoutTrackingService;
        _alertService = alertService;
        _localizationManager = localizationManager;

        TimerVM = new WorkoutTimerViewModel();
        PopupVM = new PopupManagerViewModel(localizationManager);
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
    private void AskFinishWorkout()
    {
        if (!ExerciseVM.HasExercises())
        {
            _alertService.ShowToastAsync(_localizationManager["Error_NoExercisesInWorkout"]);
            return;
        }

        if (ExerciseVM.HasExercisesWithoutSets())
        {
            _alertService.ShowToastAsync(_localizationManager["Error_ExerciseWithoutSets"]);
            return;
        }

        var totalMinutes = TimerVM.TotalWorkoutDuration.TotalMinutes;

        if (totalMinutes is < 10 or > 300)
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
        IsLoading = true;

        try
        {
            var endTime = TimeOnly.FromDateTime(DateTime.Now);
            var startTime = endTime.Add(-TimerVM.TotalWorkoutDuration);

            var workoutEntry = new WorkoutEntryDto(
                Guid.NewGuid(),
                ProgramId,
                startTime,
                endTime,
                ExerciseVM.GetPerformedExercises(),
                ExerciseVM.CalculateTotalVolume(),
                ExerciseVM.CalculateTotalReps()
            );

            var updatedPlans = ExerciseVM.GetUpdatedPlans();

            var programUpdateRequest = new WorkoutProgramRequest(
                _programName,
                _programDescription,
                updatedPlans
            );

            var logTask = _workoutTrackingService.LogEntryAsync(workoutEntry);
            var updateTask = _workoutProgramService.UpdateProgramAsync(ProgramId, programUpdateRequest);

            await Task.WhenAll(logTask, updateTask);

            string? errorMessage = null;

            if (!logTask.Result.Success)
                errorMessage = logTask.Result.Message;
            else if (!updateTask.Result.Success)
                errorMessage = updateTask.Result.Message;

            if (errorMessage != null)
            {
                var errorMsg = new LocalizedString(() => errorMessage);
                await _alertService.ShowToastAsync(errorMsg.Localized);
                return;
            }

            await Shell.Current.GoToAsync("..");
        }
        catch (Exception)
        {
            var genericError = new LocalizedString(() => _localizationManager["UnexpectedErrorMessage"]);
            await _alertService.ShowToastAsync(genericError.Localized);
            TimerVM.StartTimer();
        }
        finally
        {
            IsLoading = false;
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

public partial class ActiveSetItem : ObservableObject
{
    private readonly Action<ActiveSetItem> _onCompletedChanged;

    public Guid Id { get; }

    [ObservableProperty] private int _order;

    [ObservableProperty] private int _reps;

    partial void OnRepsChanged(int value)
    {
        Reps = value switch
        {
            < 0 => 0,
            > 100 => 100,
            _ => Reps
        };
    }

    [ObservableProperty] private double _weight;

    partial void OnWeightChanged(double value)
    {
        Weight = value switch
        {
            < 0 => 0,
            > 1500 => 1500,
            _ => Weight
        };
    }

    [ObservableProperty] private TimeSpan _restTime;

    partial void OnRestTimeChanged(TimeSpan value)
    {
        if (value.TotalMinutes >= 10) RestTime = TimeSpan.FromMinutes(9).Add(TimeSpan.FromSeconds(59));
    }

    [ObservableProperty] private WeightUnit _weightUnit;

    [ObservableProperty] private bool _isCompleted;

    public IRelayCommand DeleteCommand { get; }

    partial void OnIsCompletedChanged(bool value)
    {
        _onCompletedChanged(this);
    }

    public ActiveSetItem(
        WorkoutSetDto dto,
        Action<ActiveSetItem> onCompletedChanged,
        IRelayCommand deleteCommand)
    {
        Id = dto.Id;
        Order = dto.Order;
        Reps = dto.Reps;
        Weight = dto.Weight;
        RestTime = dto.RestTime;
        WeightUnit = dto.WeightUnit;
        _onCompletedChanged = onCompletedChanged;
        DeleteCommand = deleteCommand;
    }

    [RelayCommand]
    private void ToggleComplete()
    {
        IsCompleted = !IsCompleted;
    }

    public PerformedSetDto ToPerformedSetDto()
    {
        return new PerformedSetDto(Order, Reps, Weight, WeightUnit, IsCompleted);
    }

    public WorkoutSetDto ToDto()
    {
        return new WorkoutSetDto(Id, Order, Reps, RestTime, Weight, WeightUnit);
    }
}
