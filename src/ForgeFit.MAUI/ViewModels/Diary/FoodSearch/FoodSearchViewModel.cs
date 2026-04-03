using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ForgeFit.MAUI.ViewModels.Diary.FoodSearch;

public partial class FoodSearchViewModel : ObservableObject
{
    private CancellationTokenSource? _searchCts;

    [ObservableProperty] private string _searchText = string.Empty;
    [ObservableProperty] private bool _isShowingRecent = true;
    [ObservableProperty] private bool _isLoadingMore;
    [ObservableProperty] private bool _isLoading;

    private int _currentPage = 1;
    private bool _canLoadMore = true;

    public ObservableCollection<FoodSearchItemViewModel> SearchResults { get; } = [];

    public Func<string, CancellationToken, Task>? PerformSearchCallback { get; set; }
    public Func<CancellationToken, Task>? LoadRecentCallback { get; set; }
    public Func<Task>? LoadMoreCallback { get; set; }
    public Func<FoodSearchItemViewModel, Task>? ToggleItemCallback { get; set; }
    public Func<FoodSearchItemViewModel, Task>? OpenFoodDetailsCallback { get; set; }

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
                await Task.Delay(800, token);
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
        if (PerformSearchCallback == null) return;

        _currentPage = 1;
        _canLoadMore = true;
        IsLoading = true;
        IsShowingRecent = false;
        SearchResults.Clear();

        try
        {
            await PerformSearchCallback(query, token);
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
        if (LoadMoreCallback == null) return;

        IsLoadingMore = true;
        _currentPage++;

        try
        {
            await LoadMoreCallback();
        }
        finally
        {
            IsLoadingMore = false;
        }
    }

    public async Task LoadRecentAsync(CancellationToken token = default)
    {
        if (!string.IsNullOrWhiteSpace(SearchText)) return;
        if (LoadRecentCallback == null) return;

        IsLoading = true;
        IsShowingRecent = true;
        SearchResults.Clear();

        try
        {
            await LoadRecentCallback(token);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task ToggleItem(FoodSearchItemViewModel? itemVm)
    {
        if (itemVm == null || itemVm.IsAdding || ToggleItemCallback == null) return;

        itemVm.IsAdding = true;
        try
        {
            await ToggleItemCallback(itemVm);
        }
        finally
        {
            itemVm.IsAdding = false;
        }
    }

    [RelayCommand]
    private async Task QuickAddItem(FoodSearchItemViewModel? itemVm)
    {
        if (itemVm == null || itemVm.IsAdding || ToggleItemCallback == null) return;

        itemVm.IsAdding = true;
        try
        {
            await ToggleItemCallback(itemVm);
        }
        finally
        {
            itemVm.IsAdding = false;
        }
    }

    [RelayCommand]
    private async Task RemoveItem(FoodSearchItemViewModel? itemVm)
    {
        if (itemVm == null || itemVm.IsAdding || ToggleItemCallback == null) return;

        itemVm.IsAdding = true;
        try
        {
            await ToggleItemCallback(itemVm);
        }
        finally
        {
            itemVm.IsAdding = false;
        }
    }

    [RelayCommand]
    private async Task OpenFoodDetails(FoodSearchItemViewModel? itemVm)
    {
        if (itemVm is null || IsLoading || itemVm.IsAdding || itemVm.IsAdded || OpenFoodDetailsCallback == null) return;

        itemVm.IsAdding = true;
        try
        {
            await OpenFoodDetailsCallback(itemVm);
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

    public void AddSearchResult(FoodSearchItemViewModel itemVm)
    {
        SearchResults.Add(itemVm);
    }

    public void ClearSearchResults()
    {
        SearchResults.Clear();
    }

    public bool CanLoadMore()
    {
        return _canLoadMore;
    }

    public void DecrementPage()
    {
        _currentPage--;
    }

    public int GetCurrentPage()
    {
        return _currentPage;
    }

    public void SetCanLoadMore(bool canLoadMore)
    {
        _canLoadMore = canLoadMore;
    }
}