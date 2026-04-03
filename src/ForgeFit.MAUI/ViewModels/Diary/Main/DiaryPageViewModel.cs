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
    private readonly IGoalService _goalService;
    private readonly ILocalizationResourceManager _localizationManager;
    private CancellationTokenSource? _cts;
    [ObservableProperty] private string _dateTitle = string.Empty;

    private bool _isInitialized;

    [ObservableProperty] private bool _isRefreshing;
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
        _goalService = goalService;
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

        WeakReferenceMessenger.Default.Register<DiaryPageViewModel, DiaryUpdatedMessage>(
            this,
            (r, _) => { r.RefreshCommand.ExecuteAsync(null); }
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

        _ = LoadAsync(_cts.Token);
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
            var nutritionGoalTask = NutritionVM.LoadNutritionGoalsAsync(token);
            var foodTask = _diaryService.GetEntriesByDateAsync(SelectedDate, token);
            var waterTask = WaterVM.LoadWaterEntriesAsync(SelectedDate, token);
            var weightTask = WeightVM.LoadWeightDataAsync(token);

            await Task.WhenAll(nutritionGoalTask, foodTask, waterTask, weightTask);
            if (token.IsCancellationRequested) return;

            var foodResult = await foodTask;
            if (foodResult is { Success: true, Data: not null })
            {
                var entries = foodResult.Data;
                NutritionVM.UpdateCurrentNutrition(entries);
                MealVM.UpdateMealItems(entries, NutritionVM.TargetCalories);
            }

            var waterGoal = await _goalService.GetNutritionGoal(token);
            if (waterGoal is { Success: true, Data: not null }) WaterVM.SetWaterGoal(waterGoal.Data.WaterGoalMl);

            _isInitialized = true;
            Error = null;
        }
        catch (OperationCanceledException)
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
        var humanized = SelectedDate.Humanize(
            dateToCompareAgainst: DateTime.Today,
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
    private async Task GoToMealDetails(DayTime mealType)
    {
        await MealVM.GoToMealDetailsCommand.ExecuteAsync(mealType);
    }
}