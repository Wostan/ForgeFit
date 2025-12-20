using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ForgeFit.MAUI.Views.Diary;

namespace ForgeFit.MAUI.ViewModels;

public partial class DiaryPageViewModel : ObservableObject
{
    [RelayCommand]
    private async Task GoToMealDetails()
    {
        await Shell.Current.GoToAsync(nameof(MealDetailsPageView), false);
    }
}
