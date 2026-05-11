using CommunityToolkit.Mvvm.ComponentModel;
using ForgeFit.MAUI.Services.Interfaces;
using ForgeFit.MAUI.ViewModels.Core;

namespace ForgeFit.MAUI.ViewModels.Workout.Dashboard;

public partial class WorkoutStatsViewModel(
    IWorkoutTrackingService workoutTrackingService,
    IGoalService goalService)
    : BaseViewModel
{
    [ObservableProperty] [NotifyPropertyChangedFor(nameof(WorkoutProgress))]
    private int _completedWorkouts;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(WorkoutProgress))]
    private int _targetWorkouts;

    public double WorkoutProgress => TargetWorkouts > 0
        ? (double)CompletedWorkouts / TargetWorkouts
        : 0;

    public async Task LoadGoalAsync(CancellationToken token = default)
    {
        try
        {
            var goalResult = await goalService.GetWorkoutGoal(token);
            if (token.IsCancellationRequested) return;

            if (goalResult is { Success: true, Data: not null })
                TargetWorkouts = goalResult.Data.WorkoutsPerWeek;
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception)
        {
            if (!token.IsCancellationRequested)
                throw;
        }
    }

    public async Task LoadEntriesAsync(CancellationToken token = default)
    {
        try
        {
            var today = DateTime.Today;
            var daysSinceMonday = ((int)today.DayOfWeek - 1 + 7) % 7;
            var monday = today.AddDays(-daysSinceMonday);
            var nextMonday = monday.AddDays(7);

            var statsResult = await workoutTrackingService.GetEntriesByDateRangeAsync(monday, nextMonday, token);
            if (token.IsCancellationRequested) return;

            if (statsResult is { Success: true, Data: not null })
                CompletedWorkouts = statsResult.Data.Count;
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception)
        {
            if (!token.IsCancellationRequested)
                throw;
        }
    }

    public void ResetStats()
    {
        CompletedWorkouts = 0;
        TargetWorkouts = 0;
    }
}
