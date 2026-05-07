using ForgeFit.MAUI.ViewModels.Diary.Meals;

namespace ForgeFit.MAUI.Views.Diary;

public partial class MealDetailsPageView : ContentPage
{
    public MealDetailsPageView(MealDetailsPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override bool OnBackButtonPressed()
    {
        if (BindingContext is not MealDetailsPageViewModel vm)
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

        return base.OnBackButtonPressed();
    }
}