using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ForgeFit.MAUI.Constants;
using ForgeFit.MAUI.Models.DTOs.Food;
using ForgeFit.MAUI.Models.Enums.FoodEnums;
using ForgeFit.MAUI.Services.Interfaces;
using ForgeFit.MAUI.ViewModels.Core;
using LocalizationResourceManager.Maui;

namespace ForgeFit.MAUI.ViewModels.Diary.FoodSearch;

public partial class FoodSearchPageViewModel : BaseViewModel, IQueryAttributable
{
    private readonly IAlertService _alertService;
    private readonly IDiaryService _diaryService;
    private readonly IFoodService _foodService;
    private readonly ILocalizationResourceManager _localizationManager;

    private DateTime _date;

    [ObservableProperty] private string _mealTitle = string.Empty;
    private DayTime _mealType;

    public FoodSearchPageViewModel(
        IFoodService foodService,
        IDiaryService diaryService,
        IAlertService alertService,
        ILocalizationResourceManager localizationManager)
    {
        _foodService = foodService;
        _diaryService = diaryService;
        _alertService = alertService;
        _localizationManager = localizationManager;

        SearchVM = new FoodSearchViewModel();
        DetailsVM = new FoodDetailsViewModel(alertService);
        ScannerVM = new FoodScannerViewModel();
        DiaryVM = new FoodDiaryIntegrationViewModel(diaryService, foodService, alertService);

        SetupCallbacks();
    }

    public FoodSearchViewModel SearchVM { get; }
    public FoodDetailsViewModel DetailsVM { get; }
    public FoodScannerViewModel ScannerVM { get; }
    public FoodDiaryIntegrationViewModel DiaryVM { get; }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        ResetState();
        IsLoading = true;

        if (query.TryGetValue("Date", out var dateObj) && DateTime.TryParse(dateObj.ToString(), out var date))
            _date = date;

        if (query.TryGetValue("MealType", out var typeObj) && Enum.TryParse<DayTime>(typeObj.ToString(), out var type))
        {
            _mealType = type;
            MealTitle = type switch
            {
                DayTime.Breakfast => _localizationManager["Meal_Breakfast"],
                DayTime.Lunch => _localizationManager["Meal_Lunch"],
                DayTime.Dinner => _localizationManager["Meal_Dinner"],
                _ => _localizationManager["Meal_Snack"]
            };
        }

        Guid? entryId = null;
        if (query.TryGetValue("EntryId", out var idObj) && Guid.TryParse(idObj.ToString(), out var id))
            entryId = id;

        DiaryVM.Initialize(_date, _mealType, entryId);
        await DiaryVM.RefreshExistingIdsAsync();
        await SearchVM.LoadRecentAsync();
    }

    private void SetupCallbacks()
    {
        SearchVM.PerformSearchCallback = PerformSearchAsync;
        SearchVM.LoadRecentCallback = LoadRecentAsync;
        SearchVM.LoadMoreCallback = LoadMoreAsync;
        SearchVM.ToggleItemCallback = ToggleItemAsync;
        SearchVM.OpenFoodDetailsCallback = OpenFoodDetailsAsync;

        DetailsVM.OpenFoodDetailsCallback = (product, source) =>
            DetailsVM.OpenFoodDetailsInternal(product, source, SearchVM.IsShowingRecent);
        DetailsVM.CloseFoodDetailsCallback = CloseFoodDetailsInternal;
        DetailsVM.SaveFoodCallback = SaveFoodInternal;

        ScannerVM.BarcodeDetectedCallback = BarcodeDetectedAsync;
    }

    private void ResetState()
    {
        SearchVM.ResetState();
        DetailsVM.ResetPopupState();
        ScannerVM.ResetState();
        DiaryVM.ResetState();
        IsLoading = false;
    }

    private async Task PerformSearchAsync(string query, CancellationToken token)
    {
        var result = await _foodService.SearchFoodAsync(query, SearchVM.GetCurrentPage());

        if (token.IsCancellationRequested) return;

        if (result is not { Success: true, Data: not null })
        {
            if (result is { Success: false })
            {
                var errorMsg = new LocalizedString(() => result.Message);
                await _alertService.ShowToastAsync(errorMsg.Localized);
            }

            return;
        }

        if (result.Data.Count < AppConstants.SearchConfig.DefaultPageSize) SearchVM.SetCanLoadMore(false);

        foreach (var item in result.Data)
        {
            var vm = new FoodSearchItemViewModel(item);
            if (DiaryVM.IsProductAdded(item.ExternalId)) vm.IsAdded = true;
            SearchVM.AddSearchResult(vm);
        }
    }

    private async Task LoadRecentAsync(CancellationToken token = default)
    {
        var from = DateTime.Now.AddDays(-AppConstants.FoodDefaults.RecentItemsLookupDays);
        var to = DateTime.Now;

        var result = await _diaryService.GetEntriesByDateRangeAsync(from, to, token);

        if (token.IsCancellationRequested) return;
        if (result is not { Success: true, Data: not null }) return;

        var recentItems = result.Data
            .SelectMany(e => e.FoodItems)
            .Reverse()
            .GroupBy(x => x.ExternalId)
            .Select(g => g.First())
            .Take(AppConstants.SearchConfig.DefaultPageSize)
            .ToList();

        foreach (var item in recentItems)
        {
            var servingString = $"{item.Amount} {item.ServingUnit}";

            var dto = new FoodSearchResponse(
                item.ExternalId,
                item.Label,
                null,
                item.Calories,
                item.Carbs,
                item.Protein,
                item.Fat,
                item.Fiber,
                item.Sugar,
                item.SaturatedFat,
                item.Sodium,
                servingString
            );

            var vm = new FoodSearchItemViewModel(dto);
            if (DiaryVM.IsProductAdded(item.ExternalId)) vm.IsAdded = true;
            SearchVM.AddSearchResult(vm);
        }
    }

    private async Task LoadMoreAsync()
    {
        var result = await _foodService.SearchFoodAsync(SearchVM.SearchText, SearchVM.GetCurrentPage());

        if (result is not { Success: true, Data: not null })
        {
            SearchVM.DecrementPage();
            return;
        }

        if (result.Data.Count < AppConstants.SearchConfig.DefaultPageSize) SearchVM.SetCanLoadMore(false);

        foreach (var item in result.Data)
        {
            var vm = new FoodSearchItemViewModel(item);
            if (DiaryVM.IsProductAdded(item.ExternalId)) vm.IsAdded = true;
            SearchVM.AddSearchResult(vm);
        }
    }

    private async Task ToggleItemAsync(FoodSearchItemViewModel itemVm)
    {
        if (itemVm.IsAdded)
            await DiaryVM.RemoveItemInternal(itemVm);
        else
            await DiaryVM.QuickAddInternal(itemVm, SearchVM.IsShowingRecent);
    }

    private async Task OpenFoodDetailsAsync(FoodSearchItemViewModel itemVm)
    {
        var result = await _foodService.GetProductByIdAsync(itemVm.Data.ExternalId);

        if (result is { Success: true, Data: not null })
            await DetailsVM.OpenFoodDetailsInternal(result.Data, itemVm.Data, SearchVM.IsShowingRecent);
        else
            await _alertService.ShowToastAsync(new LocalizedString(() => result.Message).Localized);
    }

    private Task CloseFoodDetailsInternal()
    {
        DetailsVM.IsFoodDetailsVisible = false;
        return Task.CompletedTask;
    }

    private async Task SaveFoodInternal(FoodItemDto newItem)
    {
        await DiaryVM.AddEntryToDiaryInternal(newItem);
    }

    private async Task BarcodeDetectedAsync(string barcode)
    {
        IsLoading = true;

        try
        {
            var result = await _foodService.GetProductByBarcodeAsync(barcode);

            if (result is { Success: true, Data: not null })
            {
                var p = result.Data;
                var baseServing = p.Servings.FirstOrDefault();

                var itemVm = new FoodSearchItemViewModel(new FoodSearchResponse(
                    p.ExternalId, p.Label, p.BrandName,
                    baseServing?.Calories ?? 0, baseServing?.Carbs ?? 0,
                    baseServing?.Protein ?? 0, baseServing?.Fat ?? 0,
                    baseServing?.Fiber ?? 0, baseServing?.Sugar ?? 0,
                    baseServing?.SaturatedFat ?? 0, baseServing?.Sodium ?? 0,
                    $"{baseServing?.MetricAmount} {baseServing?.MetricUnit}")
                );

                if (DiaryVM.IsProductAdded(p.ExternalId)) itemVm.IsAdded = true;

                ScannerVM.IsScannerVisible = false;
                SearchVM.ClearSearchResults();
                SearchVM.AddSearchResult(itemVm);
                await DetailsVM.OpenFoodDetailsInternal(p, itemVm.Data, false);
                return;
            }

            ScannerVM.IsScannerVisible = false;
            await _alertService.ShowToastAsync(new LocalizedString(() => result.Message).Localized);
        }
        catch
        {
            ScannerVM.IsScannerVisible = false;
            await _alertService.ShowToastAsync(_localizationManager["UnexpectedErrorMessage"]);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task Back()
    {
        if (DetailsVM.IsFoodDetailsVisible)
        {
            DetailsVM.IsFoodDetailsVisible = false;
            return;
        }

        if (ScannerVM.IsScannerVisible)
        {
            ScannerVM.IsScannerVisible = false;
            return;
        }

        if (DiaryVM.EntryId.HasValue)
            await Shell.Current.GoToAsync($"..?EntryId={Uri.EscapeDataString(DiaryVM.EntryId.Value.ToString())}",
                false);
        else
            await Shell.Current.GoToAsync("..", false);
    }
}

public partial class FoodSearchItemViewModel(FoodSearchResponse data) : ObservableObject
{
    [ObservableProperty] private bool _isAdded;

    [ObservableProperty] private bool _isAdding;
    public FoodSearchResponse Data { get; } = data;

    public string Label => Data.Label;
    public string BrandName => Data.BrandName ?? "";
    public string ServingUnit => Data.Serving;
    public double Calories => Data.Calories;
    public double Carbs => Data.Carbs;
    public double Protein => Data.Protein;
    public double Fat => Data.Fat;
    public double Fiber => Data.Fiber;
    public double Sugar => Data.Sugar;
    public double SaturatedFat => Data.SaturatedFat;
    public double Sodium => Data.Sodium;
}