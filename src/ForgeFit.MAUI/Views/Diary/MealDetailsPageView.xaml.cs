using ForgeFit.MAUI.ViewModels;
using MealDetailsPageViewModel = ForgeFit.MAUI.ViewModels.Diary.Meals.MealDetailsPageViewModel;

namespace ForgeFit.MAUI.Views.Diary;

public partial class MealDetailsPageView : ContentPage
{
    public MealDetailsPageView(MealDetailsPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
