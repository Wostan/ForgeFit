using ForgeFit.MAUI.ViewModels;
using DiaryPageViewModel = ForgeFit.MAUI.ViewModels.Diary.Main.DiaryPageViewModel;

namespace ForgeFit.MAUI.Views.Diary;

public partial class DiaryPageView : ContentPage
{
    public DiaryPageView(DiaryPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is DiaryPageViewModel vm) await vm.CheckAndRefreshAsync();
    }
}
