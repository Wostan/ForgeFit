using CommunityToolkit.Mvvm.ComponentModel;
using ForgeFit.MAUI.Services.Interfaces;

namespace ForgeFit.MAUI.ViewModels.Workout.Dashboard;

public partial class WorkoutStatsViewModel(
    IWorkoutTrackingService workoutTrackingService,
    IGoalService goalService)
    : Core.BaseViewModel
{
    [ObservableProperty] [NotifyPropertyChangedFor(nameof(WorkoutProgress))]
    private int _completedWorkouts;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(WorkoutProgress))]
    private int _targetWorkouts;

    public double WorkoutProgress => TargetWorkouts > 0
        ? (double)CompletedWorkouts / TargetWorkouts
        : 0;

    public async Task LoadStatsAsync(CancellationToken token = default)
    {
        try
        {
            var today = DateTime.Today;
            var daysSinceMonday = ((int)today.DayOfWeek - 1 + 7) % 7;
            var monday = today.AddDays(-daysSinceMonday);
            var nextMonday = monday.AddDays(7);

            var goalTask = goalService.GetWorkoutGoal(token);
            var statsTask = workoutTrackingService.GetEntriesByDateRangeAsync(monday, nextMonday, token);

            await Task.WhenAll(goalTask, statsTask);

            if (token.IsCancellationRequested) return;

            if (goalTask.Result is { Success: true, Data: not null })
                TargetWorkouts = goalTask.Result.Data.WorkoutsPerWeek;

            if (statsTask.Result is { Success: true, Data: not null })
                CompletedWorkouts = statsTask.Result.Data.Count;
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