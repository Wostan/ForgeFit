using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ForgeFit.MAUI.Messages;
using ForgeFit.MAUI.Models.Enums.FoodEnums;
using ForgeFit.MAUI.Services.Interfaces;
using ForgeFit.MAUI.ViewModels.Core;
using ForgeFit.MAUI.ViewModels.Diary.Meals;
using ForgeFit.MAUI.ViewModels.Diary.Tracking;
using Humanizer;
using LocalizationResourceManager.Maui;

namespace ForgeFit.MAUI.ViewModels.Diary.Main;

public partial class DiaryPageViewModel : BaseViewModel
{
    private readonly IAlertService _alertService;
    private readonly IDiaryService _diaryService;
    private readonly ILocalizationResourceManager _localizationManager;
    private CancellationTokenSource? _cts;
    [ObservableProperty] private string _dateTitle = string.Empty;

    private bool _isFoodDirty = true;
    private bool _isGoalsDirty = true;

    private bool _isInitialized;

    [ObservableProperty] private bool _isRefreshing;
    private bool _isWaterDirty = true;
    private bool _isWeightDirty = true;
    [ObservableProperty] private DateTime _selectedDate = DateTime.Today;

    public DiaryPageViewModel(
        IDiaryService diaryService,
        IDrinkTrackingService drinkTrackingService,
        IGoalService goalService,
        IUserService userService,
        IAlertService alertService,
        ILocalizationResourceManager localizationManager)
    {
        _diaryService = diaryService;
        _alertService = alertService;
        _localizationManager = localizationManager;

        NutritionVM = new NutritionTrackingViewModel(goalService);
        WaterVM = new WaterTrackingViewModel(drinkTrackingService, alertService);
        WeightVM = new WeightManagementViewModel(goalService, userService, alertService, localizationManager);
        MealVM = new MealDashboardViewModel();

        MealVM.SetSelectedDate(SelectedDate);

        if (_localizationManager is INotifyPropertyChanged notifyService)
            notifyService.PropertyChanged += (_, _) => UpdateDateTitle();

        UpdateDateTitle();

        WeakReferenceMessenger.Default.Register<DiaryPageViewModel, FoodDataChangedMessage>(
            this,
            (r, _) => { r._isFoodDirty = true; }
        );

        WeakReferenceMessenger.Default.Register<DiaryPageViewModel, NutritionGoalChangedMessage>(
            this,
            (r, _) => { r._isGoalsDirty = true; }
        );

        WeakReferenceMessenger.Default.Register<DiaryPageViewModel, WeightChangedMessage>(
            this,
            (r, msg) =>
            {
                if (msg.Source != nameof(WeightManagementViewModel)) r._isWeightDirty = true;
            }
        );
    }

    public NutritionTrackingViewModel NutritionVM { get; }
    public WaterTrackingViewModel WaterVM { get; }
    public WeightManagementViewModel WeightVM { get; }
    public MealDashboardViewModel MealVM { get; }


    partial void OnSelectedDateChanged(DateTime value)
    {
        MealVM.SetSelectedDate(value);
        ResetDashboard();
        UpdateDateTitle();

        _cts?.Cancel();
        _cts = new CancellationTokenSource();

        _isFoodDirty = true;
        _isWaterDirty = true;
        _isGoalsDirty = true;
        _isWeightDirty = true;
        _ = CheckAndRefreshAsync();
    }

    [RelayCommand]
    private async Task Initialize()
    {
        _cts?.Cancel();
        _cts = new CancellationTokenSource();
        _isFoodDirty = true;
        _isWaterDirty = true;
        _isGoalsDirty = true;
        _isWeightDirty = true;
        await CheckAndRefreshAsync();
    }

    private async Task LoadFoodAsync(CancellationToken token)
    {
        try
        {
            var foodResult = await _diaryService.GetEntriesByDateAsync(SelectedDate, token);
            if (token.IsCancellationRequested) return;

            if (foodResult is { Success: true, Data: not null })
            {
                var entries = foodResult.Data;
                NutritionVM.UpdateCurrentNutrition(entries);
                MealVM.UpdateMealItems(entries, NutritionVM.TargetCalories);
            }
        }
        catch (OperationCanceledException)
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

    private async Task LoadWaterAsync(CancellationToken token)
    {
        try
        {
            await WaterVM.LoadWaterEntriesAsync(SelectedDate, token);
        }
        catch (OperationCanceledException)
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

    private async Task LoadGoalsAsync(CancellationToken token)
    {
        try
        {
            await NutritionVM.LoadNutritionGoalsAsync(token);
            if (token.IsCancellationRequested) return;

            WaterVM.SetWaterGoal(NutritionVM.WaterGoalMl);
        }
        catch (OperationCanceledException)
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

    private async Task LoadWeightAsync(CancellationToken token)
    {
        try
        {
            await WeightVM.LoadWeightDataAsync(token);
        }
        catch (OperationCanceledException)
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
            if (_isGoalsDirty)
            {
                await LoadGoalsAsync(_cts?.Token ?? CancellationToken.None);
                if (_cts?.Token.IsCancellationRequested ?? false) return;
                _isGoalsDirty = false;
            }

            if (_isFoodDirty)
            {
                await LoadFoodAsync(_cts?.Token ?? CancellationToken.None);
                if (_cts?.Token.IsCancellationRequested ?? false) return;
                _isFoodDirty = false;
            }

            if (_isWaterDirty)
            {
                await LoadWaterAsync(_cts?.Token ?? CancellationToken.None);
                if (_cts?.Token.IsCancellationRequested ?? false) return;
                _isWaterDirty = false;
            }

            if (_isWeightDirty)
            {
                await LoadWeightAsync(_cts?.Token ?? CancellationToken.None);
                if (_cts?.Token.IsCancellationRequested ?? false) return;
                _isWeightDirty = false;
            }

            _isInitialized = true;
            Error = null;
        }
        catch (OperationCanceledException)
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

    private void HandleError(LocalizedString errorMsg)
    {
        if (_isInitialized)
        {
            _alertService.ShowToastAsync(errorMsg.Localized);
        }
        else
        {
            ResetDashboard();
            Error = errorMsg;
        }
    }

    private void ResetDashboard()
    {
        NutritionVM.ResetNutrition();
        WaterVM.ResetWater();
        MealVM.ResetMeals();
    }

    private void UpdateDateTitle()
    {
        var selected = DateOnly.FromDateTime(SelectedDate);
        var today = DateOnly.FromDateTime(DateTime.Today);

        var humanized = selected.Humanize(
            dateToCompareAgainst: today,
            culture: _localizationManager.CurrentCulture
        );

        DateTitle = string.IsNullOrWhiteSpace(humanized)
            ? SelectedDate.ToString("D", _localizationManager.CurrentCulture)
            : humanized.Transform(To.SentenceCase);
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

        _isFoodDirty = true;
        _isWaterDirty = true;
        _isGoalsDirty = true;
        _isWeightDirty = true;

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
    private async Task GoToMealDetails(DayTime mealType)
    {
        await MealVM.GoToMealDetailsCommand.ExecuteAsync(mealType);
    }
}
