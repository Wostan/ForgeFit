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
        if (BindingContext is not FoodSearchPageViewModel vm)
            return base.OnBackButtonPressed();

        if (vm is { IsFoodDetailsVisible: true } or { IsScannerVisible: true })
        {
            vm.IsScannerVisible = false;
            vm.IsFoodDetailsVisible = false;
            return true;
        }

        if (!vm.BackCommand.CanExecute(null)) return base.OnBackButtonPressed();
        
        vm.BackCommand.Execute(null);
        return true;

    }
}
