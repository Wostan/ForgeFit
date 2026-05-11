using RegistrationPageViewModel = ForgeFit.MAUI.ViewModels.Registration.RegistrationPageViewModel;

namespace ForgeFit.MAUI.Views.Auth;

public partial class RegistrationPageView : ContentPage
{
    public RegistrationPageView(RegistrationPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
