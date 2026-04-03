using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ForgeFit.MAUI.Messages;
using ForgeFit.MAUI.Models;
using ForgeFit.MAUI.Models.DTOs.Food;
using ForgeFit.MAUI.Models.Enums.FoodEnums;
using ForgeFit.MAUI.Services.Interfaces;
using ForgeFit.MAUI.ViewModels.Core;
using ForgeFit.MAUI.ViewModels.Diary.FoodSearch;
using ForgeFit.MAUI.Views.Diary;
using LocalizationResourceManager.Maui;

namespace ForgeFit.MAUI.ViewModels.Diary.Meals;

public partial class MealDetailsPageViewModel : BaseViewModel, IQueryAttributable
{
    private readonly IAlertService _alertService;
    private readonly IDiaryService _diaryService;
    private readonly IFoodService _foodService;
    private readonly IGoalService _goalService;
    private readonly ILocalizationResourceManager _localizationManager;

    private CancellationTokenSource? _cts;

    private FoodEntryDto? _currentEntry;

    private DateTime _date;
    private FoodItemDto? _editingItem;
    private Guid? _entryId;
    [ObservableProperty] private string _mealEmoji = string.Empty;

    [ObservableProperty] private string _mealTitle = string.Empty;
    private DayTime _mealType;

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

        DetailsVM = new FoodDetailsViewModel(alertService);
        MacrosVM = new MealMacroStatsViewModel();

        SetupFoodDetailsCallbacks();

        WeakReferenceMessenger.Default.Register<MealDetailsPageViewModel, DiaryUpdatedMessage>(
            this,
            (r, m) => { _ = r.LoadDataAsync(); }
        );
    }

    public ObservableCollection<FoodItemDto> FoodItems { get; } = [];

    public FoodDetailsViewModel DetailsVM { get; }
    public MealMacroStatsViewModel MacrosVM { get; }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        ResetState();

        if (query.TryGetValue("Date", out var dateObj) &&
            DateTime.TryParse(dateObj.ToString(), out var date)) _date = date;

        if (query.TryGetValue("MealType", out var typeObj) && Enum.TryParse<DayTime>(typeObj.ToString(), out var type))
        {
            _mealType = type;
            SetupMealInfo(type);
        }

        if (query.TryGetValue("EntryId", out var idObj) && Guid.TryParse(idObj.ToString(), out var id)) _entryId = id;

        LoadDataCommand.Execute(null);
    }

    private void SetupFoodDetailsCallbacks()
    {
        DetailsVM.SaveFoodCallback = async newItem =>
        {
            if (_editingItem == null || _entryId == null) return;

            var originalItem = _editingItem;
            var index = FoodItems.IndexOf(_editingItem);
            if (index >= 0) FoodItems[index] = newItem;
            _editingItem = null;
            MacrosVM.CalculateTotals(FoodItems);

            try
            {
                var request = new FoodEntryCreateRequest(_mealType, _date, FoodItems.ToList());
                var response = await _diaryService.UpdateEntryAsync(_entryId.Value, request);

                if (response is { Success: true, Data: not null })
                    return;

                var errorMsg = new LocalizedString(() => response.Message);
                await _alertService.ShowToastAsync(errorMsg.Localized);

                if (index >= 0 && !FoodItems.Contains(originalItem))
                    FoodItems[index] = originalItem;

                MacrosVM.CalculateTotals(FoodItems);
            }
            catch
            {
                await _alertService.ShowToastAsync(_localizationManager["UnexpectedErrorMessage"]);

                if (index >= 0 && !FoodItems.Contains(originalItem))
                    FoodItems[index] = originalItem;

                MacrosVM.CalculateTotals(FoodItems);
            }
        };
    }

    private void ResetState()
    {
        _cts?.Cancel();

        FoodItems.Clear();
        _currentEntry = null;
        _entryId = null;
        _editingItem = null;

        MacrosVM.Reset();

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
                MacrosVM.CalculateTargets(goalTask.Result.Data, _mealType);

            var entryResponse = entryTask.Result;
            if (!_entryId.HasValue)
            {
                MacrosVM.CalculateTotals(FoodItems);
                return;
            }

            if (!entryResponse.Success || entryResponse.Data == null)
            {
                ClearEntryState();
                return;
            }

            _currentEntry = entryResponse.Data;
            FoodItems.Clear();
            foreach (var item in _currentEntry.FoodItems)
                FoodItems.Add(item);

            MacrosVM.CalculateTotals(FoodItems);
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
        MacrosVM.CalculateTotals(FoodItems);
    }


    [RelayCommand]
    private async Task DeleteItem(FoodItemDto item)
    {
        if (_currentEntry == null || _entryId == null)
            return;

        var deletedIndex = FoodItems.IndexOf(item);
        FoodItems.Remove(item);
        MacrosVM.CalculateTotals(FoodItems);

        try
        {
            if (FoodItems.Count == 0)
            {
                var result = await _diaryService.DeleteEntryAsync(_entryId.Value);
                if (!result.Success)
                {
                    FoodItems.Add(item);
                    MacrosVM.CalculateTotals(FoodItems);
                    await _alertService.ShowToastAsync(result.Message);
                    return;
                }

                _entryId = null;
                _currentEntry = null;
                MacrosVM.CalculateTotals(FoodItems);
                return;
            }

            var request = new FoodEntryCreateRequest(_mealType, _date, FoodItems.ToList());
            var updateResult = await _diaryService.UpdateEntryAsync(_entryId.Value, request);

            if (updateResult.Success)
                return;

            if (deletedIndex >= 0)
                FoodItems.Insert(deletedIndex, item);
            else
                FoodItems.Add(item);

            MacrosVM.CalculateTotals(FoodItems);
            var errorMsg = new LocalizedString(() => updateResult.Message);
            await _alertService.ShowToastAsync(errorMsg.Localized);
        }
        catch
        {
            if (deletedIndex >= 0 && !FoodItems.Contains(item))
                FoodItems.Insert(deletedIndex, item);

            MacrosVM.CalculateTotals(FoodItems);
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
                _editingItem = item;
                DetailsVM.OpenFoodDetailsInternal(result.Data, item);
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
    private async Task GoToFoodSearch()
    {
        await Shell.Current.GoToAsync(
            $"{nameof(FoodSearchPageView)}?Date={_date:yyyy-MM-dd}&MealType={_mealType}&EntryId={_entryId}");
    }
}