using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForgeFit.MAUI.ViewModels;

namespace ForgeFit.MAUI.Views.Workout;

public partial class ExerciseSearchPageView : ContentPage
{
    public ExerciseSearchPageView(ExerciseSearchPageViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}

