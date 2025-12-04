using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

