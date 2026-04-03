using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForgeFit.MAUI.ViewModels;
using ActiveWorkoutPageViewModel = ForgeFit.MAUI.ViewModels.Workout.ActiveSession.ActiveWorkoutPageViewModel;

namespace ForgeFit.MAUI.Views.Workout;

public partial class ActiveWorkoutPageView : ContentPage
{
    public ActiveWorkoutPageView(ActiveWorkoutPageViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
