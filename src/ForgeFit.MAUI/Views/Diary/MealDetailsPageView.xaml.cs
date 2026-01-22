using ForgeFit.MAUI.ViewModels;

namespace ForgeFit.MAUI.Views.Diary;

public partial class MealDetailsPageView : ContentPage
{
    public MealDetailsPageView(MealDetailsPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is MealDetailsPageViewModel vm) vm.LoadDataCommand.Execute(null);
    }
}
