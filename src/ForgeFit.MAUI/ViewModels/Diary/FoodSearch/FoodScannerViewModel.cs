using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ForgeFit.MAUI.Messages;
using ForgeFit.MAUI.Services.Interfaces;
using LocalizationResourceManager.Maui;

namespace ForgeFit.MAUI.ViewModels.Diary.FoodSearch;

public partial class FoodScannerViewModel(
    IFoodService foodService,
    IAlertService alertService,
    ILocalizationResourceManager localizationManager)
    : ObservableObject
{
    private bool _isProcessingBarcode;
    [ObservableProperty] private bool _isTorchOn;

    [RelayCommand]
    private async Task Close()
    {
        IsTorchOn = false;
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private void ToggleTorch()
    {
        IsTorchOn = !IsTorchOn;
    }

    [RelayCommand]
    private async Task BarcodeDetected(string barcode)
    {
        if (string.IsNullOrEmpty(barcode) || _isProcessingBarcode) return;

        _isProcessingBarcode = true;

        try
        {
            HapticFeedback.Perform();
        }
        catch
        {
        }

        try
        {
            var result = await foodService.GetProductByBarcodeAsync(barcode);

            if (result is { Success: true, Data: not null })
            {
                var p = result.Data;
                WeakReferenceMessenger.Default.Send(new BarcodeDetectedMessage(barcode, p));
            }
            else
            {
                await alertService.ShowToastAsync(new LocalizedString(() => result.Message).Localized);
            }

            await Shell.Current.GoToAsync("..");
        }
        catch
        {
            await alertService.ShowToastAsync(localizationManager["UnexpectedErrorMessage"]);
            await Shell.Current.GoToAsync("..");
        }
        finally
        {
            _isProcessingBarcode = false;
            IsTorchOn = false;
        }
    }

    public void ResetState()
    {
        IsTorchOn = false;
        _isProcessingBarcode = false;
    }
}
