using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ForgeFit.MAUI.Messages;
using ForgeFit.MAUI.Models.DTOs.Workout;
using ForgeFit.MAUI.Services.Interfaces;
using ForgeFit.MAUI.ViewModels.Core;
using LocalizationResourceManager.Maui;

namespace ForgeFit.MAUI.ViewModels.Workout.ExerciseSearch;

public partial class ExerciseSearchPageViewModel : BaseViewModel, IQueryAttributable
{
    private const string DefaultApiQuery = " ";
    private const int PageSize = 20;
    private readonly IAlertService _alertService;
    private readonly IWorkoutExerciseService _exerciseService;
    private readonly ILocalizationResourceManager _localizationManager;

    [ObservableProperty] private string _programName = string.Empty;

    public ExerciseSearchPageViewModel(
        IWorkoutExerciseService exerciseService,
        IAlertService alertService,
        ILocalizationResourceManager localizationManager)
    {
        _exerciseService = exerciseService;
        _alertService = alertService;
        _localizationManager = localizationManager;

        SearchVM = new ExerciseSearchViewModel();
        FiltersVM = new ExerciseFiltersViewModel();
        DetailsVM = new ExerciseDetailsViewModel();

        SetupCallbacks();
    }

    public ExerciseSearchViewModel SearchVM { get; }
    public ExerciseFiltersViewModel FiltersVM { get; }
    public ExerciseDetailsViewModel DetailsVM { get; }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        ResetState();

        if (query.TryGetValue(nameof(ProgramName), out var name) && name is string nameStr)
            ProgramName = nameStr;

        FiltersVM.InitializeFilters();

        SearchVM.SearchText = " ";
        SearchVM.SearchText = string.Empty;
    }

    private void SetupCallbacks()
    {
        SearchVM.PerformSearchCallback = async (query, token) => await PerformSearchAsync(query, token);
        SearchVM.LoadMoreCallback = async () => await LoadMoreAsync();
        SearchVM.ToggleExerciseCallback = async itemVm => await ToggleExerciseAsync(itemVm);
        SearchVM.OpenDetailsCallback = async itemVm => await OpenDetailsAsync(itemVm);

        FiltersVM.ApplyFiltersCallback = async () => await PerformSearchAsync(SearchVM.SearchText, CancellationToken.None);

        DetailsVM.AddFromDetailsCallback = async exercise => await AddFromDetailsAsync(exercise);
    }

    private void ResetState()
    {
        SearchVM.ResetState();
        FiltersVM.ResetState();
        DetailsVM.ResetState();
    }

    private async Task PerformSearchAsync(string query, CancellationToken token)
    {
        var selectedMuscles = FiltersVM.GetSelectedMuscles();
        var selectedParts = FiltersVM.GetSelectedBodyParts();
        var selectedEq = FiltersVM.GetSelectedEquipment();

        var queryToSend = GetQueryToSend(query);

        try
        {
            var result = await _exerciseService.SearchExercisesAsync(
                queryToSend,
                selectedMuscles,
                selectedParts,
                selectedEq);

            if (token.IsCancellationRequested) return;

            if (result is not { Success: true, Data: not null })
            {
                if (result is not { Success: false }) return;

                var errorMsg = new LocalizedString(() => result.Message);
                await _alertService.ShowToastAsync(errorMsg.Localized);
                return;
            }

            if (result.Data.Count < PageSize) SearchVM.SetCanLoadMore(false);
            foreach (var item in result.Data)
                SearchVM.AddSearchResult(new ExerciseSearchItemViewModel(item));
        }
        catch
        {
            if (!token.IsCancellationRequested)
                await _alertService.ShowToastAsync(_localizationManager["UnexpectedErrorMessage"]);
        }
    }

    private async Task LoadMoreAsync()
    {
        var selectedMuscles = FiltersVM.GetSelectedMuscles();
        var selectedParts = FiltersVM.GetSelectedBodyParts();
        var selectedEq = FiltersVM.GetSelectedEquipment();

        var queryToSend = GetQueryToSend(SearchVM.SearchText);

        try
        {
            var result = await _exerciseService.SearchExercisesAsync(
                queryToSend,
                selectedMuscles,
                selectedParts,
                selectedEq,
                SearchVM.GetCurrentPage());

            if (result is not { Success: true, Data: not null })
            {
                if (result is { Success: false })
                {
                    var errorMsg = new LocalizedString(() => result.Message);
                    await _alertService.ShowToastAsync(errorMsg.Localized);
                }

                SearchVM.DecrementPage();
                return;
            }

            if (result.Data.Count < PageSize) SearchVM.SetCanLoadMore(false);
            foreach (var item in result.Data)
                SearchVM.AddSearchResult(new ExerciseSearchItemViewModel(item));
        }
        catch
        {
            await _alertService.ShowToastAsync(_localizationManager["UnexpectedErrorMessage"]);
            SearchVM.DecrementPage();
        }
    }

    private static string GetQueryToSend(string userInput)
    {
        return string.IsNullOrWhiteSpace(userInput) ? DefaultApiQuery : userInput;
    }

    private async Task ToggleExerciseAsync(ExerciseSearchItemViewModel itemVm)
    {
        try
        {
            var detailsResult = await _exerciseService.GetExerciseByIdAsync(itemVm.Data.ExternalId);

            if (detailsResult is not { Success: true, Data: not null })
            {
                var errorMsg = new LocalizedString(() => detailsResult.Message);
                await _alertService.ShowToastAsync(errorMsg.Localized);
                return;
            }

            var exerciseDto = detailsResult.Data;
            WeakReferenceMessenger.Default.Send(new AddExerciseMessage(exerciseDto));
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            Debug.WriteLine("EXERCISE ADDING ERROR: " + ex.Message);
            await _alertService.ShowToastAsync(_localizationManager["UnexpectedErrorMessage"]);
        }
    }

    private async Task OpenDetailsAsync(ExerciseSearchItemViewModel itemVm)
    {
        var details = await _exerciseService.GetExerciseByIdAsync(itemVm.Data.ExternalId);

        if (details is not { Success: true, Data: not null })
        {
            if (details is not { Success: false }) return;

            var errorMsg = new LocalizedString(() => details.Message);
            await _alertService.ShowToastAsync(errorMsg.Localized);
            return;
        }

        DetailsVM.OpenDetails(details.Data);
    }

    private async Task AddFromDetailsAsync(WorkoutExerciseDto exercise)
    {
        WeakReferenceMessenger.Default.Send(new AddExerciseMessage(exercise));
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task Back()
    {
        if (DetailsVM.IsDetailsVisible)
        {
            DetailsVM.CloseDetailsCommand.Execute(null);
            return;
        }

        if (FiltersVM.IsFiltersVisible)
        {
            FiltersVM.ToggleFiltersCommand.Execute(null);
            return;
        }

        await Shell.Current.GoToAsync("..", false);
    }
}

public partial class FilterItem<T>(T value, string name) : ObservableObject
{
    [ObservableProperty] private bool _isSelected;
    public T Value { get; } = value;
    public string Name { get; } = name;
}

public partial class ExerciseSearchItemViewModel(WorkoutExerciseSearchResponse data) : ObservableObject
{
    [ObservableProperty] private bool _isBusy;
    public WorkoutExerciseSearchResponse Data { get; } = data;

    public string Name => Data.Name;
    public Uri? GifUrl => Data.GifUrl;

    public string TargetMusclesDisplay => string.Join(", ", Data.TargetMuscles);
}