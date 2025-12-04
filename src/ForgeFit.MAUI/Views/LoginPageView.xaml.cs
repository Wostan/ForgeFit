using ForgeFit.MAUI.ViewModels;

namespace ForgeFit.MAUI.Views;

public partial class LoginPageView : ContentPage
{
    public LoginPageView(LoginPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
