using ForgeFit.MAUI.ViewModels;

namespace ForgeFit.MAUI.Views.Workout;

public partial class WorkoutPageView : ContentPage
{
    public WorkoutPageView(WorkoutPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
    
    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is WorkoutPageViewModel vm) vm.InitializeCommand.Execute(null);
    }
}
