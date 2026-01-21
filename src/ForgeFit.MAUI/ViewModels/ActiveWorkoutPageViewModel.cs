using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ForgeFit.MAUI.Messages;
using ForgeFit.MAUI.Models.DTOs.Workout;
using ForgeFit.MAUI.Models.Enums.ProfileEnums;
using ForgeFit.MAUI.Services.Interfaces;
using ForgeFit.MAUI.Views.Workout;
using LocalizationResourceManager.Maui;

namespace ForgeFit.MAUI.ViewModels;

public partial class ActiveWorkoutPageViewModel : BaseViewModel, IQueryAttributable
{
    private IDispatcherTimer? _timer;
    private TimeSpan _totalWorkoutDuration;
    private TimeSpan _currentRestTime;
    private bool _isResting;
    
    private bool _isInitialized;

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
    private readonly IWorkoutProgramService _workoutProgramService;
    private readonly IWorkoutTrackingService _workoutTrackingService;
    private readonly IAlertService _alertService;
    private readonly ILocalizationResourceManager _localizationManager;

    public ActiveWorkoutPageViewModel(IWorkoutProgramService workoutProgramService,
        IWorkoutTrackingService workoutTrackingService,
        IAlertService alertService,
        ILocalizationResourceManager localizationManager)
    {
        _workoutProgramService = workoutProgramService;
        _workoutTrackingService = workoutTrackingService;
        _alertService = alertService;
        _localizationManager = localizationManager;
        
        WeakReferenceMessenger.Default.Register<AddExerciseMessage>(this, (r, m) =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                ((ActiveWorkoutPageViewModel)r).AddNewExercise(m.Value);
            });
        });
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (!_isInitialized && query.TryGetValue(nameof(ProgramId), out var id) && id is string idStr && Guid.TryParse(idStr, out var guid))
        {
            ProgramId = guid;
            InitializeCommand.Execute(null);
        }
    }

    [RelayCommand]
    private async Task Initialize()
    {
        if (_isInitialized) return;
        
        IsLoading = true;
        Error = null;
        try
        {
            var result = await _workoutProgramService.GetProgramAsync(ProgramId);
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
            _isInitialized = true;
        }
        catch (Exception)
        {
            var genericError = new LocalizedString(() => _localizationManager["UnexpectedErrorMessage"]);
            HandleError(genericError);
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void AddNewExercise(WorkoutExerciseDto exerciseDto)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            var defaultSet = new WorkoutSetDto(Guid.NewGuid(), 1, 10, TimeSpan.FromMinutes(1.5), 0, WeightUnit.Kg);
            
            var tempPlanDto = new WorkoutExercisePlanDto(Guid.NewGuid(), ProgramId, exerciseDto, [defaultSet]);

            var activeItem = new ActiveExerciseItem(
                tempPlanDto,
                OnSetCompleted,
                DeleteSetCommand,
                AskDeleteExerciseCommand,
                AddSetCommand);

            Exercises.Add(activeItem);
        });
    }

    private void StartTimer()
    {
        _totalWorkoutDuration = TimeSpan.Zero;
        _timer = Application.Current?.Dispatcher.CreateTimer();
        if (_timer == null) return;
        
        _timer.Interval = TimeSpan.FromSeconds(1);
        _timer.Tick += OnTimerTick;
        _timer.Start();
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
             _alertService.ShowToastAsync(_localizationManager["Error_MaxSetsReached"]);
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
            _alertService.ShowToastAsync(_localizationManager["Error_CannotDeleteLastSet"]);
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
        ConfirmationTitle = _localizationManager["Title_DeleteExercise"];
        ConfirmationMessage = _localizationManager["Msg_DeleteExerciseConfirm"];
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
            _alertService.ShowToastAsync(_localizationManager["Error_NoExercisesInWorkout"]);
            return;
        }

        if (Exercises.Any(e => e.Sets.Count == 0))
        {
            _alertService.ShowToastAsync(_localizationManager["Error_ExerciseWithoutSets"]);
            return;
        }

        var totalMinutes = _totalWorkoutDuration.TotalMinutes;
        
        if (totalMinutes is < 10 or > 300)
        {
            EntryPopupTitle = _localizationManager["Title_AdjustDuration"];
            EntryPopupPlaceholder = _localizationManager["Placeholder_EnterDurationMinutes"];
            
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
                _alertService.ShowToastAsync(_localizationManager["Error_DurationTooShort"]);
                return;
            case > 300:
                _alertService.ShowToastAsync(_localizationManager["Error_DurationTooLong"]);
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
        ConfirmationTitle = _localizationManager["Title_FinishWorkout"];
        ConfirmationMessage = _localizationManager["Msg_FinishWorkoutConfirm"];
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

            var logTask = _workoutTrackingService.LogEntryAsync(workoutEntry);
            var updateTask = _workoutProgramService.UpdateProgramAsync(ProgramId, programUpdateRequest);

            await Task.WhenAll(logTask, updateTask);

            string? errorMessage = null;
            
            if (!logTask.Result.Success) 
                errorMessage = logTask.Result.Message;
            else if (!updateTask.Result.Success) 
                errorMessage = updateTask.Result.Message;

            if (errorMessage != null)
            {
                 var errorMsg = new LocalizedString(() => errorMessage);
                 await _alertService.ShowToastAsync(errorMsg.Localized);
                 return;
            }
            
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception)
        {
             var genericError = new LocalizedString(() => _localizationManager["UnexpectedErrorMessage"]);
             await _alertService.ShowToastAsync(genericError.Localized);
             _timer?.Start();
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task AddExercise()
    {
        await Shell.Current.GoToAsync($"{nameof(ExerciseSearchPageView)}");
    }
    
    [RelayCommand] 
    private void AskCancelWorkout()
    {
        ConfirmationTitle = _localizationManager["Title_CancelWorkout"];
        ConfirmationMessage = _localizationManager["Msg_CancelWorkoutConfirm"];
        
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
            _alertService.ShowToastAsync(errorMsg.Localized);
        else
            Error = errorMsg;
    }
}

public partial class ActiveExerciseItem : ObservableObject
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
    
    [ObservableProperty] 
    private int _reps;

    partial void OnRepsChanged(int value)
    {
        if (value < 0) Reps = 0;
        else if (value > 100) Reps = 100;
    }

    [ObservableProperty] 
    private double _weight;

    partial void OnWeightChanged(double value)
    {
        if (value < 0) Weight = 0;
        else if (value > 1500) Weight = 1500;
    }

    [ObservableProperty] 
    private TimeSpan _restTime;

    partial void OnRestTimeChanged(TimeSpan value)
    {
        if (value.TotalMinutes >= 10) RestTime = TimeSpan.FromMinutes(9).Add(TimeSpan.FromSeconds(59));
    }

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
