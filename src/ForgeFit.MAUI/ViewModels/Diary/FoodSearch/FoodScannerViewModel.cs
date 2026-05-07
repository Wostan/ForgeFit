using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ForgeFit.MAUI.Constants;
using ForgeFit.MAUI.Models.DTOs.Food;
using ForgeFit.MAUI.Services.Interfaces;
using ForgeFit.MAUI.ViewModels.Core;
using ForgeFit.MAUI.ViewModels.Diary.FoodSearch;
using LocalizationResourceManager.Maui;

namespace ForgeFit.MAUI.ViewModels.Diary.FoodSearch;

public partial class FoodScannerViewModel(
    IFoodService foodService,
    IAlertService alertService,
    ILocalizationResourceManager localizationManager,
    FoodSearchViewModel searchVM,
    FoodDetailsViewModel detailsVM,
    FoodDiaryIntegrationViewModel diaryVM)
    : ObservableObject
{
    private bool _isProcessingBarcode;
    [ObservableProperty] private bool _isScannerVisible;
    [ObservableProperty] private bool _isTorchOn;

    [RelayCommand]
    private void ToggleScanner()
    {
        if (IsScannerVisible)
        {
            IsTorchOn = false;
            IsScannerVisible = false;
        }
        else
        {
            IsScannerVisible = true;
        }

        _isProcessingBarcode = false;
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

        try { HapticFeedback.Perform(); } catch { }

        try
        {
            var result = await foodService.GetProductByBarcodeAsync(barcode);

            if (result is { Success: true, Data: not null })
            {
                var p = result.Data;
                var baseServing = p.Servings.FirstOrDefault();

                var searchResponse = new FoodSearchResponse(
                    p.ExternalId, p.Label, p.BrandName,
                    baseServing?.Calories ?? 0, baseServing?.Carbs ?? 0,
                    baseServing?.Protein ?? 0, baseServing?.Fat ?? 0,
                    baseServing?.Fiber ?? 0, baseServing?.Sugar ?? 0,
                    baseServing?.SaturatedFat ?? 0, baseServing?.Sodium ?? 0,
                    $"{baseServing?.MetricAmount} {baseServing?.MetricUnit}");

                IsScannerVisible = false;
                
                var itemVm = new FoodSearchItemViewModel(searchResponse);
                if (diaryVM.IsProductAdded(p.ExternalId)) itemVm.IsAdded = true;

                searchVM.ClearSearchResults();
                searchVM.AddSearchResult(itemVm);
                await detailsVM.OpenFoodDetailsInternal(p, itemVm.Data, false);
            }
            else
            {
                IsScannerVisible = false;
                await alertService.ShowToastAsync(new LocalizedString(() => result.Message).Localized);
            }
        }
        catch
        {
            IsScannerVisible = false;
            await alertService.ShowToastAsync(localizationManager["UnexpectedErrorMessage"]);
        }
        finally
        {
            _isProcessingBarcode = false;
            IsTorchOn = false;
        }
    }

    public void ResetState()
    {
        IsScannerVisible = false;
        IsTorchOn = false;
        _isProcessingBarcode = false;
    }
}