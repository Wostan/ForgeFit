using ForgeFit.MAUI.ViewModels;

namespace ForgeFit.MAUI.Views;

public partial class DiaryPageView : ContentPage
{
    public DiaryPageView(DiaryPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
