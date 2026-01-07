using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ForgeFit.MAUI.Messages;
using ForgeFit.MAUI.Models;
using ForgeFit.MAUI.Models.DTOs.Food;
using ForgeFit.MAUI.Models.Enums.FoodEnums;
using ForgeFit.MAUI.Services.Interfaces;
using ForgeFit.MAUI.Views.Diary;
using LocalizationResourceManager.Maui;

namespace ForgeFit.MAUI.ViewModels;

public partial class MealDetailsPageViewModel : BaseViewModel, IQueryAttributable
{
    private readonly IDiaryService _diaryService;
    private readonly IFoodService _foodService;
    private readonly IGoalService _goalService;
    
    private readonly IAlertService _alertService;
    private readonly ILocalizationResourceManager _localizationManager;

    private CancellationTokenSource? _cts;

    [ObservableProperty] private string _dateStr = string.Empty;
    [ObservableProperty] private string _mealTypeStr = string.Empty;
    [ObservableProperty] private string? _entryIdStr;

    [ObservableProperty] private string _mealTitle = string.Empty;
    [ObservableProperty] private string _mealEmoji = string.Empty;

    private DateTime _date;
    private DayTime _mealType;
    private Guid? _entryId;

    private FoodEntryDto? _currentEntry;

    public ObservableCollection<FoodItemDto> FoodItems { get; } = [];

    [ObservableProperty] private double _totalCalories;
    [ObservableProperty] private double _totalCarbs;
    [ObservableProperty] private double _totalProtein;
    [ObservableProperty] private double _totalFat;

    [ObservableProperty] private double _caloriesProgress;
    [ObservableProperty] private double _carbsProgress;
    [ObservableProperty] private double _proteinProgress;
    [ObservableProperty] private double _fatProgress;

    private double _dailyTargetCalories;
    private double _dailyTargetCarbs;
    private double _dailyTargetProtein;
    private double _dailyTargetFat;

    [ObservableProperty] private double _mealTargetCalories;
    [ObservableProperty] private double _mealTargetCarbs;
    [ObservableProperty] private double _mealTargetProtein;
    [ObservableProperty] private double _mealTargetFat;
    
    [ObservableProperty] private string _mealTargetCaloriesDisplay;
    [ObservableProperty] private string _mealTargetCarbsDisplay;
    [ObservableProperty] private string _mealTargetProteinDisplay;
    [ObservableProperty] private string _mealTargetFatDisplay;

    [ObservableProperty] private bool _isFoodDetailsVisible;
    [ObservableProperty] private FoodProductResponse? _selectedFoodDetail;
    [ObservableProperty] private FoodServingDto? _selectedServing;

    [NotifyCanExecuteChangedFor(nameof(SaveFoodCommand))] [ObservableProperty]
    private string? _inputAmount;

    private FoodItemDto? _editingItem;

    public MealDetailsPageViewModel(
        IDiaryService diaryService,
        IFoodService foodService,
        IGoalService goalService,
        IAlertService alertService,
        ILocalizationResourceManager localizationManager)
    {
        _diaryService = diaryService;
        _foodService = foodService;
        _goalService = goalService;
        _alertService = alertService;
        _localizationManager = localizationManager;

        WeakReferenceMessenger.Default.Register<MealDetailsPageViewModel, DiaryUpdatedMessage>(
            this,
            (r, m) => { _ = r.LoadDataAsync(); }
        );
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        ResetState();

        if (query.TryGetValue("Date", out var dateObj) && DateTime.TryParse(dateObj.ToString(), out var date))
        {
            _date = date;
            DateStr = date.ToString("yyyy-MM-dd");
        }

        if (query.TryGetValue("MealType", out var typeObj) && Enum.TryParse<DayTime>(typeObj.ToString(), out var type))
        {
            _mealType = type;
            MealTypeStr = type.ToString();
            SetupMealInfo(type);
        }

        if (query.TryGetValue("EntryId", out var idObj) && Guid.TryParse(idObj.ToString(), out var id))
        {
            _entryId = id;
            EntryIdStr = id.ToString();
        }

        LoadDataCommand.Execute(null);
    }

    private void ResetState()
    {
        _cts?.Cancel();

        FoodItems.Clear();
        _currentEntry = null;
        _entryId = null;
        _editingItem = null;

        IsFoodDetailsVisible = false;
        SelectedFoodDetail = null;
        SelectedServing = null;
        InputAmount = null;

        TotalCalories = 0;
        TotalCarbs = 0;
        TotalProtein = 0;
        TotalFat = 0;

        CaloriesProgress = 0;
        CarbsProgress = 0;
        ProteinProgress = 0;
        FatProgress = 0;
        
        MealTargetCaloriesDisplay = "-";
        MealTargetCarbsDisplay = "-";
        MealTargetProteinDisplay = "-";
        MealTargetFatDisplay = "-";

        IsLoading = false;
    }

    private void SetupMealInfo(DayTime type)
    {
        MealEmoji = type switch
        {
            DayTime.Breakfast => "🍳",
            DayTime.Lunch => "🍲",
            DayTime.Dinner => "🍝",
            _ => "🍌"
        };

        MealTitle = type switch
        {
            DayTime.Breakfast => _localizationManager["Meal_Breakfast"],
            DayTime.Lunch => _localizationManager["Meal_Lunch"],
            DayTime.Dinner => _localizationManager["Meal_Dinner"],
            _ => _localizationManager["Meal_Snack"]
        };
    }

    [RelayCommand]
    private async Task LoadDataAsync()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = new CancellationTokenSource();
        var token = _cts.Token;

        IsLoading = true;

        try
        {
            var goalTask = _goalService.GetNutritionGoal(token);
            var entryTask = _entryId.HasValue 
                ? _diaryService.GetEntryAsync(_entryId.Value, token) 
                : Task.FromResult(new ServiceResponse<FoodEntryDto?> { Success = true, Data = null });
            
            await Task.WhenAll(goalTask, entryTask);
            if (token.IsCancellationRequested) return;
            
            if (goalTask.Result is { Success: true, Data: not null })
            {
                var g = goalTask.Result.Data;
                _dailyTargetCalories = g.Calories;
                _dailyTargetCarbs = g.Carbs;
                _dailyTargetProtein = g.Protein;
                _dailyTargetFat = g.Fat;
            }

            var entryResponse = entryTask.Result;
            if (_entryId.HasValue)
            {
                if (!entryResponse.Success || entryResponse.Data == null)
                {
                    ClearEntryState(); 
                    return; 
                }

                _currentEntry = entryResponse.Data;
                FoodItems.Clear();
                foreach (var item in _currentEntry.FoodItems)
                    FoodItems.Add(item);
            }

            RecalculateMacros();
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception)
        {
            if (!token.IsCancellationRequested)
                await _alertService.ShowToastAsync(_localizationManager["UnexpectedErrorMessage"]);
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void ClearEntryState()
    {
        _entryId = null;
        _currentEntry = null;
        FoodItems.Clear();
        RecalculateMacros();
    }


    [RelayCommand]
    private async Task DeleteItem(FoodItemDto item)
    {
        if (_currentEntry == null || _entryId == null) return;

        var deletedIndex = FoodItems.IndexOf(item);
        FoodItems.Remove(item);
        RecalculateMacros();

        try
        {
            if (FoodItems.Count == 0)
            {
                var result = await _diaryService.DeleteEntryAsync(_entryId.Value);
                if (result.Success)
                {
                    _entryId = null;
                    _currentEntry = null;
                    RecalculateMacros();
                }
                else
                {
                    FoodItems.Add(item);
                    RecalculateMacros();
                    await _alertService.ShowToastAsync(result.Message);
                }
            }
            else
            {
                var request = new FoodEntryCreateRequest(_mealType, _date, FoodItems.ToList());
                var result = await _diaryService.UpdateEntryAsync(_entryId.Value, request);

                if (!result.Success)
                {
                    if (deletedIndex >= 0) FoodItems.Insert(deletedIndex, item);
                    else FoodItems.Add(item);

                    RecalculateMacros();
                    var errorMsg = new LocalizedString(() => result.Message);
                    await _alertService.ShowToastAsync(errorMsg.Localized);
                }
            }
        }
        catch
        {
            if (deletedIndex >= 0 && !FoodItems.Contains(item)) FoodItems.Insert(deletedIndex, item);
            RecalculateMacros();
            await _alertService.ShowToastAsync(_localizationManager["UnexpectedErrorMessage"]);
        }
    }

    [RelayCommand]
    private async Task OpenEditPopup(FoodItemDto item)
    {
        if (IsLoading) return;

        try
        {
            IsLoading = true;

            var result = await _foodService.GetProductByIdAsync(item.ExternalId);

            if (result is { Success: true, Data: not null })
            {
                var details = result.Data;
                _editingItem = item;
                SelectedFoodDetail = details;

                SelectedServing = details.Servings.FirstOrDefault(s => s.MetricUnit == item.ServingUnit)
                                  ?? details.Servings.FirstOrDefault();

                InputAmount = item.Amount.ToString(CultureInfo.InvariantCulture);
                IsFoodDetailsVisible = true;
            }
            else
            {
                var errorMsg = new LocalizedString(() => result.Message);
                await _alertService.ShowToastAsync(errorMsg.Localized);
            }
        }
        catch
        {
            await _alertService.ShowToastAsync(_localizationManager["UnexpectedErrorMessage"]);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void CloseFoodDetails()
    {
        IsFoodDetailsVisible = false;
        _editingItem = null;
        InputAmount = null;
    }

    [RelayCommand(CanExecute = nameof(CanSaveFood))]
    private async Task SaveFood()
    {
        if (_editingItem == null || SelectedFoodDetail == null || SelectedServing == null ||
            string.IsNullOrEmpty(InputAmount)) return;
        if (_entryId == null) return;

        IsFoodDetailsVisible = false;

        var amount = double.Parse(InputAmount, CultureInfo.InvariantCulture);
        var ratio = amount / SelectedServing.MetricAmount;

        var updatedItem = _editingItem with
        {
            Amount = amount,
            ServingUnit = SelectedServing.MetricUnit,
            Calories = SelectedServing.Calories * ratio,
            Carbs = SelectedServing.Carbs * ratio,
            Protein = SelectedServing.Protein * ratio,
            Fat = SelectedServing.Fat * ratio
        };

        var index = FoodItems.IndexOf(_editingItem);
        if (index >= 0) FoodItems[index] = updatedItem;

        RecalculateMacros();
        _editingItem = null;

        try
        {
            var request = new FoodEntryCreateRequest(_mealType, _date, FoodItems.ToList());
            var response = await _diaryService.UpdateEntryAsync(_entryId.Value, request);

            if (!response.Success || response.Data is null)
            {
                var errorMsg = new LocalizedString(() => response.Message);
                await _alertService.ShowToastAsync(errorMsg.Localized);
            }
        }
        catch
        {
            await _alertService.ShowToastAsync(_localizationManager["UnexpectedErrorMessage"]);
        }
    }

    private bool CanSaveFood()
    {
        return double.TryParse(InputAmount, out var amount) && amount is > 0 and <= 5000;
    }

    [RelayCommand]
    private async Task GoToFoodSearch()
    {
        await Shell.Current.GoToAsync(
            $"{nameof(FoodSearchPageView)}?Date={_date:yyyy-MM-dd}&MealType={_mealType}&EntryId={_entryId}");
    }

    public double CurrentCalories => CalculateNutrient(s => s.Calories);
    public double CurrentCarbs => CalculateNutrient(s => s.Carbs);
    public double CurrentProtein => CalculateNutrient(s => s.Protein);
    public double CurrentFat => CalculateNutrient(s => s.Fat);

    private double CalculateNutrient(Func<FoodServingDto, double> selector)
    {
        var amount = double.TryParse(InputAmount, out var a) ? a : 0;

        if (SelectedServing == null || SelectedServing.MetricAmount == 0 || amount <= 0)
            return 0;

        return amount * selector(SelectedServing) / SelectedServing.MetricAmount;
    }

    partial void OnInputAmountChanged(string? value)
    {
        NotifyPopupUpdates();
    }

    partial void OnSelectedServingChanged(FoodServingDto? value)
    {
        NotifyPopupUpdates();
    }

    private void NotifyPopupUpdates()
    {
        OnPropertyChanged(nameof(CurrentCalories));
        OnPropertyChanged(nameof(CurrentCarbs));
        OnPropertyChanged(nameof(CurrentProtein));
        OnPropertyChanged(nameof(CurrentFat));
    }

    private void RecalculateTargetsOnly()
    {
        var ratio = _mealType switch
        {
            DayTime.Breakfast => 0.25,
            DayTime.Lunch => 0.35,
            DayTime.Dinner => 0.25,
            _ => 0.15
        };

        MealTargetCalories = _dailyTargetCalories * ratio;
        MealTargetCarbs = _dailyTargetCarbs * ratio;
        MealTargetProtein = _dailyTargetProtein * ratio;
        MealTargetFat = _dailyTargetFat * ratio;
        
        MealTargetCaloriesDisplay = MealTargetCalories.ToString("F0");
        MealTargetCarbsDisplay = MealTargetCarbs.ToString("F0");
        MealTargetProteinDisplay = MealTargetProtein.ToString("F0");
        MealTargetFatDisplay = MealTargetFat.ToString("F0");
    }

    private void RecalculateMacros()
    {
        TotalCalories = FoodItems.Sum(x => x.Calories);
        TotalCarbs = FoodItems.Sum(x => x.Carbs);
        TotalProtein = FoodItems.Sum(x => x.Protein);
        TotalFat = FoodItems.Sum(x => x.Fat);

        RecalculateTargetsOnly();

        CaloriesProgress = MealTargetCalories > 0 ? TotalCalories / MealTargetCalories : 0;
        CarbsProgress = MealTargetCarbs > 0 ? TotalCarbs / MealTargetCarbs : 0;
        ProteinProgress = MealTargetProtein > 0 ? TotalProtein / MealTargetProtein : 0;
        FatProgress = MealTargetFat > 0 ? TotalFat / MealTargetFat : 0;
    }
}
