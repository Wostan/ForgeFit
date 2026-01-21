using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ForgeFit.MAUI.Messages;
using ForgeFit.MAUI.Models.DTOs.Workout;
using ForgeFit.MAUI.Models.Enums.WorkoutEnums;
using ForgeFit.MAUI.Services.Interfaces;
using LocalizationResourceManager.Maui;

namespace ForgeFit.MAUI.ViewModels;

public partial class ExerciseSearchPageViewModel(
    IWorkoutExerciseService exerciseService,
    IAlertService alertService,
    ILocalizationResourceManager localizationManager)
    : BaseViewModel, IQueryAttributable
{
    private const string DefaultApiQuery = " ";
    
    private CancellationTokenSource? _searchCts;

    [ObservableProperty] private string _searchText = string.Empty;
    [ObservableProperty] private bool _isLoadingMore;
    private int _currentPage = 1;
    private const int PageSize = 20;
    private bool _canLoadMore = true;

    [ObservableProperty] private bool _isFiltersVisible;
    [ObservableProperty] private bool _isDetailsVisible;

    public ObservableCollection<FilterItem<Muscle>> FilterMuscles { get; } = [];
    public ObservableCollection<FilterItem<BodyPart>> FilterBodyParts { get; } = [];
    public ObservableCollection<FilterItem<Equipment>> FilterEquipment { get; } = [];

    public ObservableCollection<ExerciseSearchItemViewModel> SearchResults { get; } = [];
    [ObservableProperty] private WorkoutExerciseDto? _selectedExerciseDetail;

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        ResetState();
        InitializeFilters();
        await PerformSearch(string.Empty);
    }

    private void ResetState()
    {
        SearchResults.Clear();
        SearchText = string.Empty;
        IsFiltersVisible = false;
        IsDetailsVisible = false;
        _searchCts?.Cancel();
    }

    private void InitializeFilters()
    {
        if (FilterMuscles.Count == 0)
        {
            foreach (var muscle in Enum.GetValues<Muscle>())
                FilterMuscles.Add(new FilterItem<Muscle>(muscle, ConvertEnumToReadable(muscle.ToString())));
        }

        if (FilterBodyParts.Count == 0)
        {
            foreach (var part in Enum.GetValues<BodyPart>())
                FilterBodyParts.Add(new FilterItem<BodyPart>(part, ConvertEnumToReadable(part.ToString())));
        }

        if (FilterEquipment.Count == 0)
        {
            foreach (var eq in Enum.GetValues<Equipment>())
                FilterEquipment.Add(new FilterItem<Equipment>(eq, ConvertEnumToReadable(eq.ToString())));
        }
        
        foreach (var item in FilterMuscles) item.IsSelected = false;
        foreach (var item in FilterBodyParts) item.IsSelected = false;
        foreach (var item in FilterEquipment) item.IsSelected = false;
    }
    
    private static string ConvertEnumToReadable(string text)
    {
        return System.Text.RegularExpressions.Regex.Replace(text, "(\\B[A-Z])", " $1");
    }

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
                
                MainThread.BeginInvokeOnMainThread(async void () => await PerformSearch(value, token));
            }
            catch (TaskCanceledException) { }
        }, token);
    }

    private async Task PerformSearch(string query, CancellationToken token = default)
    {
        _currentPage = 1;
        _canLoadMore = true;
        IsLoading = true;
        SearchResults.Clear();
        IsDetailsVisible = false;

        var selectedMuscles = FilterMuscles.Where(x => x.IsSelected).Select(x => x.Value).ToList();
        var selectedParts = FilterBodyParts.Where(x => x.IsSelected).Select(x => x.Value).ToList();
        var selectedEq = FilterEquipment.Where(x => x.IsSelected).Select(x => x.Value).ToList();
        
        var queryToSend = GetQueryToSend(query);

        try
        {
            var result = await exerciseService.SearchExercisesAsync(
                queryToSend,
                selectedMuscles, 
                selectedParts, 
                selectedEq, 
                _currentPage);

            if (token.IsCancellationRequested) return;

            if (result is { Success: true, Data: not null })
            {
                if (result.Data.Count < PageSize) _canLoadMore = false;

                foreach (var item in result.Data)
                {
                    SearchResults.Add(new ExerciseSearchItemViewModel(item));
                }
            }
            else if (result is { Success: false })
            {
                var errorMsg = new LocalizedString(() => result.Message); 
                await alertService.ShowToastAsync(errorMsg.Localized);
            }
        }
        catch
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
        if (IsLoading || IsLoadingMore || !_canLoadMore) return;

        IsLoadingMore = true;
        _currentPage++;
        
        var selectedMuscles = FilterMuscles.Where(x => x.IsSelected).Select(x => x.Value).ToList();
        var selectedParts = FilterBodyParts.Where(x => x.IsSelected).Select(x => x.Value).ToList();
        var selectedEq = FilterEquipment.Where(x => x.IsSelected).Select(x => x.Value).ToList();

        var queryToSend = GetQueryToSend(SearchText);
        
        try
        {
            var result = await exerciseService.SearchExercisesAsync(
                queryToSend, 
                selectedMuscles, 
                selectedParts, 
                selectedEq, 
                _currentPage);

            if (result is { Success: true, Data: not null })
            {
                if (result.Data.Count < PageSize) _canLoadMore = false;

                foreach (var item in result.Data)
                {
                    SearchResults.Add(new ExerciseSearchItemViewModel(item));
                }
            }
            else if (result is { Success: false })
            {
                var errorMsg = new LocalizedString(() => result.Message); 
                await alertService.ShowToastAsync(errorMsg.Localized);
                _currentPage--;
            }
        }
        catch
        { 
            await alertService.ShowToastAsync(localizationManager["UnexpectedErrorMessage"]);
            _currentPage--;
        }
        finally
        {
            IsLoadingMore = false;
        }
    }
    
    private static string GetQueryToSend(string userInput)
    {
        return string.IsNullOrWhiteSpace(userInput) ? DefaultApiQuery : userInput;
    }

    [RelayCommand]
    private void ToggleFilters()
    {
        IsFiltersVisible = !IsFiltersVisible;
    }
    
    [RelayCommand]
    private void ToggleFilterSelection(object? item)
    {
        switch (item)
        {
            case FilterItem<Muscle> muscleItem:
                muscleItem.IsSelected = !muscleItem.IsSelected;
                break;
            case FilterItem<BodyPart> bodyPartItem:
                bodyPartItem.IsSelected = !bodyPartItem.IsSelected;
                break;
            case FilterItem<Equipment> equipmentItem:
                equipmentItem.IsSelected = !equipmentItem.IsSelected;
                break;
        }
    }

    [RelayCommand]
    private async Task ApplyFilters()
    {
        IsFiltersVisible = false;
        await PerformSearch(SearchText);
    }

    [RelayCommand]
    private async Task ToggleExercise(ExerciseSearchItemViewModel? itemVm)
    {
        if (itemVm == null || itemVm.IsBusy) return;
        
        itemVm.IsBusy = true;
        await ReturnExerciseToWorkout(itemVm.Data.ExternalId);
        itemVm.IsBusy = false;
    }

    private async Task ReturnExerciseToWorkout(string externalId)
    {
        try
        {
            var detailsResult = await exerciseService.GetExerciseByIdAsync(externalId);

            if (detailsResult is { Success: true, Data: not null })
            {
                var exerciseDto = detailsResult.Data;

                WeakReferenceMessenger.Default.Send(new AddExerciseMessage(exerciseDto));
                
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                var errorMsg = new LocalizedString(() => detailsResult.Message);
                await alertService.ShowToastAsync(errorMsg.Localized);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("EXERCISE ADDING ERROR: " + ex.Message);
            await alertService.ShowToastAsync(localizationManager["UnexpectedErrorMessage"]);
        }
    }

    [RelayCommand]
    private async Task OpenExerciseDetails(ExerciseSearchItemViewModel? itemVm)
    {
        if (itemVm == null || IsLoading || itemVm.IsBusy) return;
        
        itemVm.IsBusy = true;
        try 
        {
            var details = await exerciseService.GetExerciseByIdAsync(itemVm.Data.ExternalId);
            if (details is { Success: true, Data: not null })
            {
                SelectedExerciseDetail = details.Data;
                IsDetailsVisible = true;
            }
            else if (details is { Success: false })
            {
                var errorMsg = new LocalizedString(() => details.Message); 
                await alertService.ShowToastAsync(errorMsg.Localized);
            }
        }
        catch
        {
            await alertService.ShowToastAsync(localizationManager["UnexpectedErrorMessage"]);
        }
        finally
        {
            itemVm.IsBusy = false;
        }
    }

    [RelayCommand]
    private void CloseExerciseDetails()
    {
        IsDetailsVisible = false;
        SelectedExerciseDetail = null;
    }
    
    [RelayCommand]
    private async Task AddFromDetails()
    {
        if (SelectedExerciseDetail == null) return;

        IsDetailsVisible = false;

        WeakReferenceMessenger.Default.Send(new AddExerciseMessage(SelectedExerciseDetail));

        await Shell.Current.GoToAsync("..");
    }
    
    [RelayCommand]
    private async Task Back()
    {
        await Shell.Current.GoToAsync("..", false);
    }
}

public partial class FilterItem<T>(T value, string name) : ObservableObject
{
    public T Value { get; } = value;
    public string Name { get; } = name;
    
    [ObservableProperty] private bool _isSelected;
}

public partial class ExerciseSearchItemViewModel(WorkoutExerciseSearchResponse data) : ObservableObject
{
    public WorkoutExerciseSearchResponse Data { get; } = data;

    [ObservableProperty] private bool _isBusy;

    public string Name => Data.Name;
    public Uri? GifUrl => Data.GifUrl;
    
    public string TargetMusclesDisplay => string.Join(", ", Data.TargetMuscles);
}
