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

public partial class WorkoutProgramEditorPageViewModel : BaseViewModel, IQueryAttributable
{
    private readonly IWorkoutProgramService _workoutProgramService;
    private readonly IAlertService _alertService;
    private readonly ILocalizationResourceManager _localizationManager;

    private bool _isInitialized;
    private Func<Task>? _pendingConfirmationAction;

    [ObservableProperty] private Guid _programId;
    
    [ObservableProperty] private string _programName = string.Empty;
    [ObservableProperty] private string? _programDescription;
    [ObservableProperty] private ObservableCollection<EditorExerciseItem> _exercises = [];

    [ObservableProperty] private bool _isRenamePopupVisible;
    [ObservableProperty] private string _tempProgramName = string.Empty;

    [ObservableProperty] private bool _isConfirmationPopupVisible;
    [ObservableProperty] private string _confirmationTitle = string.Empty;
    [ObservableProperty] private string _confirmationMessage = string.Empty;

    public WorkoutProgramEditorPageViewModel(
        IWorkoutProgramService workoutProgramService,
        IAlertService alertService,
        ILocalizationResourceManager localizationManager)
    {
        _workoutProgramService = workoutProgramService;
        _alertService = alertService;
        _localizationManager = localizationManager;

        WeakReferenceMessenger.Default.Register<AddExerciseMessage>(this, (r, m) =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                ((WorkoutProgramEditorPageViewModel)r).AddNewExercise(m.Value);
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
                HandleError(new LocalizedString(() => result.Message));
                return;
            }

            ProgramName = result.Data.Name;
            ProgramDescription = result.Data.Description;

            var mappedExercises = result.Data.WorkoutExercisePlans
                .Select(plan => new EditorExerciseItem(
                    plan,
                    DeleteSetCommand,
                    AskDeleteExerciseCommand,
                    AddSetCommand))
                .ToList();

            Exercises = new ObservableCollection<EditorExerciseItem>(mappedExercises);
            _isInitialized = true;
        }
        catch (Exception)
        {
            HandleError(new LocalizedString(() => _localizationManager["UnexpectedErrorMessage"]));
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void OpenRenamePopup()
    {
        TempProgramName = ProgramName;
        IsRenamePopupVisible = true;
    }

    [RelayCommand]
    private async Task ConfirmRename()
    {
        if (string.IsNullOrWhiteSpace(TempProgramName))
        {
            return;
        }
        
        if (TempProgramName.Length > 50)
        {
            await _alertService.ShowToastAsync(_localizationManager["Error_ProgramNameTooLong"]);
            return;
        }
        
        ProgramName = TempProgramName;
        IsRenamePopupVisible = false;
    }

    [RelayCommand]
    private void CloseRenamePopup()
    {
        IsRenamePopupVisible = false;
    }

    private void AddNewExercise(WorkoutExerciseDto exerciseDto)
    {
        var defaultSet = new WorkoutSetDto(Guid.NewGuid(), 1, 10, TimeSpan.FromMinutes(1.5), 0, WeightUnit.Kg);
        var tempPlanDto = new WorkoutExercisePlanDto(Guid.NewGuid(), ProgramId, exerciseDto, [defaultSet]);

        var item = new EditorExerciseItem(
            tempPlanDto,
            DeleteSetCommand,
            AskDeleteExerciseCommand,
            AddSetCommand);

        Exercises.Add(item);
    }

    [RelayCommand]
    private async Task AddExercise()
    {
        await Shell.Current.GoToAsync($"{nameof(ExerciseSearchPageView)}?ProgramName={ProgramName}");
    }

    [RelayCommand]
    private void AddSet(EditorExerciseItem exercise)
    {
        if (exercise.Sets.Count >= 20)
        {
            _alertService.ShowToastAsync(_localizationManager["Error_MaxSetsReached"]);
            return;
        }

        var lastSet = exercise.Sets.LastOrDefault();
        var newOrder = (lastSet?.Order ?? 0) + 1;

        var newSet = new EditorSetItem(new WorkoutSetDto(
            Guid.NewGuid(),
            newOrder,
            lastSet?.Reps ?? 10,
            lastSet?.RestTime ?? TimeSpan.FromMinutes(1.5),
            lastSet?.Weight ?? 0,
            lastSet?.WeightUnit ?? WeightUnit.Kg),
            DeleteSetCommand);

        exercise.Sets.Add(newSet);
    }

    [RelayCommand]
    private void DeleteSet(EditorSetItem set)
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
    private void AskDeleteExercise(EditorExerciseItem exercise)
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
    private async Task SaveProgram()
    {
        if (string.IsNullOrWhiteSpace(ProgramName))
        {
            await _alertService.ShowToastAsync(_localizationManager["Error_NameRequired"]);
            return;
        }
        
        if (ProgramName.Length > 50)
        {
            await _alertService.ShowToastAsync(_localizationManager["Error_ProgramNameTooLong"]);
            return;
        }
        
        if (ProgramDescription?.Length > 300)
        {
            await _alertService.ShowToastAsync(_localizationManager["Error_ProgramDescriptionTooLong"]);
            return;
        }

        if (Exercises.Count > 50)
        {
            await _alertService.ShowToastAsync(_localizationManager["Error_TooManyExercises"]);
            return;
        }

        IsLoading = true;
        try
        {
            var updatedPlans = Exercises.Select(e => new WorkoutExercisePlanDto(
                e.Id,
                ProgramId,
                e.WorkoutExercise,
                e.Sets.Select(s => s.ToDto()).ToList()
            )).ToList();

            var request = new WorkoutProgramRequest(ProgramName, ProgramDescription, updatedPlans);
            var result = await _workoutProgramService.UpdateProgramAsync(ProgramId, request);

            if (result.Success)
            {
                WeakReferenceMessenger.Default.Send(new WorkoutDataUpdatedMessage());
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                await _alertService.ShowToastAsync(result.Message);
            }
        }
        catch (Exception)
        {
            await _alertService.ShowToastAsync(_localizationManager["UnexpectedErrorMessage"]);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void AskCancel()
    {
        ConfirmationTitle = _localizationManager["Title_UnsavedChanges"];
        ConfirmationMessage = _localizationManager["Msg_UnsavedChangesConfirm"];
        _pendingConfirmationAction = async () =>
        {
            await Shell.Current.GoToAsync("..");
        };
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

    private void HandleError(LocalizedString errorMsg)
    {
        Error = errorMsg;
    }
}

public partial class EditorExerciseItem : ObservableObject
{
    public Guid Id { get; }
    public WorkoutExerciseDto WorkoutExercise { get; }
    public ObservableCollection<EditorSetItem> Sets { get; }

    public IRelayCommand DeleteExerciseCommand { get; }
    public IRelayCommand AddSetCommand { get; }

    public EditorExerciseItem(
        WorkoutExercisePlanDto dto,
        IRelayCommand deleteSetCommand,
        IRelayCommand deleteExerciseCommand,
        IRelayCommand addSetCommand)
    {
        Id = dto.Id;
        WorkoutExercise = dto.WorkoutExercise;
        DeleteExerciseCommand = deleteExerciseCommand;
        AddSetCommand = addSetCommand;

        Sets = new ObservableCollection<EditorSetItem>(
            dto.WorkoutSets.OrderBy(s => s.Order)
                           .Select(s => new EditorSetItem(s, deleteSetCommand))
        );
    }
}

public partial class EditorSetItem : ObservableObject
{
    public Guid Id { get; }

    [ObservableProperty] private int _order;

    [ObservableProperty] private int _reps;
    partial void OnRepsChanged(int value) => Reps = Math.Clamp(value, 0, 100);

    [ObservableProperty] private double _weight;
    partial void OnWeightChanged(double value) => Weight = Math.Clamp(value, 0, 1500);

    [ObservableProperty] private TimeSpan _restTime;

    [ObservableProperty] private WeightUnit _weightUnit;

    public IRelayCommand DeleteCommand { get; }

    public EditorSetItem(WorkoutSetDto dto, IRelayCommand deleteCommand)
    {
        Id = dto.Id;
        Order = dto.Order;
        Reps = dto.Reps;
        Weight = dto.Weight;
        RestTime = dto.RestTime;
        WeightUnit = dto.WeightUnit;
        DeleteCommand = deleteCommand;
    }

    public WorkoutSetDto ToDto() => new(Id, Order, Reps, RestTime, Weight, WeightUnit);
}
