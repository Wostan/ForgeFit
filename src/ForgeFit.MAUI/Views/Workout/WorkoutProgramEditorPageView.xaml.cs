using WorkoutProgramEditorPageViewModel =
    ForgeFit.MAUI.ViewModels.Workout.ProgramEditor.WorkoutProgramEditorPageViewModel;

namespace ForgeFit.MAUI.Views.Workout;

public partial class WorkoutProgramEditorPageView : ContentPage
{
    public WorkoutProgramEditorPageView(WorkoutProgramEditorPageViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
