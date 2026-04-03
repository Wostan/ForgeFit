using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ForgeFit.MAUI.Messages;
using ForgeFit.MAUI.Models.DTOs.Workout;
using ForgeFit.MAUI.Models.Enums.ProfileEnums;
using ForgeFit.MAUI.Services.Interfaces;
using LocalizationResourceManager.Maui;

namespace ForgeFit.MAUI.ViewModels.Workout.ActiveSession;

public partial class ExerciseSessionViewModel : ObservableObject
{
    private readonly IAlertService _alertService;
    private readonly ILocalizationResourceManager _localizationManager;
    private readonly Action<ActiveSetItem> _onSetCompleted;
    private readonly Guid _programId;

    [ObservableProperty] private ObservableCollection<ActiveExerciseItem> _exercises = [];

    public ExerciseSessionViewModel(
        IAlertService alertService,
        ILocalizationResourceManager localizationManager,
        Guid programId,
        Action<ActiveSetItem> onSetCompleted)
    {
        _alertService = alertService;
        _localizationManager = localizationManager;
        _programId = programId;
        _onSetCompleted = onSetCompleted;

        WeakReferenceMessenger.Default.Register<AddExerciseMessage>(this,
            (r, m) =>
            {
                MainThread.BeginInvokeOnMainThread(() => { ((ExerciseSessionViewModel)r).AddNewExercise(m.Value); });
            });
    }

    public void InitializeExercises(IEnumerable<WorkoutExercisePlanDto> exercisePlans)
    {
        var mappedExercises = exercisePlans
            .Select(plan => new ActiveExerciseItem(
                plan,
                _onSetCompleted,
                DeleteSetCommand,
                AskDeleteExerciseCommand,
                AddSetCommand))
            .ToList();

        Exercises = new ObservableCollection<ActiveExerciseItem>(mappedExercises);
    }

    private void AddNewExercise(WorkoutExerciseDto exerciseDto)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            var defaultSet = new WorkoutSetDto(Guid.NewGuid(), 1, 10, TimeSpan.FromMinutes(1.5), 0, WeightUnit.Kg);

            var tempPlanDto = new WorkoutExercisePlanDto(Guid.NewGuid(), _programId, exerciseDto, [defaultSet]);

            var activeItem = new ActiveExerciseItem(
                tempPlanDto,
                _onSetCompleted,
                DeleteSetCommand,
                AskDeleteExerciseCommand,
                AddSetCommand);

            Exercises.Add(activeItem);
        });
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
            _onSetCompleted,
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

        for (var i = 0; i < parentExercise.Sets.Count; i++) parentExercise.Sets[i].Order = i + 1;
    }

    [RelayCommand]
    private void AskDeleteExercise(ActiveExerciseItem exercise)
    {
        DeleteExerciseRequested?.Invoke(exercise);
    }

    public double CalculateTotalVolume()
    {
        return Exercises.Sum(e => e.Sets.Where(s => s.IsCompleted).Sum(s => s.Weight * s.Reps));
    }

    public double CalculateTotalReps()
    {
        return Exercises.Sum(e => e.Sets.Where(s => s.IsCompleted).Sum(s => s.Reps));
    }

    public bool HasExercises()
    {
        return Exercises.Any();
    }

    public bool HasExercisesWithoutSets()
    {
        return Exercises.Any(e => e.Sets.Count == 0);
    }

    public List<PerformedExerciseDto> GetPerformedExercises()
    {
        return Exercises.Select<ActiveExerciseItem, PerformedExerciseDto>(e => e.ToPerformedExerciseDto()).ToList();
    }

    public List<WorkoutExercisePlanDto> GetUpdatedPlans()
    {
        return Exercises.Select<ActiveExerciseItem, WorkoutExercisePlanDto>(e => new WorkoutExercisePlanDto(
            e.Id,
            _programId,
            e.WorkoutExercise,
            e.Sets.Select<ActiveSetItem, WorkoutSetDto>(s => s.ToDto()).ToList()
        )).ToList();
    }

    public event Action<ActiveExerciseItem>? DeleteExerciseRequested;
}