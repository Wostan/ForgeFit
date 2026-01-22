using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ForgeFit.MAUI.Models.DTOs.Workout;
using ForgeFit.MAUI.Models.Enums.ProfileEnums;
using ForgeFit.MAUI.Services.Interfaces;
using LocalizationResourceManager.Maui;

namespace ForgeFit.MAUI.ViewModels;

public partial class ActiveWorkoutPageViewModel(
    IWorkoutProgramService workoutProgramService,
    IWorkoutTrackingService workoutTrackingService,
    IAlertService alertService,
    ILocalizationResourceManager localizationManager)
    : BaseViewModel, IQueryAttributable
{
    private IDispatcherTimer? _timer;
    private TimeSpan _totalWorkoutDuration;
    private TimeSpan _currentRestTime;
    private bool _isResting;

    private string _programName = string.Empty;
    private string? _programDescription;

    private Func<Task>? _pendingConfirmationAction;

    [ObservableProperty] private Guid _programId;
    [ObservableProperty] private string _headerTitle = "00:00:00";
    [ObservableProperty] private ObservableCollection<ActiveExerciseItem> _exercises = [];

    [ObservableProperty] private bool _isConfirmationPopupVisible;
    [ObservableProperty] private string _confirmationTitle = string.Empty;
    [ObservableProperty] private string _confirmationMessage = string.Empty;

    [ObservableProperty] private bool _isEntryPopupVisible;
    [ObservableProperty] private string _entryPopupTitle = string.Empty;
    [ObservableProperty] private string _entryPopupPlaceholder = string.Empty;
    [ObservableProperty] private TimeSpan _entryPopupDuration;

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue(nameof(ProgramId), out var id) && id is string idStr && Guid.TryParse(idStr, out var guid))
        {
            ProgramId = guid;
            InitializeCommand.Execute(null);
        }
    }

    [RelayCommand]
    private async Task Initialize()
    {
        IsLoading = true;
        Error = null;
        try
        {
            var result = await workoutProgramService.GetProgramAsync(ProgramId);
            if (!result.Success || result.Data == null)
            {
                var errorMsg = new LocalizedString(() => result.Message);
                HandleError(errorMsg);
                return;
            }

            _programName = result.Data.Name;
            _programDescription = result.Data.Description;

            var mappedExercises = result.Data.WorkoutExercisePlans
                .Select(plan => new ActiveExerciseItem(
                    plan, 
                    OnSetCompleted, 
                    DeleteSetCommand, 
                    AskDeleteExerciseCommand, 
                    AddSetCommand))
                .ToList();
            
            Exercises = new ObservableCollection<ActiveExerciseItem>(mappedExercises);

            StartTimer();
        }
        catch (Exception)
        {
            var genericError = new LocalizedString(() => localizationManager["UnexpectedErrorMessage"]);
            HandleError(genericError);
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void StartTimer()
    {
        _totalWorkoutDuration = TimeSpan.Zero;
        _timer = Application.Current?.Dispatcher.CreateTimer();
        if (_timer != null)
        {
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += OnTimerTick;
            _timer.Start();
        }
    }

    private void OnTimerTick(object? sender, EventArgs e)
    {
        _totalWorkoutDuration = _totalWorkoutDuration.Add(TimeSpan.FromSeconds(1));

        if (_isResting)
        {
            if (_currentRestTime.TotalSeconds > 0)
            {
                _currentRestTime = _currentRestTime.Add(TimeSpan.FromSeconds(-1));
                HeaderTitle = _currentRestTime.ToString(@"mm\:ss");
            }
            else
            {
                _isResting = false;
            }
        }

        if (!_isResting)
        {
            HeaderTitle = _totalWorkoutDuration.ToString(@"hh\:mm\:ss");
        }
    }

    private void OnSetCompleted(ActiveSetItem set)
    {
        if (set.IsCompleted)
        {
            _currentRestTime = set.RestTime;
            _isResting = true;
            HeaderTitle = _currentRestTime.ToString(@"mm\:ss");
        }
        else
        {
            _isResting = false;
            HeaderTitle = _totalWorkoutDuration.ToString(@"hh\:mm\:ss");
        }
    }

    [RelayCommand]
    private void AddSet(ActiveExerciseItem exercise)
    {
        if (exercise.Sets.Count >= 20)
        {
             alertService.ShowToastAsync(localizationManager["Error_MaxSetsReached"]);
             return;
        }

        var lastSet = exercise.Sets.LastOrDefault();
        var newOrder = (lastSet?.Order ?? 0) + 1;
        
        var newSet = new ActiveSetItem(new WorkoutSetDto(
            Guid.NewGuid(), 
            newOrder, 
            lastSet?.Reps ?? 10, 
            lastSet?.RestTime ?? TimeSpan.FromMinutes(1.5), 
            lastSet?.Weight ?? 0, 
            lastSet?.WeightUnit ?? WeightUnit.Kg), 
            OnSetCompleted,
            DeleteSetCommand);

        exercise.Sets.Add(newSet);
    }

    [RelayCommand]
    private void DeleteSet(ActiveSetItem set)
    {
        var parentExercise = Exercises.FirstOrDefault(e => e.Sets.Contains(set));
        if (parentExercise == null) return;
        
        if (parentExercise.Sets.Count <= 1)
        {
            alertService.ShowToastAsync(localizationManager["Error_CannotDeleteLastSet"]);
            return;
        }

        parentExercise.Sets.Remove(set);
            
        for (var i = 0; i < parentExercise.Sets.Count; i++)
        {
            parentExercise.Sets[i].Order = i + 1;
        }
    }

    [RelayCommand]
    private void AskDeleteExercise(ActiveExerciseItem exercise)
    {
        ConfirmationTitle = localizationManager["Title_DeleteExercise"];
        ConfirmationMessage = localizationManager["Msg_DeleteExerciseConfirm"];
        _pendingConfirmationAction = async () => 
        {
            Exercises.Remove(exercise);
            await Task.CompletedTask;
        };
        IsConfirmationPopupVisible = true;
    }

    [RelayCommand]
    private void AskFinishWorkout()
    {
        if (!Exercises.Any())
        {
            alertService.ShowToastAsync(localizationManager["Error_NoExercisesInWorkout"]);
            return;
        }

        if (Exercises.Any(e => e.Sets.Count == 0))
        {
            alertService.ShowToastAsync(localizationManager["Error_ExerciseWithoutSets"]);
            return;
        }

        var totalMinutes = _totalWorkoutDuration.TotalMinutes;
        
        if (totalMinutes is < 10 or > 300)
        {
            EntryPopupTitle = localizationManager["Title_AdjustDuration"];
            EntryPopupPlaceholder = localizationManager["Placeholder_EnterDurationMinutes"];
            
            EntryPopupDuration = _totalWorkoutDuration;

            if (EntryPopupDuration.TotalMinutes < 10) 
                EntryPopupDuration = TimeSpan.FromMinutes(10); 
            
            IsEntryPopupVisible = true;
            return;
        }

        ShowFinishConfirmation();
    }

    [RelayCommand]
    private void SaveCorrectedDuration()
    {
        var minutes = EntryPopupDuration.TotalMinutes;

        switch (minutes)
        {
            case < 10:
                alertService.ShowToastAsync(localizationManager["Error_DurationTooShort"]);
                return;
            case > 300:
                alertService.ShowToastAsync(localizationManager["Error_DurationTooLong"]);
                return;
        }

        _totalWorkoutDuration = EntryPopupDuration;
        IsEntryPopupVisible = false;
        
        ShowFinishConfirmation();
    }

    [RelayCommand]
    private void CloseEntryPopup()
    {
        IsEntryPopupVisible = false;
    }

    private void ShowFinishConfirmation()
    {
        ConfirmationTitle = localizationManager["Title_FinishWorkout"];
        ConfirmationMessage = localizationManager["Msg_FinishWorkoutConfirm"];
        _pendingConfirmationAction = FinishWorkoutInternal;
        IsConfirmationPopupVisible = true;
    }

    [RelayCommand]
    private async Task ConfirmAction()
    {
        IsConfirmationPopupVisible = false;
        if (_pendingConfirmationAction != null)
        {
            await _pendingConfirmationAction.Invoke();
            _pendingConfirmationAction = null;
        }
    }

    [RelayCommand]
    private void CloseConfirmationPopup()
    {
        IsConfirmationPopupVisible = false;
        _pendingConfirmationAction = null;
    }

    private async Task FinishWorkoutInternal()
    {
        StopTimer();
        IsLoading = true;

        try
        {
            var endTime = TimeOnly.FromDateTime(DateTime.Now);
            // Используем _totalWorkoutDuration (которое могло быть исправлено в попапе)
            var startTime = endTime.Add(-_totalWorkoutDuration);

            var workoutEntry = new WorkoutEntryDto(
                Guid.NewGuid(),
                ProgramId,
                startTime,
                endTime,
                Exercises.Select(e => e.ToPerformedExerciseDto()).ToList(),
                CalculateTotalVolume(),
                CalculateTotalReps()
            );

            // Для обновления программы мы берем текущее состояние
            // (даже если там что-то пустое - мы убрали валидацию на бэке для плана)
            var updatedPlans = Exercises.Select(e => new WorkoutExercisePlanDto(
                e.Id,
                ProgramId,
                e.WorkoutExercise,
                e.Sets.Select(s => s.ToDto()).ToList()
            )).ToList();

            var programUpdateRequest = new WorkoutProgramRequest(
                _programName, 
                _programDescription, 
                updatedPlans
            );

            var logTask = workoutTrackingService.LogEntryAsync(workoutEntry);
            var updateTask = workoutProgramService.UpdateProgramAsync(ProgramId, programUpdateRequest);

            await Task.WhenAll(logTask, updateTask);

            string? errorMessage = null;
            
            if (!logTask.Result.Success) 
                errorMessage = logTask.Result.Message;
            else if (!updateTask.Result.Success) 
                errorMessage = updateTask.Result.Message;

            if (errorMessage != null)
            {
                 var errorMsg = new LocalizedString(() => errorMessage);
                 await alertService.ShowToastAsync(errorMsg.Localized);
                 return;
            }
            
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception)
        {
             var genericError = new LocalizedString(() => localizationManager["UnexpectedErrorMessage"]);
             await alertService.ShowToastAsync(genericError.Localized);
             _timer?.Start();
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    [RelayCommand] 
    private void AskCancelWorkout()
    {
        ConfirmationTitle = localizationManager["Title_CancelWorkout"];
        ConfirmationMessage = localizationManager["Msg_CancelWorkoutConfirm"];
        
        _pendingConfirmationAction = CancelWorkoutInternal;
        
        IsConfirmationPopupVisible = true;
    }

    private async Task CancelWorkoutInternal()
    {
        StopTimer();
        await Shell.Current.GoToAsync("..");
    }

    private void StopTimer()
    {
        if (_timer == null) return;
        
        _timer.Stop();
        _timer.Tick -= OnTimerTick;
        _timer = null;
    }

    private double CalculateTotalVolume() => 
        Exercises.Sum(e => e.Sets.Where(s => s.IsCompleted).Sum(s => s.Weight * s.Reps));

    private double CalculateTotalReps() => 
        Exercises.Sum(e => e.Sets.Where(s => s.IsCompleted).Sum(s => s.Reps));

    private void HandleError(LocalizedString errorMsg)
    {
        if (Exercises.Any())
            alertService.ShowToastAsync(errorMsg.Localized);
        else
            Error = errorMsg;
    }
}

public class ActiveExerciseItem : ObservableObject
{
    public Guid Id { get; }
    public WorkoutExerciseDto WorkoutExercise { get; }
    public ObservableCollection<ActiveSetItem> Sets { get; }

    public IRelayCommand DeleteExerciseCommand { get; }
    public IRelayCommand AddSetCommand { get; }

    public ActiveExerciseItem(
        WorkoutExercisePlanDto dto, 
        Action<ActiveSetItem> onSetAction,
        IRelayCommand deleteSetCommand,
        IRelayCommand deleteExerciseCommand,
        IRelayCommand addSetCommand)
    {
        Id = dto.Id;
        WorkoutExercise = dto.WorkoutExercise;
        DeleteExerciseCommand = deleteExerciseCommand;
        AddSetCommand = addSetCommand;

        Sets = new ObservableCollection<ActiveSetItem>(
            dto.WorkoutSets.OrderBy(s => s.Order)
                           .Select(s => new ActiveSetItem(s, onSetAction, deleteSetCommand))
        );
    }

    public PerformedExerciseDto ToPerformedExerciseDto()
    {
        return new PerformedExerciseDto(
            WorkoutExercise,
            Sets.Select(s => s.ToPerformedSetDto()).ToList()
        );
    }
}

public partial class ActiveSetItem : ObservableObject
{
    private readonly Action<ActiveSetItem> _onCompletedChanged;

    public Guid Id { get; }
    [ObservableProperty] private int _order;
    [ObservableProperty] private int _reps;
    [ObservableProperty] private double _weight;
    [ObservableProperty] private TimeSpan _restTime;
    [ObservableProperty] private WeightUnit _weightUnit;
    
    [ObservableProperty] private bool _isCompleted;
    
    public IRelayCommand DeleteCommand { get; }

    partial void OnIsCompletedChanged(bool value)
    {
        _onCompletedChanged?.Invoke(this);
    }

    public ActiveSetItem(
        WorkoutSetDto dto, 
        Action<ActiveSetItem> onCompletedChanged,
        IRelayCommand deleteCommand)
    {
        Id = dto.Id;
        Order = dto.Order;
        Reps = dto.Reps;
        Weight = dto.Weight;
        RestTime = dto.RestTime;
        WeightUnit = dto.WeightUnit;
        _onCompletedChanged = onCompletedChanged;
        DeleteCommand = deleteCommand;
    }
    
    [RelayCommand]
    private void ToggleComplete()
    {
        IsCompleted = !IsCompleted;
    }

    public PerformedSetDto ToPerformedSetDto()
    {
        return new PerformedSetDto(Order, Reps, Weight, WeightUnit, IsCompleted);
    }
    
    public WorkoutSetDto ToDto()
    {
        return new WorkoutSetDto(Id, Order, Reps, RestTime, Weight, WeightUnit);
    }
}
