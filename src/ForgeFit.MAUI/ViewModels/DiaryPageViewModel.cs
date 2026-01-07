using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ForgeFit.MAUI.Messages;
using ForgeFit.MAUI.Models.DTOs.DrinkTracking;
using ForgeFit.MAUI.Models.DTOs.Food;
using ForgeFit.MAUI.Models.Enums.FoodEnums;
using ForgeFit.MAUI.Services.Interfaces;
using ForgeFit.MAUI.Views.Diary;
using Humanizer;
using LocalizationResourceManager.Maui;

namespace ForgeFit.MAUI.ViewModels;

public partial class DiaryPageViewModel : BaseViewModel
{
    private readonly IDiaryService _diaryService;
    private readonly IDrinkTrackingService _drinkTrackingService;
    private readonly IAlertService _alertService;
    private readonly ILocalizationResourceManager _localizationManager;

    private bool _isInitialized;

    [ObservableProperty] private bool _isRefreshing;

    private CancellationTokenSource? _cts;

    [ObservableProperty] private DateTime _selectedDate = DateTime.Today;
    [ObservableProperty] private string _dateTitle = string.Empty;

    [ObservableProperty] private double _currentCalories;
    [ObservableProperty] private double _currentCarbs;
    [ObservableProperty] private double _currentProtein;
    [ObservableProperty] private double _currentFat;

    [ObservableProperty] private double _currentWater;

    [ObservableProperty] private ObservableCollection<DrinkEntryResponse> _waterEntries = [];

    [ObservableProperty] private bool _isWaterInputVisible;

    [NotifyCanExecuteChangedFor(nameof(SaveCustomWaterCommand))] [ObservableProperty]
    private string _waterInputValue = "";

    // TODO: Connect goal
    [ObservableProperty] private double _targetCalories = 2500;
    [ObservableProperty] private double _targetCarbs = 300;
    [ObservableProperty] private double _targetProtein = 180;
    [ObservableProperty] private double _targetFat = 80;
    [ObservableProperty] private double _targetWater = 2500;

    public MealDashboardItem Breakfast { get; } = new(DayTime.Breakfast);
    public MealDashboardItem Lunch { get; } = new(DayTime.Lunch);
    public MealDashboardItem Dinner { get; } = new(DayTime.Dinner);
    public MealDashboardItem Snack { get; } = new(DayTime.Snack);

    public double CaloriesProgress => TargetCalories > 0 ? CurrentCalories / TargetCalories : 0;
    public double CarbsProgress => TargetCarbs > 0 ? CurrentCarbs / TargetCarbs : 0;
    public double ProteinProgress => TargetProtein > 0 ? CurrentProtein / TargetProtein : 0;
    public double FatProgress => TargetFat > 0 ? CurrentFat / TargetFat : 0;
    public double WaterProgress => TargetWater > 0 ? CurrentWater / TargetWater : 0;

    public DiaryPageViewModel(
        IDiaryService diaryService,
        IDrinkTrackingService drinkTrackingService,
        IAlertService alertService,
        ILocalizationResourceManager localizationManager)
    {
        _diaryService = diaryService;
        _drinkTrackingService = drinkTrackingService;
        _alertService = alertService;
        _localizationManager = localizationManager;

        if (_localizationManager is INotifyPropertyChanged notifyService)
            notifyService.PropertyChanged += (_, _) => UpdateDateTitle();

        UpdateDateTitle();

        WeakReferenceMessenger.Default.Register<DiaryPageViewModel, DiaryUpdatedMessage>(
            this,
            (r, _) => { r.RefreshCommand.Execute(null); }
        );
    }

    partial void OnSelectedDateChanged(DateTime value)
    {
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
            var foodTask = _diaryService.GetEntriesByDateAsync(SelectedDate, token);
            var waterTask = _drinkTrackingService.GetEntriesByDateAsync(SelectedDate, token);

            await Task.WhenAll(foodTask, waterTask);

            var foodResult = foodTask.Result;
            var waterResult = waterTask.Result;

            if (token.IsCancellationRequested) return;

            if (foodResult is { Success: true, Data: not null })
            {
                var entries = foodResult.Data;

                CurrentCalories = entries.Sum(e => e.TotalCalories);
                CurrentCarbs = entries.Sum(e => e.TotalCarbs);
                CurrentProtein = entries.Sum(e => e.TotalProtein);
                CurrentFat = entries.Sum(e => e.TotalFat);

                OnPropertyChanged(nameof(CaloriesProgress));
                OnPropertyChanged(nameof(CarbsProgress));
                OnPropertyChanged(nameof(ProteinProgress));
                OnPropertyChanged(nameof(FatProgress));

                UpdateMealItem(Breakfast, entries);
                UpdateMealItem(Lunch, entries);
                UpdateMealItem(Dinner, entries);
                UpdateMealItem(Snack, entries);
            }
            else
            {
                var errorMsg = new LocalizedString(() => foodResult.Message);
                HandleError(errorMsg);
            }

            if (waterResult is { Success: true, Data: not null })
            {
                var sortedData = waterResult.Data.OrderByDescending(e => e.Date);

                WaterEntries = new ObservableCollection<DrinkEntryResponse>(sortedData);
                CurrentWater = WaterEntries.Sum(e => e.VolumeMl);
                OnPropertyChanged(nameof(WaterProgress));
            }
            else
            {
                var errorMsg = new LocalizedString(() => waterResult.Message);
                HandleError(errorMsg);
            }

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

    [RelayCommand]
    private async Task AddWater(int volumeMl)
    {
        if (volumeMl <= 0) return;

        var timestamp = SelectedDate.Date == DateTime.Today
            ? DateTime.Now
            : SelectedDate.Date.Add(DateTime.Now.TimeOfDay);

        var tempEntry = new DrinkEntryResponse(Guid.Empty, volumeMl, timestamp);

        WaterEntries.Insert(0, tempEntry);
        CurrentWater += volumeMl;
        OnPropertyChanged(nameof(WaterProgress));

        var request = new DrinkEntryCreateRequest(volumeMl, timestamp);
        var result = await _drinkTrackingService.CreateEntryAsync(request);

        if (result is { Success: true, Data: not null })
        {
            var index = WaterEntries.IndexOf(tempEntry);
            if (index != -1) WaterEntries[index] = result.Data;
        }
        else
        {
            WaterEntries.Remove(tempEntry);
            CurrentWater -= volumeMl;
            OnPropertyChanged(nameof(WaterProgress));

            var errorMsg = new LocalizedString(() => result.Message);
            await _alertService.ShowToastAsync(errorMsg.Localized);
        }
    }

    [RelayCommand]
    private async Task DeleteWaterEntry(DrinkEntryResponse entry)
    {
        var index = WaterEntries.IndexOf(entry);
        if (index == -1) return;

        WaterEntries.Remove(entry);
        CurrentWater -= entry.VolumeMl;
        OnPropertyChanged(nameof(WaterProgress));

        var result = await _drinkTrackingService.DeleteEntryAsync(entry.Id);

        if (!result.Success)
        {
            if (index <= WaterEntries.Count)
                WaterEntries.Insert(index, entry);
            else
                WaterEntries.Add(entry);

            CurrentWater += entry.VolumeMl;
            OnPropertyChanged(nameof(WaterProgress));

            var errorMsg = new LocalizedString(() => result.Message);
            await _alertService.ShowToastAsync(errorMsg.Localized);
        }
    }

    [RelayCommand]
    private void OpenWaterInput()
    {
        WaterInputValue = "250";
        IsWaterInputVisible = true;
    }

    [RelayCommand]
    private void CloseWaterInput()
    {
        IsWaterInputVisible = false;
    }

    [RelayCommand(CanExecute = nameof(CanSaveCustomWater))]
    private async Task SaveCustomWater()
    {
        if (int.TryParse(WaterInputValue, out var amount) && amount > 0)
        {
            await AddWater(amount);
            CloseWaterInput();
        }
    }

    private bool CanSaveCustomWater()
    {
        return int.TryParse(WaterInputValue, out var amount) && amount is > 50 and < 2000;
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

    private void UpdateMealItem(MealDashboardItem item, List<FoodEntryDto> entries)
    {
        var mealEntries = entries.Where(e => e.DayTime == item.Type).ToList();

        if (mealEntries.Count != 0)
        {
            item.CurrentCalories = mealEntries.Sum(e => e.TotalCalories);
            item.EntryId = mealEntries.FirstOrDefault()?.Id;
            item.HasEntry = true;
        }
        else
        {
            item.CurrentCalories = 0;
            item.EntryId = null;
            item.HasEntry = false;
        }

        var ratio = item.Type switch
        {
            DayTime.Breakfast => 0.25,
            DayTime.Lunch => 0.35,
            DayTime.Dinner => 0.25,
            _ => 0.15
        };

        item.TargetCalories = TargetCalories * ratio;
    }

    private void ResetDashboard()
    {
        CurrentCalories = 0;
        CurrentCarbs = 0;
        CurrentProtein = 0;
        CurrentFat = 0;

        CurrentWater = 0;
        WaterEntries.Clear();

        OnPropertyChanged(nameof(CaloriesProgress));
        OnPropertyChanged(nameof(CarbsProgress));
        OnPropertyChanged(nameof(ProteinProgress));
        OnPropertyChanged(nameof(FatProgress));
        OnPropertyChanged(nameof(WaterProgress));

        UpdateMealItem(Breakfast, []);
        UpdateMealItem(Lunch, []);
        UpdateMealItem(Dinner, []);
        UpdateMealItem(Snack, []);
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
    private async Task GoToMealDetails(DayTime mealType)
    {
        var item = mealType switch
        {
            DayTime.Breakfast => Breakfast,
            DayTime.Lunch => Lunch,
            DayTime.Dinner => Dinner,
            _ => Snack
        };

        var route = $"{nameof(MealDetailsPageView)}?Date={SelectedDate:yyyy-MM-dd}&MealType={mealType}";

        if (item.EntryId != null) route += $"&EntryId={item.EntryId}";

        await Shell.Current.GoToAsync(route);
    }
}

public partial class MealDashboardItem(DayTime type) : ObservableObject
{
    public DayTime Type { get; } = type;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(Progress))]
    private double _currentCalories;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(Progress))]
    private double _targetCalories;

    [ObservableProperty] private Guid? _entryId;

    [ObservableProperty] private bool _hasEntry;

    public double Progress => TargetCalories > 0 ? CurrentCalories / TargetCalories : 0;
}
