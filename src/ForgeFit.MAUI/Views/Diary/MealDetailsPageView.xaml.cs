using ForgeFit.MAUI.ViewModels;

namespace ForgeFit.MAUI.Views.Diary;

public partial class MealDetailsPageView : ContentPage
{
    public MealDetailsPageView(MealDetailsPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
