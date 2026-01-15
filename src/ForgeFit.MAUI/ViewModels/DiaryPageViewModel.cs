using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ForgeFit.MAUI.Messages;
using ForgeFit.MAUI.Models.DTOs.DrinkTracking;
using ForgeFit.MAUI.Models.DTOs.Food;
using ForgeFit.MAUI.Models.DTOs.User;
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
    private readonly IGoalService _goalService;
    private readonly IUserService _userService;

    private readonly IAlertService _alertService;
    private readonly ILocalizationResourceManager _localizationManager;

    private bool _isInitialized;

    [ObservableProperty] private bool _isRefreshing;

    private CancellationTokenSource? _cts;
    private CancellationTokenSource? _weightCts;

    private UserProfileDto? _userProfile;

    [ObservableProperty] private DateTime _selectedDate = DateTime.Today;
    [ObservableProperty] private string _dateTitle = string.Empty;

    [ObservableProperty] private double _currentCalories;
    [ObservableProperty] private double _currentCarbs;
    [ObservableProperty] private double _currentProtein;
    [ObservableProperty] private double _currentFat;
    [ObservableProperty] private double _currentWater;

    [NotifyPropertyChangedFor(nameof(WeightProgress))]
    [NotifyPropertyChangedFor(nameof(WeightLeft))]
    [NotifyPropertyChangedFor(nameof(CurrentWeightInput))]
    [ObservableProperty]
    private double _currentWeight;

    [ObservableProperty] private ObservableCollection<DrinkEntryResponse> _waterEntries = [];
    [ObservableProperty] private bool _isWaterInputVisible;

    [NotifyCanExecuteChangedFor(nameof(SaveCustomWaterCommand))] [ObservableProperty]
    private string _waterInputValue = "";

    [ObservableProperty] private string _currentWeightInput = string.Empty;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(CaloriesProgress))]
    private double _targetCalories;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(CarbsProgress))]
    private double _targetCarbs;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(ProteinProgress))]
    private double _targetProtein;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(FatProgress))]
    private double _targetFat;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(WaterProgress))]
    private double _targetWater;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(WeightProgress))]
    [NotifyPropertyChangedFor(nameof(WeightLeft))]
    private double _targetWeight;

    [ObservableProperty] private string? _targetCaloriesDisplay;
    [ObservableProperty] private string? _targetCarbsDisplay;
    [ObservableProperty] private string? _targetProteinDisplay;
    [ObservableProperty] private string? _targetFatDisplay;
    [ObservableProperty] private string? _targetWaterDisplay;
    [ObservableProperty] private string? _targetWeightDisplay;

    public MealDashboardItem Breakfast { get; } = new(DayTime.Breakfast);
    public MealDashboardItem Lunch { get; } = new(DayTime.Lunch);
    public MealDashboardItem Dinner { get; } = new(DayTime.Dinner);
    public MealDashboardItem Snack { get; } = new(DayTime.Snack);

    public double CaloriesProgress => TargetCalories > 0 ? CurrentCalories / TargetCalories : 0;
    public double CarbsProgress => TargetCarbs > 0 ? CurrentCarbs / TargetCarbs : 0;
    public double ProteinProgress => TargetProtein > 0 ? CurrentProtein / TargetProtein : 0;
    public double FatProgress => TargetFat > 0 ? CurrentFat / TargetFat : 0;
    public double WaterProgress => TargetWater > 0 ? CurrentWater / TargetWater : 0;
    public double WeightProgress => TargetWeight > 0 ? CurrentWeight / TargetWeight : 0;
    public double WeightLeft => Math.Abs(TargetWeight - CurrentWeight);

    public DiaryPageViewModel(
        IDiaryService diaryService,
        IDrinkTrackingService drinkTrackingService,
        IGoalService goalService,
        IUserService userService,
        IAlertService alertService,
        ILocalizationResourceManager localizationManager)
    {
        _diaryService = diaryService;
        _drinkTrackingService = drinkTrackingService;
        _goalService = goalService;
        _userService = userService;
        _alertService = alertService;
        _localizationManager = localizationManager;

        if (_localizationManager is INotifyPropertyChanged notifyService)
            notifyService.PropertyChanged += (_, _) => UpdateDateTitle();

        SetLoadingState();
        UpdateDateTitle();

        WeakReferenceMessenger.Default.Register<DiaryPageViewModel, DiaryUpdatedMessage>(
            this,
            (r, _) => { r.RefreshCommand.Execute(null); }
        );
    }

    private void SetLoadingState()
    {
        TargetCaloriesDisplay = TargetCarbsDisplay = TargetProteinDisplay =
            TargetFatDisplay = TargetWaterDisplay = TargetWeightDisplay = "-";
    }

    partial void OnSelectedDateChanged(DateTime value)
    {
        ResetDashboard();
        SetLoadingState();
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
            var nutritionGoalTask = _goalService.GetNutritionGoal(token);
            var foodTask = _diaryService.GetEntriesByDateAsync(SelectedDate, token);
            var waterTask = _drinkTrackingService.GetEntriesByDateAsync(SelectedDate, token);
            var bodyGoalTask = _goalService.GetBodyGoal(token);
            var profileTask = _userService.GetProfileAsync(token);

            await Task.WhenAll(foodTask, waterTask, nutritionGoalTask, bodyGoalTask, profileTask);
            if (token.IsCancellationRequested) return;

            string? firstErrorMessage = null;

            if (!nutritionGoalTask.Result.Success) firstErrorMessage = nutritionGoalTask.Result.Message;
            else if (!foodTask.Result.Success) firstErrorMessage = foodTask.Result.Message;
            else if (!bodyGoalTask.Result.Success) firstErrorMessage = bodyGoalTask.Result.Message;
            else if (!waterTask.Result.Success) firstErrorMessage = waterTask.Result.Message;
            else if (!profileTask.Result.Success) firstErrorMessage = profileTask.Result.Message;

            if (firstErrorMessage != null)
            {
                HandleError(new LocalizedString(() => firstErrorMessage));
                return;
            }

            if (nutritionGoalTask.Result is { Success: true, Data: not null })
            {
                var g = nutritionGoalTask.Result.Data;

                TargetCalories = g.Calories;
                TargetCarbs = g.Carbs;
                TargetProtein = g.Protein;
                TargetFat = g.Fat;
                TargetWater = g.WaterGoalMl;

                TargetCaloriesDisplay = TargetCalories.ToString("F0");
                TargetCarbsDisplay = TargetCarbs.ToString("F0");
                TargetProteinDisplay = TargetProtein.ToString("F0");
                TargetFatDisplay = TargetFat.ToString("F0");
                TargetWaterDisplay = TargetWater.ToString("F0");
            }

            if (foodTask.Result is { Success: true, Data: not null })
            {
                var entries = foodTask.Result.Data;

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

            if (waterTask.Result is { Success: true, Data: not null })
            {
                var sortedData = waterTask.Result.Data.OrderByDescending(e => e.Date);

                WaterEntries = new ObservableCollection<DrinkEntryResponse>(sortedData);
                CurrentWater = WaterEntries.Sum(e => e.VolumeMl);
                OnPropertyChanged(nameof(WaterProgress));
            }

            if (bodyGoalTask.Result is { Success: true, Data: not null })
            {
                TargetWeight = bodyGoalTask.Result.Data.WeightGoal;
                TargetWeightDisplay = $"{TargetWeight:F1}";

                if (profileTask.Result is { Success: true, Data: not null })
                {
                    _userProfile = profileTask.Result.Data;
                    CurrentWeight = _userProfile.Weight;
                }
                else
                {
                    CurrentWeight = 80.0;
                }

                if (string.IsNullOrWhiteSpace(CurrentWeightInput))
                    CurrentWeightInput = CurrentWeight.ToString("F1");
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
            HandleError(errorMsg);
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
    private void IncreaseWeight()
    {
        CurrentWeightInput = double.TryParse(CurrentWeightInput, out var weight)
            ? (weight + 0.1).ToString("F1")
            : TargetWeight.ToString("F1");
    }

    [RelayCommand]
    private void DecreaseWeight()
    {
        CurrentWeightInput = double.TryParse(CurrentWeightInput, out var weight)
            ? Math.Max(0, weight - 0.1).ToString("F1")
            : TargetWeight.ToString("F1");
    }

    private async Task SaveWeight(double weight)
    {
        _weightCts?.Cancel();
        _weightCts = new CancellationTokenSource();
        var token = _weightCts.Token;

        try
        {
            await Task.Delay(800, token);
            if (token.IsCancellationRequested) return;

            if (_userProfile == null)
            {
                var loadResult = await _userService.GetProfileAsync(token);
                if (!loadResult.Success)
                    return;

                _userProfile = loadResult.Data;
            }

            var updatedProfile = _userProfile with { Weight = weight };
            var result = await _userService.UpdateProfileAsync(updatedProfile, token);

            if (!result.Success || result.Data == null)
            {
                var errorMsg = new LocalizedString(() => result.Message);
                await _alertService.ShowToastAsync(errorMsg.Localized);
                return;
            }

            _userProfile = updatedProfile;
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception)
        {
            if (token.IsCancellationRequested) return;
            var errorMsg = new LocalizedString(() => _localizationManager["UnexpectedErrorMessage"]);
            await _alertService.ShowToastAsync(errorMsg.Localized);
        }
    }

    partial void OnCurrentWeightInputChanged(string value)
    {
        if (!double.TryParse(value, out var weight)) return;

        CurrentWeight = weight;
        _ = SaveWeight(weight);
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
        SetLoadingState();

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
