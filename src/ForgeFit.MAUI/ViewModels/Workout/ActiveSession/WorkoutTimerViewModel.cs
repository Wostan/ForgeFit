using CommunityToolkit.Mvvm.ComponentModel;

namespace ForgeFit.MAUI.ViewModels.Workout.ActiveSession;

public partial class WorkoutTimerViewModel : ObservableObject
{
    private TimeSpan _currentRestTime;

    [ObservableProperty] private string _headerTitle = "00:00:00";
    private bool _isResting;
    private IDispatcherTimer? _timer;
    private TimeSpan _totalWorkoutDuration;

    public TimeSpan TotalWorkoutDuration => _totalWorkoutDuration;

    public void StartTimer()
    {
        _totalWorkoutDuration = TimeSpan.Zero;
        _timer = Application.Current?.Dispatcher.CreateTimer();
        if (_timer == null) return;

        _timer.Interval = TimeSpan.FromSeconds(1);
        _timer.Tick += OnTimerTick;
        _timer.Start();
    }

    public void StopTimer()
    {
        if (_timer == null) return;

        _timer.Stop();
        _timer.Tick -= OnTimerTick;
        _timer = null;
    }

    public void SetRestTime(TimeSpan restTime)
    {
        _currentRestTime = restTime;
        _isResting = true;
        HeaderTitle = _currentRestTime.ToString(@"mm\:ss");
    }

    public void ClearRestTime()
    {
        _isResting = false;
        HeaderTitle = _totalWorkoutDuration.ToString(@"hh\:mm\:ss");
    }

    public void SetTotalDuration(TimeSpan duration)
    {
        _totalWorkoutDuration = duration;
        if (!_isResting)
            HeaderTitle = _totalWorkoutDuration.ToString(@"hh\:mm\:ss");
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

        if (!_isResting) HeaderTitle = _totalWorkoutDuration.ToString(@"hh\:mm\:ss");
    }
}