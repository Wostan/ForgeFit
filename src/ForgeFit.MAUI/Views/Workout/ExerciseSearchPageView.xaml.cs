using ExerciseSearchPageViewModel = ForgeFit.MAUI.ViewModels.Workout.ExerciseSearch.ExerciseSearchPageViewModel;

namespace ForgeFit.MAUI.Views.Workout;

public partial class ExerciseSearchPageView : ContentPage
{
    public ExerciseSearchPageView(ExerciseSearchPageViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
