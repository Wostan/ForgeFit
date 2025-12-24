using ForgeFit.MAUI.ViewModels;

namespace ForgeFit.MAUI.Views.Diary;

public partial class FoodSearchPageView : ContentPage
{
    public FoodSearchPageView(FoodSearchPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
