using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ForgeFit.MAUI.Messages;
using ForgeFit.MAUI.Models.DTOs.Workout;
using ForgeFit.MAUI.Services.Interfaces;
using ForgeFit.MAUI.Views.Workout;
using LocalizationResourceManager.Maui;

namespace ForgeFit.MAUI.ViewModels;

public partial class WorkoutPageViewModel : BaseViewModel
{
    private readonly IWorkoutTrackingService _workoutTrackingService;
    private readonly IWorkoutProgramService _workoutProgramService;
    private readonly IGoalService _goalService;
    private readonly IAlertService _alertService;
    private readonly ILocalizationResourceManager _localizationManager;

    private bool _isInitialized;
    private CancellationTokenSource? _cts;

    [ObservableProperty] private bool _isRefreshing;

    // stats
    [ObservableProperty] [NotifyPropertyChangedFor(nameof(WeeklyProgress))]
    private int _completedWorkouts;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(WeeklyProgress))]
    private int _targetWorkouts;

    public double WeeklyProgress => TargetWorkouts > 0
        ? (double)CompletedWorkouts / TargetWorkouts
        : 0;

    [ObservableProperty] private string _statsSubtitle = string.Empty;

    [ObservableProperty] private ObservableCollection<WorkoutProgramResponse> _programs = [];

    public WorkoutPageViewModel(
        IWorkoutTrackingService workoutTrackingService,
        IWorkoutProgramService workoutProgramService,
        IGoalService goalService,
        IAlertService alertService,
        ILocalizationResourceManager localizationManager)
    {
        _workoutTrackingService = workoutTrackingService;
        _workoutProgramService = workoutProgramService;
        _goalService = goalService;
        _alertService = alertService;
        _localizationManager = localizationManager;

        WeakReferenceMessenger.Default.Register<WorkoutPageViewModel, WorkoutDataUpdatedMessage>(
            this,
            (r, _) => { r.RefreshCommand.Execute(null); }
        );

        SetLoadingState();
    }

    [RelayCommand]
    private async Task Initialize()
    {
        _cts?.Cancel();
        _cts = new CancellationTokenSource();
        await LoadAsync(_cts.Token);
    }

    private async Task LoadAsync(CancellationToken token = default)
    {
        if (!_isInitialized)
        {
            IsLoading = true;
            Error = null;
        }

        try
        {
            var today = DateTime.Today;


            var daysSinceMonday = ((int)today.DayOfWeek - 1 + 7) % 7;
            var monday = today.AddDays(-daysSinceMonday);
            var nextMonday = monday.AddDays(7);

            var goalTask = _goalService.GetWorkoutGoal(token);
            var statsTask = _workoutTrackingService.GetEntriesByDateRangeAsync(monday, nextMonday, token);
            var programsTask = _workoutProgramService.GetAllProgramsAsync();

            await Task.WhenAll(goalTask, statsTask, programsTask);

            if (token.IsCancellationRequested) return;

            string? errorMessage = null;
            if (!goalTask.Result.Success) errorMessage = goalTask.Result.Message;
            else if (!statsTask.Result.Success) errorMessage = statsTask.Result.Message;
            else if (!programsTask.Result.Success) errorMessage = programsTask.Result.Message;

            if (errorMessage != null)
            {
                HandleError(new LocalizedString(() => errorMessage));
                return;
            }

            if (goalTask.Result is { Success: true, Data: not null })
                TargetWorkouts = goalTask.Result.Data.WorkoutsPerWeek;

            if (statsTask.Result is { Success: true, Data: not null }) CompletedWorkouts = statsTask.Result.Data.Count;

            if (programsTask.Result is { Success: true, Data: not null })
                Programs = new ObservableCollection<WorkoutProgramResponse>(programsTask.Result.Data);

            UpdateStatsSubtitle();

            _isInitialized = true;
            Error = null;
        }
        catch (TaskCanceledException)
        {
        }
        catch (Exception)
        {
            if (token.IsCancellationRequested) return;
            var genericError = new LocalizedString(() => _localizationManager["UnexpectedErrorMessage"]);
            HandleError(genericError);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task Refresh()
    {
        if (IsLoading)
        {
            IsRefreshing = false;
            return;
        }

        _cts?.Cancel();
        _cts = new CancellationTokenSource();

        try
        {
            await LoadAsync(_cts.Token);
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    [RelayCommand]
    private async Task GoToCreateProgram()
    {
        // Переход на страницу создания/редактирования
        // await Shell.Current.GoToAsync(nameof(WorkoutProgramEditorView));
        await _alertService.ShowToastAsync("Nav to Create Program (TODO)");
    }

    [RelayCommand]
    private async Task GoToProgramDetails(WorkoutProgramResponse program)
    {
        // await Shell.Current.GoToAsync($"{nameof(WorkoutProgramDetailsView)}?ProgramId={program.Id}");
    }

    [RelayCommand]
    private async Task StartProgram(WorkoutProgramResponse program)
    {
        // await Shell.Current.GoToAsync($"{nameof(ActiveWorkoutSessionView)}?ProgramId={program.Id}");
    }

    private void SetLoadingState()
    {
        CompletedWorkouts = 0;
        TargetWorkouts = 0;
        StatsSubtitle = "-";
        Programs.Clear();
    }

    private void HandleError(LocalizedString errorMsg)
    {
        SetLoadingState();

        if (_isInitialized)
            _alertService.ShowToastAsync(errorMsg.Localized);
        else
            Error = errorMsg;
    }

    private void UpdateStatsSubtitle()
    {
        StatsSubtitle = $"{CompletedWorkouts} / {TargetWorkouts} completed";
    }
}
