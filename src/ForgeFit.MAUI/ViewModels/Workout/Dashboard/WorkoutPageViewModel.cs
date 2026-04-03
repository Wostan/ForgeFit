using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ForgeFit.MAUI.Messages;
using ForgeFit.MAUI.Services.Interfaces;
using ForgeFit.MAUI.ViewModels.Core;
using ForgeFit.MAUI.ViewModels.Workout.ProgramEditor;
using LocalizationResourceManager.Maui;

namespace ForgeFit.MAUI.ViewModels.Workout.Dashboard;

public partial class WorkoutPageViewModel : BaseViewModel
{
    private readonly IAlertService _alertService;
    private readonly ILocalizationResourceManager _localizationManager;
    private CancellationTokenSource? _cts;

    private bool _isInitialized;

    [ObservableProperty] private bool _isRefreshing;

    public WorkoutPageViewModel(
        IWorkoutTrackingService workoutTrackingService,
        IWorkoutProgramService workoutProgramService,
        IGoalService goalService,
        IAlertService alertService,
        ILocalizationResourceManager localizationManager)
    {
        _alertService = alertService;
        _localizationManager = localizationManager;

        StatsVM = new WorkoutStatsViewModel(workoutTrackingService, goalService);
        ProgramManagerVM = new WorkoutProgramManagerViewModel(workoutProgramService, alertService, localizationManager);
        PopupVM = new PopupManagerViewModel(localizationManager);

        WeakReferenceMessenger.Default.Register<WorkoutPageViewModel, WorkoutDataUpdatedMessage>(
            this,
            (r, _) => { r.RefreshCommand.Execute(null); }
        );

        SetLoadingState();
    }

    public WorkoutStatsViewModel StatsVM { get; }
    public WorkoutProgramManagerViewModel ProgramManagerVM { get; }
    public PopupManagerViewModel PopupVM { get; }

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
            var statsTask = StatsVM.LoadStatsAsync(token);
            var programsTask = ProgramManagerVM.LoadProgramsAsync(token);

            await Task.WhenAll(statsTask, programsTask);

            if (token.IsCancellationRequested) return;

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
    private void GoToCreateProgram()
    {
        PopupVM.ShowCreatePopup();
    }

    [RelayCommand]
    private async Task ConfirmCreateProgram()
    {
        PopupVM.CloseCreatePopupCommand.Execute(null);
        await ProgramManagerVM.CreateProgramCommand.ExecuteAsync(PopupVM.CreateInputValue);
    }

    [RelayCommand]
    private void AskDeleteProgram(WorkoutProgramItem item)
    {
        PopupVM.ShowConfirmation(
            "Title_DeleteProgram",
            "Msg_DeleteProgramConfirm",
            async () => await ProgramManagerVM.DeleteProgramCommand.ExecuteAsync(item));
    }

    [RelayCommand]
    private async Task GoToEditProgram(WorkoutProgramItem item)
    {
        await ProgramManagerVM.GoToEditProgramCommand.ExecuteAsync(item);
    }

    [RelayCommand]
    private async Task StartProgram(WorkoutProgramItem item)
    {
        await ProgramManagerVM.StartProgramCommand.ExecuteAsync(item);
    }

    private void SetLoadingState()
    {
        StatsVM.ResetStats();
        ProgramManagerVM.ResetPrograms();
    }

    private void HandleError(LocalizedString errorMsg)
    {
        if (_isInitialized)
        {
            _alertService.ShowToastAsync(errorMsg.Localized);
        }
        else
        {
            SetLoadingState();
            Error = errorMsg;
        }
    }
}