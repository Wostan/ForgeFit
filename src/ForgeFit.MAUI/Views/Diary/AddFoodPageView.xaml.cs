using AddFoodPageViewModel = ForgeFit.MAUI.ViewModels.Diary.AddFood.AddFoodPageViewModel;

namespace ForgeFit.MAUI.Views.Diary;

public partial class AddFoodPageView : ContentPage
{
    public AddFoodPageView(AddFoodPageViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override bool OnBackButtonPressed()
    {
        if (BindingContext is not AddFoodPageViewModel vm)
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
