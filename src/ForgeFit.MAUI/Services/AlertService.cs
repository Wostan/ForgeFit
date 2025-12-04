using ForgeFit.MAUI.Services.Interfaces;

namespace ForgeFit.MAUI.Services;

public class AlertService : IAlertService
{
    public async Task ShowErrorAsync(string message, string cancel)
    {
        await Shell.Current.DisplayAlert("Error", message, cancel);
    }

    public async Task ShowSuccessAsync(string message, string cancel)
    {
        await  Shell.Current.DisplayAlert("Success", message, cancel);
    }

    public async Task ShowInfoAsync(string title, string message, string cancel)
    {
        await Shell.Current.DisplayAlert("Info", message, cancel);
    }
}
