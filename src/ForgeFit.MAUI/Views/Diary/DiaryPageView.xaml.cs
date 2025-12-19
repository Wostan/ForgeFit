using ForgeFit.MAUI.ViewModels;

namespace ForgeFit.MAUI.Views.Diary;

public partial class DiaryPageView : ContentPage
{
    public DiaryPageView(DiaryPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
