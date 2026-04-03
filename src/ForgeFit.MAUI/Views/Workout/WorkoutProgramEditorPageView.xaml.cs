using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForgeFit.MAUI.ViewModels;
using WorkoutProgramEditorPageViewModel = ForgeFit.MAUI.ViewModels.Workout.ProgramEditor.WorkoutProgramEditorPageViewModel;

namespace ForgeFit.MAUI.Views.Workout;

public partial class WorkoutProgramEditorPageView : ContentPage
{
    public WorkoutProgramEditorPageView(WorkoutProgramEditorPageViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
