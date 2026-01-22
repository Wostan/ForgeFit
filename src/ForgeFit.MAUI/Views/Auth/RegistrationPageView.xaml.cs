using ForgeFit.MAUI.ViewModels;

namespace ForgeFit.MAUI.Views.Auth;

public partial class RegistrationPageView : ContentPage
{
    public RegistrationPageView(RegistrationPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
