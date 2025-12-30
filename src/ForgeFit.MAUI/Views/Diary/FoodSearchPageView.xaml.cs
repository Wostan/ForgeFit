using ForgeFit.MAUI.ViewModels;

namespace ForgeFit.MAUI.Views.Diary;

public partial class FoodSearchPageView : ContentPage
{
    public FoodSearchPageView(FoodSearchPageViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override bool OnBackButtonPressed()
    {
        if (BindingContext is not FoodSearchPageViewModel vm ||
            vm is { IsFoodDetailsVisible: false, IsScannerVisible: false }) return base.OnBackButtonPressed();

        vm.IsScannerVisible = false;
        vm.IsFoodDetailsVisible = false;
        return true;
    }
}
