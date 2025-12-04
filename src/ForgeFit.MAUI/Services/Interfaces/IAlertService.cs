namespace ForgeFit.MAUI.Services.Interfaces;

public interface IAlertService
{
    Task ShowErrorAsync(string message, string cancel);
    Task ShowSuccessAsync(string message, string cancel);
    Task ShowInfoAsync(string title, string message, string cancel);
}
