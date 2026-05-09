using ForgeFit.MAUI.ViewModels.Diary.AddFood;

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

        if (vm.PopupVM.IsConfirmationPopupVisible)
        {
            vm.PopupVM.CloseConfirmationPopupCommand.Execute(null);
            return true;
        }

        if (vm.DetailsVM.IsFoodDetailsVisible)
        {
            vm.DetailsVM.IsFoodDetailsVisible = false;
            return true;
        }

        if (vm.PopupVM.IsRecipeIngredientSearchPopupVisible)
        {
            vm.PopupVM.CloseRecipeIngredientSearchPopupCommand.Execute(null);
            return true;
        }

        if (vm.PopupVM.IsCreateRecipePopupVisible)
        {
            vm.PopupVM.CloseCreateRecipePopupCommand.Execute(null);
            return true;
        }

        if (vm.PopupVM.IsCreateFoodPopupVisible)
        {
            vm.PopupVM.CloseCreateFoodPopupCommand.Execute(null);
            return true;
        }

        if (vm.BackCommand.CanExecute(null))
        {
            vm.BackCommand.Execute(null);
            return true;
        }

        return base.OnBackButtonPressed();
    }
}