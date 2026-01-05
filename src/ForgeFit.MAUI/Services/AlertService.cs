using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using ForgeFit.MAUI.Services.Interfaces;

namespace ForgeFit.MAUI.Services;

public class AlertService : IAlertService
{
    public async Task ShowToastAsync(string message)
    {
        var toast = Toast.Make(message, ToastDuration.Long);
        await toast.Show();
    }
}
