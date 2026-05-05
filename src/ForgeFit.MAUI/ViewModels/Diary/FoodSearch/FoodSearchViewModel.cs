using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ForgeFit.MAUI.Constants;
using ForgeFit.MAUI.Models.DTOs.Food;
using ForgeFit.MAUI.Services.Interfaces;
using LocalizationResourceManager.Maui;

namespace ForgeFit.MAUI.ViewModels.Diary.FoodSearch;

public partial class FoodSearchViewModel : ObservableObject
{
    private readonly IFoodService _foodService;
    private readonly IDiaryService _diaryService;
    private readonly IAlertService _alertService;
    private readonly ILocalizationResourceManager _localizationManager;
    private readonly FoodDiaryIntegrationViewModel _diaryVM;
    private readonly FoodDetailsViewModel _detailsVM;

    private bool _canLoadMore = true;
    private int _currentPage = 1;
    private CancellationTokenSource? _searchCts;

    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private bool _isLoadingMore;
    [ObservableProperty] private bool _isShowingRecent = true;
    [ObservableProperty] private string _searchText = string.Empty;

    public ObservableCollection<FoodSearchItemViewModel> SearchResults { get; } = [];

    public FoodSearchViewModel(
        IFoodService foodService,
        IDiaryService diaryService,
        IAlertService alertService,
        ILocalizationResourceManager localizationManager,
        FoodDiaryIntegrationViewModel diaryVM,
        FoodDetailsViewModel detailsVM)
    {
        _foodService = foodService;
        _diaryService = diaryService;
        _alertService = alertService;
        _localizationManager = localizationManager;
        _diaryVM = diaryVM;
        _detailsVM = detailsVM;
    }

    partial void OnSearchTextChanged(string value)
    {
        _searchCts?.Cancel();
        _searchCts?.Dispose();
        _searchCts = new CancellationTokenSource();
        var token = _searchCts.Token;

        if (string.IsNullOrWhiteSpace(value))
        {
            _ = Task.Run(async () => await LoadRecentAsync(token), token);
            return;
        }

        _ = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(AppConstants.SearchConfig.DebounceDelayMs, token);
                if (token.IsCancellationRequested) return;
                MainThread.BeginInvokeOnMainThread(async void () => await PerformSearchAsync(value, token));
            }
            catch (TaskCanceledException)
            {
            }
        }, token);
    }

    private async Task PerformSearchAsync(string query, CancellationToken token)
    {
        _currentPage = 1;
        _canLoadMore = true;
        IsLoading = true;
        IsShowingRecent = false;
        SearchResults.Clear();

        try
        {
            var result = await _foodService.SearchFoodAsync(query, _currentPage);

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

            if (result.Data.Count < AppConstants.SearchConfig.DefaultPageSize) SetCanLoadMore(false);

            foreach (var item in result.Data)
            {
                var vm = new FoodSearchItemViewModel(item);
                if (_diaryVM.IsProductAdded(item.ExternalId)) vm.IsAdded = true;
                AddSearchResult(vm);
            }
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
            var result = await _foodService.SearchFoodAsync(SearchText, _currentPage);

            if (result is not { Success: true, Data: not null })
            {
                DecrementPage();
                return;
            }

            if (result.Data.Count < AppConstants.SearchConfig.DefaultPageSize) SetCanLoadMore(false);

            foreach (var item in result.Data)
            {
                var vm = new FoodSearchItemViewModel(item);
                if (_diaryVM.IsProductAdded(item.ExternalId)) vm.IsAdded = true;
                AddSearchResult(vm);
            }
        }
        finally
        {
            IsLoadingMore = false;
        }
    }

    public async Task LoadRecentAsync(CancellationToken token = default)
    {
        if (!string.IsNullOrWhiteSpace(SearchText)) return;

        IsLoading = true;
        IsShowingRecent = true;
        SearchResults.Clear();

        try
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
                if (_diaryVM.IsProductAdded(item.ExternalId)) vm.IsAdded = true;
                AddSearchResult(vm);
            }
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

        itemVm.IsAdding = true;
        try
        {
            if (itemVm.IsAdded)
                await _diaryVM.RemoveItemInternal(itemVm);
            else
                await _diaryVM.QuickAddInternal(itemVm, IsShowingRecent);
        }
        finally
        {
            itemVm.IsAdding = false;
        }
    }

    [RelayCommand]
    private async Task QuickAddItem(FoodSearchItemViewModel? itemVm) => await ToggleItem(itemVm);

    [RelayCommand]
    private async Task RemoveItem(FoodSearchItemViewModel? itemVm) => await ToggleItem(itemVm);

    [RelayCommand]
    private async Task OpenFoodDetails(FoodSearchItemViewModel? itemVm)
    {
        if (itemVm is null || IsLoading || itemVm.IsAdding || itemVm.IsAdded) return;

        itemVm.IsAdding = true;
        try
        {
            var result = await _foodService.GetProductByIdAsync(itemVm.Data.ExternalId);

            if (result is { Success: true, Data: not null })
                await _detailsVM.OpenFoodDetailsInternal(result.Data, itemVm.Data, IsShowingRecent);
            else
                await _alertService.ShowToastAsync(new LocalizedString(() => result.Message).Localized);
        }
        finally
        {
            itemVm.IsAdding = false;
        }
    }

    public void ResetState()
    {
        SearchText = string.Empty;
        IsShowingRecent = true;
        SearchResults.Clear();
        _searchCts?.Cancel();
        IsLoading = false;
        IsLoadingMore = false;
        _currentPage = 1;
        _canLoadMore = true;
    }

    public void AddSearchResult(FoodSearchItemViewModel itemVm) => SearchResults.Add(itemVm);
    public void ClearSearchResults() => SearchResults.Clear();
    public void DecrementPage() => _currentPage--;
    public void SetCanLoadMore(bool canLoadMore) => _canLoadMore = canLoadMore;
    public int GetCurrentPage() => _currentPage;
}