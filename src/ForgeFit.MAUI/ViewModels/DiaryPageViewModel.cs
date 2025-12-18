using CommunityToolkit.Mvvm.ComponentModel;

namespace ForgeFit.MAUI.ViewModels;

public partial class DiaryPageViewModel : ObservableObject
{
    [ObservableProperty]
    private string _test = "Test";
}
