using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ForgeFit.MAUI.Messages;
using ForgeFit.MAUI.Models.DTOs.Food;
using ForgeFit.MAUI.Models.Enums.FoodEnums;
using ForgeFit.MAUI.Services.Interfaces;
using LocalizationResourceManager.Maui;
using ZXing.Net.Maui;

namespace ForgeFit.MAUI.ViewModels;

public partial class FoodSearchPageViewModel(
    IFoodService foodService,
    IDiaryService diaryService,
    IAlertService alertService,
    ILocalizationResourceManager localizationManager)
    : BaseViewModel, IQueryAttributable
{
    private CancellationTokenSource? _searchCts;

    [ObservableProperty] private string _dateStr = string.Empty;
    [ObservableProperty] private string _mealTypeStr = string.Empty;
    [ObservableProperty] private string _entryIdStr = string.Empty;
    [ObservableProperty] private string _mealTitle = string.Empty;
    
    private DateTime _date;
    private DayTime _mealType;
    
    private HashSet<string> _existingProductIds = [];

    [ObservableProperty] private string _searchText = string.Empty;
    [ObservableProperty] private bool _isShowingRecent = true;
    [ObservableProperty] private bool _isScannerVisible;
    [ObservableProperty] private bool _isTorchOn;
    
    [ObservableProperty] private bool _isLoadingMore;
    private int _currentPage = 1;
    private const int PageSize = 20;
    private bool _canLoadMore = true;

    public ObservableCollection<FoodSearchItemViewModel> SearchResults { get; } = [];

    [ObservableProperty] private bool _isFoodDetailsVisible;
    [ObservableProperty] private FoodProductResponse? _selectedFoodDetail;
    [ObservableProperty] private FoodServingDto? _selectedServing;
    [NotifyCanExecuteChangedFor(nameof(SaveFoodCommand))] 
    [ObservableProperty] private string? _inputAmount;
    
    public double CurrentCalories => CalculateNutrient(s => s.Calories);
    public double CurrentCarbs => CalculateNutrient(s => s.Carbs);
    public double CurrentProtein => CalculateNutrient(s => s.Protein);
    public double CurrentFat => CalculateNutrient(s => s.Fat);
    
    private Guid? _entryId;

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        ResetState();
        
        IsLoading = true;

        if (query.TryGetValue("Date", out var dateObj) && DateTime.TryParse(dateObj.ToString(), out var date))
        {
            _date = date;
            DateStr = date.ToString("yyyy-MM-dd");
        }

        if (query.TryGetValue("MealType", out var typeObj) && Enum.TryParse<DayTime>(typeObj.ToString(), out var type))
        {
            _mealType = type;
            MealTypeStr = type.ToString();
            
            MealTitle = type switch
            {
                DayTime.Breakfast => localizationManager["Meal_Breakfast"],
                DayTime.Lunch => localizationManager["Meal_Lunch"],
                DayTime.Dinner => localizationManager["Meal_Dinner"],
                _ => localizationManager["Meal_Snack"]
            };
        }
        
        if (query.TryGetValue("EntryId", out var idObj) && Guid.TryParse(idObj.ToString(), out var id))
        {
            _entryId = id;
            EntryIdStr = id.ToString();
        }
        
        await RefreshExistingIdsAsync();
        LoadRecent(); 
    }

    private void ResetState()
    {
        SearchText = string.Empty;
        IsShowingRecent = true;
        IsScannerVisible = false;
        IsTorchOn = false;
        SearchResults.Clear();
        _existingProductIds.Clear();
        _entryId = null;
        EntryIdStr = string.Empty;
        
        IsFoodDetailsVisible = false;
        ResetPopupState();
        
        _searchCts?.Cancel();
        IsLoading = false;
    }

    private async Task RefreshExistingIdsAsync()
    {
        try 
        {
            if (_entryId.HasValue)
            {
                var result = await diaryService.GetEntryAsync(_entryId.Value);
                if (result is { Success: true, Data: not null })
                {
                    _existingProductIds = result.Data.FoodItems.Select(x => x.ExternalId).ToHashSet();
                }
            }
        }
        catch
        {
            // ignored
        }
    }

    partial void OnSearchTextChanged(string value)
    {
        _searchCts?.Cancel();
        _searchCts?.Dispose();
        _searchCts = new CancellationTokenSource();
        var token = _searchCts.Token;

        if (string.IsNullOrWhiteSpace(value))
        {
            LoadRecent(token);
            return;
        }
        
        Task.Run(async () => 
        {
            try
            {
                await Task.Delay(800, token);
                if (token.IsCancellationRequested) return;

                MainThread.BeginInvokeOnMainThread(async void () => await PerformSearch(value, token));
            }
            catch (TaskCanceledException) { }
        }, token);
    }

    private async Task PerformSearch(string query, CancellationToken token)
    {
        _currentPage = 1;
        _canLoadMore = true;
        
        IsLoading = true;
        IsShowingRecent = false;
        SearchResults.Clear();

        try
        {
            var result = await foodService.SearchFoodAsync(query, _currentPage);
            
            if (token.IsCancellationRequested) return;

            if (result is { Success: true, Data: not null })
            {
                if (result.Data.Count < PageSize) _canLoadMore = false;
                
                foreach (var item in result.Data)
                {
                    var vm = new FoodSearchItemViewModel(item);
                    if (_existingProductIds.Contains(item.ExternalId))
                    {
                        vm.IsAdded = true;
                    }
                    SearchResults.Add(vm);
                }
            }
            else if (result is { Success: false })
            {
                var errorMsg = new LocalizedString(() => result.Message);
                await alertService.ShowToastAsync(errorMsg.Localized);
            }
        }
        catch (Exception)
        {
            if (!token.IsCancellationRequested)
                await alertService.ShowToastAsync(localizationManager["UnexpectedErrorMessage"]);
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    [RelayCommand]
    private async Task LoadMore()
    {
        if (IsShowingRecent || IsLoading || IsLoadingMore || !_canLoadMore || string.IsNullOrWhiteSpace(SearchText)) 
            return;

        IsLoadingMore = true;
        _currentPage++;

        try
        {
            var result = await foodService.SearchFoodAsync(SearchText, _currentPage);

            if (result is { Success: true, Data: not null })
            {
                if (result.Data.Count < PageSize) _canLoadMore = false;

                foreach (var item in result.Data)
                {
                    var vm = new FoodSearchItemViewModel(item);
                    if (_existingProductIds.Contains(item.ExternalId)) vm.IsAdded = true;
                    SearchResults.Add(vm);
                }
            }
            else
            {
                _currentPage--; 
            }
        }
        catch
        {
            _currentPage--;
        }
        finally
        {
            IsLoadingMore = false;
        }
    }

    private async void LoadRecent(CancellationToken token = default)
    {
        if (!string.IsNullOrWhiteSpace(SearchText)) return;
        
        IsLoading = true;
        IsShowingRecent = true;
        SearchResults.Clear();

        try
        {
            var from = DateTime.Now.AddDays(-7);
            var to = DateTime.Now;

            var result = await diaryService.GetEntriesByDateRangeAsync(from, to, token);
            
            if (token.IsCancellationRequested) return;

            if (result is { Success: true, Data: not null })
            {
                var recentItems = result.Data
                    .SelectMany(e => e.FoodItems)
                    .Reverse()
                    .GroupBy(x => x.ExternalId)
                    .Select(g => g.First())
                    .Take(20)
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
                        servingString
                    );
                    
                    var vm = new FoodSearchItemViewModel(dto);
                    if (_existingProductIds.Contains(item.ExternalId))
                    {
                        vm.IsAdded = true;
                    }
                    SearchResults.Add(vm);
                }
            }
        }
        catch (Exception)
        {
            if (!token.IsCancellationRequested)
                await alertService.ShowToastAsync(localizationManager["UnexpectedErrorMessage"]);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task ToggleItem(FoodSearchItemViewModel? itemVm)
    {
        if (itemVm == null || itemVm.IsAdding) return;

        if (itemVm.IsAdded)
            await RemoveItemInternal(itemVm);
        else
            await QuickAddInternal(itemVm);
    }

    private async Task QuickAddInternal(FoodSearchItemViewModel itemVm)
    {
        itemVm.IsAdding = true;

        try
        {
            var productResult = await foodService.GetProductByIdAsync(itemVm.Data.ExternalId);

            if (productResult is { Success: true, Data: not null })
            {
                var product = productResult.Data;
            
                FoodServingDto? targetServing = null;
                double amount = 0;
                var isHistoryMatch = false;

                if (IsShowingRecent && !string.IsNullOrEmpty(itemVm.Data.Serving))
                {
                    var parts = itemVm.Data.Serving.Split(' ');
                    if (parts.Length >= 2)
                    {
                        if (double.TryParse(parts[0], out var historyAmount))
                        {
                            var historyUnit = parts.Last();
                        
                            targetServing = product.Servings.FirstOrDefault(s => 
                                string.Equals(s.MetricUnit, historyUnit, StringComparison.OrdinalIgnoreCase));

                            if (targetServing != null)
                            {
                                amount = historyAmount;
                                isHistoryMatch = true;
                            }
                        }
                    }
                }
                
                if (!isHistoryMatch)
                {
                    targetServing = product.Servings.FirstOrDefault();
                    amount = targetServing?.MetricAmount ?? 100;
                }
            
                var unit = targetServing?.MetricUnit ?? "g";
                
                var baseAmount = targetServing?.MetricAmount ?? 1;
                var ratio = amount / baseAmount;

                var newItem = new FoodItemDto(
                    product.ExternalId, 
                    product.Label, 
                    (targetServing?.Calories ?? itemVm.Calories) * ratio, 
                    (targetServing?.Carbs ?? itemVm.Carbs) * ratio, 
                    (targetServing?.Protein ?? itemVm.Protein) * ratio, 
                    (targetServing?.Fat ?? itemVm.Fat) * ratio, 
                    unit, 
                    amount
                );

                await AddEntryToDiaryInternal(newItem);
                itemVm.IsAdded = true;
                _existingProductIds.Add(product.ExternalId);
            }
            else
            {
                var errorMsg = new LocalizedString(() => productResult.Message);
                await alertService.ShowToastAsync(errorMsg.Localized);
            }
        }
        catch (Exception ex)
        {
            var errorMsg = new LocalizedString(() => ex.Message);
            await alertService.ShowToastAsync(errorMsg.Localized);
        }
        finally
        {
            itemVm.IsAdding = false;
        }
    }

    private async Task RemoveItemInternal(FoodSearchItemViewModel itemVm)
    {
        itemVm.IsAdding = true;

        try
        {
            var entriesResult = await diaryService.GetEntriesByDateAsync(_date);
            
            if (entriesResult is { Success: true, Data: not null })
            {
                var existingEntry = entriesResult.Data.FirstOrDefault(e => e.DayTime == _mealType);

                if (existingEntry != null)
                {
                    var itemToRemove = existingEntry.FoodItems
                        .FirstOrDefault(x => x.ExternalId == itemVm.Data.ExternalId);

                    if (itemToRemove != null)
                    {
                        var updatedItems = existingEntry.FoodItems.ToList();
                        updatedItems.Remove(itemToRemove);
                        
                        if (updatedItems.Count == 0)
                            await diaryService.DeleteEntryAsync(existingEntry.Id);
                        else
                        {
                            var updateRequest = new FoodEntryCreateRequest(_mealType, _date, updatedItems);
                            await diaryService.UpdateEntryAsync(existingEntry.Id, updateRequest);
                        }

                        WeakReferenceMessenger.Default.Send(new DiaryUpdatedMessage());
                        
                        itemVm.IsAdded = false;
                        _existingProductIds.Remove(itemVm.Data.ExternalId);
                    }
                }
            }
        }
        catch
        {
            await alertService.ShowToastAsync(localizationManager["UnexpectedErrorMessage"]);
        }
        finally
        {
            itemVm.IsAdding = false;
        }
    }

    [RelayCommand]
    private async Task OpenFoodDetails(FoodSearchItemViewModel? itemVm)
    {
        if (itemVm is null || IsLoading || itemVm.IsAdding || itemVm.IsAdded) return;
        
        itemVm.IsAdding = true;
        ResetPopupState();

        try
        {
            var result = await foodService.GetProductByIdAsync(itemVm.Data.ExternalId);

            if (result is { Success: true, Data: not null })
            {
                await OpenFoodDetailsInternal(result.Data, itemVm.Data);
            }
            else
            { 
                var errorMsg = new LocalizedString(() => result.Message);
                await alertService.ShowToastAsync(errorMsg.Localized);
            }
        }
        catch
        {
            await alertService.ShowToastAsync(localizationManager["UnexpectedErrorMessage"]);
        }
        finally
        {
            itemVm.IsAdding = false;
        }
    }

    private Task OpenFoodDetailsInternal(FoodProductResponse productDetails, FoodSearchResponse sourceItem)
    {
        var uniqueServings = productDetails.Servings
            .GroupBy(s => s.MetricUnit)
            .Select(g => g.First())
            .ToList();

        productDetails = productDetails with { Servings = uniqueServings };
        SelectedFoodDetail = productDetails;

        FoodServingDto? targetServing = null;

        if (IsShowingRecent && !string.IsNullOrEmpty(sourceItem.Serving))
        {
            var parts = sourceItem.Serving.Split(' ');
            if (parts.Length > 1)
            {
                var historyUnit = parts.Last();
            
                targetServing = uniqueServings.FirstOrDefault(s => 
                    string.Equals(s.MetricUnit, historyUnit, StringComparison.OrdinalIgnoreCase));
            }
        }

        targetServing ??= uniqueServings.FirstOrDefault();
        SelectedServing = targetServing;

        if (targetServing == null) 
        {
            InputAmount = "100";
            IsFoodDetailsVisible = true;
            return Task.CompletedTask;
        }

        if (IsShowingRecent && sourceItem.Calories > 0 && targetServing.Calories > 0)
        {
            var ratio = sourceItem.Calories / targetServing.Calories;
            InputAmount = Math.Round(targetServing.MetricAmount * ratio, 2).ToString(CultureInfo.InvariantCulture);
        }
        else
        {
            InputAmount = targetServing.MetricAmount.ToString(CultureInfo.InvariantCulture);
        }

        IsFoodDetailsVisible = true;
        return Task.CompletedTask;
    }

    [RelayCommand]
    private void CloseFoodDetails()
    {
        IsFoodDetailsVisible = false;
    }
    
    [RelayCommand(CanExecute = nameof(CanSaveFood))]
    private async Task SaveFood()
    {
        if (SelectedFoodDetail == null || SelectedServing == null || string.IsNullOrEmpty(InputAmount)) return;
        
        var product = SelectedFoodDetail;
        var serving = SelectedServing;
        var amount = double.Parse(InputAmount);

        var itemVm = SearchResults.FirstOrDefault(x => x.Data.ExternalId == product.ExternalId);
        itemVm?.IsAdding = true;

        IsFoodDetailsVisible = false;
        ResetPopupState();
        
        try
        {
            var ratio = amount / serving.MetricAmount;
            
            var newItem = new FoodItemDto(
                product.ExternalId, product.Label,
                serving.Calories * ratio, serving.Carbs * ratio,
                serving.Protein * ratio, serving.Fat * ratio,
                serving.MetricUnit, amount
            );

            await AddEntryToDiaryInternal(newItem);
            
            if (itemVm != null) 
            {
                itemVm.IsAdded = true;
                _existingProductIds.Add(product.ExternalId);
            }
        }
        catch (Exception ex)
        {
            var errorMsg = new LocalizedString(() => ex.Message);
            await alertService.ShowToastAsync(errorMsg.Localized);
        }
        finally
        {
            itemVm?.IsAdding = false;
        }
    }

    private async Task AddEntryToDiaryInternal(FoodItemDto newItem)
    {
        if (_entryId.HasValue)
        {
            var entryResult = await diaryService.GetEntryAsync(_entryId.Value);
            if (entryResult is { Success: true, Data: not null })
            {
                var updatedItems = entryResult.Data.FoodItems.ToList();
                updatedItems.Add(newItem);
                var updateRequest = new FoodEntryCreateRequest(_mealType, _date, updatedItems);
                await diaryService.UpdateEntryAsync(_entryId.Value, updateRequest);
            }
        }
        else
        {
            var createRequest = new FoodEntryCreateRequest(_mealType, _date, [newItem]);
            var createResponse = await diaryService.CreateEntryAsync(createRequest);
            if (createResponse is { Success: true, Data: not null })
            {
                _entryId = createResponse.Data.Id;
                EntryIdStr = _entryId.Value.ToString();
            }
        }
        WeakReferenceMessenger.Default.Send(new DiaryUpdatedMessage());
    }

    private void ResetPopupState()
    {
        SelectedFoodDetail = null;
        SelectedServing = null;
        InputAmount = null;
        NotifyPopupUpdates();
    }
    
    private bool CanSaveFood() => double.TryParse(InputAmount, out var amount) && amount is > 0 and <= 5000;

    partial void OnInputAmountChanged(string? value)
    {
        NotifyPopupUpdates();
        SaveFoodCommand.NotifyCanExecuteChanged();
    }
    
    partial void OnSelectedServingChanged(FoodServingDto? value) => NotifyPopupUpdates();

    private void NotifyPopupUpdates()
    {
        OnPropertyChanged(nameof(CurrentCalories));
        OnPropertyChanged(nameof(CurrentCarbs));
        OnPropertyChanged(nameof(CurrentProtein));
        OnPropertyChanged(nameof(CurrentFat));
    }
    
    private double CalculateNutrient(Func<FoodServingDto, double> selector)
    {
        var val = double.TryParse(InputAmount, out var amount) ? amount : 0;
        if (SelectedServing == null || SelectedServing.MetricAmount == 0 || val <= 0) return 0;
        return val * selector(SelectedServing) / SelectedServing.MetricAmount;
    }
    
    [RelayCommand] private void ToggleScanner() => IsScannerVisible = !IsScannerVisible;
    [RelayCommand] private void ToggleTorch() => IsTorchOn = !IsTorchOn;
    
    [RelayCommand]
    private async Task BarcodeDetected(string barcode)
    {
        if (string.IsNullOrEmpty(barcode)) return;

        try
        {
            HapticFeedback.Perform();
        }
        catch
        {
            // ignored
        }

        IsScannerVisible = false;
        await PerformBarcodeSearch(barcode);
    }
    
    private async Task PerformBarcodeSearch(string barcode)
    {
        IsLoading = true;
        IsShowingRecent = false;
        SearchResults.Clear();
        SearchText = barcode;

        try
        {
            var result = await foodService.GetProductByBarcodeAsync(barcode);

            if (result is { Success: true, Data: not null })
            {
                var p = result.Data;
                var baseServing = p.Servings.FirstOrDefault();

                var itemVm = new FoodSearchItemViewModel(new FoodSearchResponse(
                    p.ExternalId, p.Label, p.BrandName,
                    baseServing?.Calories ?? 0, baseServing?.Carbs ?? 0,
                    baseServing?.Protein ?? 0, baseServing?.Fat ?? 0,
                    $"{baseServing?.MetricAmount} {baseServing?.MetricUnit}")
                );
                
                if (_existingProductIds.Contains(p.ExternalId)) itemVm.IsAdded = true;

                SearchResults.Add(itemVm);
                await OpenFoodDetailsInternal(p, itemVm.Data);
            }
            else
            {
                var errorMsg = new LocalizedString(() => result.Message);
                await alertService.ShowToastAsync(errorMsg.Localized);
            }
        }
        catch
        {
            await alertService.ShowToastAsync(localizationManager["UnexpectedErrorMessage"]);
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    [RelayCommand]
    private async Task Back()
    {
        if (_entryId.HasValue)
        {
            var idStr = Uri.EscapeDataString(_entryId.Value.ToString());
            await Shell.Current.GoToAsync($"..?EntryId={idStr}", false);
        }
        else
        {
            await Shell.Current.GoToAsync("..", false);
        }
    }
}

public partial class FoodSearchItemViewModel(FoodSearchResponse data) : ObservableObject
{
    public FoodSearchResponse Data { get; } = data;

    [ObservableProperty] private bool _isAdding;
    [ObservableProperty] private bool _isAdded;

    public string Label => Data.Label;
    public string BrandName => Data.BrandName ?? "";
    public string ServingUnit => Data.Serving;
    public double Calories => Data.Calories;
    public double Carbs => Data.Carbs;
    public double Protein => Data.Protein;
    public double Fat => Data.Fat;
}
