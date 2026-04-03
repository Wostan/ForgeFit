using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ForgeFit.MAUI.ViewModels.Workout.ExerciseSearch;

public partial class ExerciseSearchViewModel : ObservableObject
{
    private CancellationTokenSource? _searchCts;

    [ObservableProperty] private string _searchText = string.Empty;
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private bool _isLoadingMore;

    private int _currentPage = 1;
    private bool _canLoadMore;

    public ObservableCollection<ExerciseSearchItemViewModel> SearchResults { get; } = [];

    public Func<string, CancellationToken, Task>? PerformSearchCallback { get; set; }
    public Func<Task>? LoadMoreCallback { get; set; }
    public Func<ExerciseSearchItemViewModel, Task>? ToggleExerciseCallback { get; set; }
    public Func<ExerciseSearchItemViewModel, Task>? OpenDetailsCallback { get; set; }

    partial void OnSearchTextChanged(string value)
    {
        _searchCts?.Cancel();
        _searchCts?.Dispose();
        _searchCts = new CancellationTokenSource();
        var token = _searchCts.Token;

        Task.Run(async () =>
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
        IsLoading = true;
        _canLoadMore = true;
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
        if (IsLoading || IsLoadingMore || !_canLoadMore || LoadMoreCallback == null) return;

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

    [RelayCommand]
    private async Task ToggleExercise(ExerciseSearchItemViewModel? itemVm)
    {
        if (itemVm == null || itemVm.IsBusy || ToggleExerciseCallback == null) return;

        itemVm.IsBusy = true;
        try
        {
            await ToggleExerciseCallback(itemVm);
        }
        finally
        {
            itemVm.IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task OpenExerciseDetails(ExerciseSearchItemViewModel? itemVm)
    {
        if (itemVm is null || IsLoading || itemVm.IsBusy || OpenDetailsCallback == null) return;

        itemVm.IsBusy = true;
        try
        {
            await OpenDetailsCallback(itemVm);
        }
        finally
        {
            itemVm.IsBusy = false;
        }
    }

    public void ResetState()
    {
        SearchText = string.Empty;
        SearchResults.Clear();
        _searchCts?.Cancel();
        IsLoading = false;
        IsLoadingMore = false;
        _currentPage = 1;
        _canLoadMore = false;
    }

    public void AddSearchResult(ExerciseSearchItemViewModel itemVm)
    {
        SearchResults.Add(itemVm);
    }

    public void SetCanLoadMore(bool canLoadMore)
    {
        _canLoadMore = canLoadMore;
    }

    public int GetCurrentPage()
    {
        return _currentPage;
    }

    public void DecrementPage()
    {
        _currentPage--;
    }
}