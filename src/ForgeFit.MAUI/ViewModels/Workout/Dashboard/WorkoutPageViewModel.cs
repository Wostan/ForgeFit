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
    private bool _isProgramsDirty = true;

    [ObservableProperty] private bool _isRefreshing;
    private bool _isWorkoutEntriesDirty = true;

    private bool _isWorkoutGoalDirty = true;

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

        SetLoadingState();

        WeakReferenceMessenger.Default.Register<WorkoutPageViewModel, WorkoutGoalChangedMessage>(
            this,
            (r, _) => { r._isWorkoutGoalDirty = true; }
        );

        WeakReferenceMessenger.Default.Register<WorkoutPageViewModel, WorkoutProgramChangedMessage>(
            this,
            (r, _) => { r._isProgramsDirty = true; }
        );
    }

    public WorkoutStatsViewModel StatsVM { get; }
    public WorkoutProgramManagerViewModel ProgramManagerVM { get; }
    public PopupManagerViewModel PopupVM { get; }

    [RelayCommand]
    private async Task Initialize()
    {
        _cts?.Cancel();
        _cts = new CancellationTokenSource();
        _isWorkoutGoalDirty = true;
        _isWorkoutEntriesDirty = true;
        _isProgramsDirty = true;
        await CheckAndRefreshAsync();
    }

    private async Task LoadWorkoutGoalAsync(CancellationToken token)
    {
        try
        {
            await StatsVM.LoadGoalAsync(token);
        }
        catch (TaskCanceledException)
        {
        }
        catch (Exception)
        {
            if (!token.IsCancellationRequested)
            {
                var genericError = new LocalizedString(() => _localizationManager["UnexpectedErrorMessage"]);
                HandleError(genericError);
            }
        }
    }

    private async Task LoadWorkoutEntriesAsync(CancellationToken token)
    {
        try
        {
            await StatsVM.LoadEntriesAsync(token);
        }
        catch (TaskCanceledException)
        {
        }
        catch (Exception)
        {
            if (!token.IsCancellationRequested)
            {
                var genericError = new LocalizedString(() => _localizationManager["UnexpectedErrorMessage"]);
                HandleError(genericError);
            }
        }
    }

    private async Task LoadProgramsAsync(CancellationToken token)
    {
        try
        {
            await ProgramManagerVM.LoadProgramsAsync(token);
        }
        catch (TaskCanceledException)
        {
        }
        catch (Exception)
        {
            if (!token.IsCancellationRequested)
            {
                var genericError = new LocalizedString(() => _localizationManager["UnexpectedErrorMessage"]);
                HandleError(genericError);
            }
        }
    }

    public async Task CheckAndRefreshAsync()
    {
        if (!_isInitialized)
        {
            IsLoading = true;
            Error = null;
        }

        try
        {
            if (_isWorkoutGoalDirty)
            {
                await LoadWorkoutGoalAsync(_cts?.Token ?? CancellationToken.None);
                if (_cts?.Token.IsCancellationRequested ?? false) return;
                _isWorkoutGoalDirty = false;
            }

            if (_isWorkoutEntriesDirty)
            {
                await LoadWorkoutEntriesAsync(_cts?.Token ?? CancellationToken.None);
                if (_cts?.Token.IsCancellationRequested ?? false) return;
                _isWorkoutEntriesDirty = false;
            }

            if (_isProgramsDirty)
            {
                await LoadProgramsAsync(_cts?.Token ?? CancellationToken.None);
                if (_cts?.Token.IsCancellationRequested ?? false) return;
                _isProgramsDirty = false;
            }

            _isInitialized = true;
            Error = null;
        }
        catch (TaskCanceledException)
        {
        }
        catch (Exception)
        {
            if (!(_cts?.Token.IsCancellationRequested ?? false))
            {
                var genericError = new LocalizedString(() => _localizationManager["UnexpectedErrorMessage"]);
                HandleError(genericError);
            }
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

        _isWorkoutGoalDirty = true;
        _isWorkoutEntriesDirty = true;
        _isProgramsDirty = true;

        try
        {
            await CheckAndRefreshAsync();
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
