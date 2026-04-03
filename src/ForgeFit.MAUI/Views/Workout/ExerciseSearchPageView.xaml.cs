using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForgeFit.MAUI.ViewModels;
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
