using ProfilePageViewModel = ForgeFit.MAUI.ViewModels.Profile.Main.ProfilePageViewModel;

namespace ForgeFit.MAUI.Views.Profile;

public partial class ProfilePageView : ContentPage
{
    public ProfilePageView(ProfilePageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
