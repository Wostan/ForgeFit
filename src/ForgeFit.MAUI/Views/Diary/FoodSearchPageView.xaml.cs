using ForgeFit.MAUI.ViewModels;
using FoodSearchPageViewModel = ForgeFit.MAUI.ViewModels.Diary.FoodSearch.FoodSearchPageViewModel;

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

        if (vm.DetailsVM.IsFoodDetailsVisible || vm.ScannerVM.IsScannerVisible)
        {
            vm.ScannerVM.IsScannerVisible = false;
            vm.DetailsVM.IsFoodDetailsVisible = false;
            return true;
        }

        if (!vm.BackCommand.CanExecute(null)) return base.OnBackButtonPressed();

        vm.BackCommand.Execute(null);
        return true;
    }
}
