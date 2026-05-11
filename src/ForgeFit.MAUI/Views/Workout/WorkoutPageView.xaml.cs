using WorkoutPageViewModel = ForgeFit.MAUI.ViewModels.Workout.Dashboard.WorkoutPageViewModel;

namespace ForgeFit.MAUI.Views.Workout;

public partial class WorkoutPageView : ContentPage
{
    public WorkoutPageView(WorkoutPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is WorkoutPageViewModel vm) await vm.CheckAndRefreshAsync();
    }
}
