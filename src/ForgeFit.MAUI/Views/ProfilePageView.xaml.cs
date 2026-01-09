using ForgeFit.MAUI.ViewModels;

namespace ForgeFit.MAUI.Views;

public partial class ProfilePageView : ContentPage
{
    public ProfilePageView(ProfilePageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
