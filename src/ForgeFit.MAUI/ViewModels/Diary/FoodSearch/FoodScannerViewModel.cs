using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ForgeFit.MAUI.Models.DTOs.Food;
using ForgeFit.MAUI.Services.Interfaces;
using LocalizationResourceManager.Maui;

namespace ForgeFit.MAUI.ViewModels.Diary.FoodSearch;

public partial class FoodScannerViewModel : ObservableObject
{
    private readonly IFoodService _foodService;
    private readonly IAlertService _alertService;
    private readonly ILocalizationResourceManager _localizationManager;
    private readonly FoodSearchViewModel _searchVM;
    private readonly FoodDetailsViewModel _detailsVM;
    private readonly FoodDiaryIntegrationViewModel _diaryVM;

    private bool _isProcessingBarcode;
    [ObservableProperty] private bool _isScannerVisible;
    [ObservableProperty] private bool _isTorchOn;

    public FoodScannerViewModel(
        IFoodService foodService,
        IAlertService alertService,
        ILocalizationResourceManager localizationManager,
        FoodSearchViewModel searchVM,
        FoodDetailsViewModel detailsVM,
        FoodDiaryIntegrationViewModel diaryVM)
    {
        _foodService = foodService;
        _alertService = alertService;
        _localizationManager = localizationManager;
        _searchVM = searchVM;
        _detailsVM = detailsVM;
        _diaryVM = diaryVM;
    }

    [RelayCommand]
    private void ToggleScanner()
    {
        IsScannerVisible = !IsScannerVisible;
        if (IsScannerVisible)
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

        try
        {
            HapticFeedback.Perform();
        }
        catch
        {
            // ignored
        }

        try
        {
            var result = await _foodService.GetProductByBarcodeAsync(barcode);

            if (result is { Success: true, Data: not null })
            {
                var p = result.Data;
                var baseServing = p.Servings.FirstOrDefault();

                var itemVm = new FoodSearchItemViewModel(new FoodSearchResponse(
                    p.ExternalId, p.Label, p.BrandName,
                    baseServing?.Calories ?? 0, baseServing?.Carbs ?? 0,
                    baseServing?.Protein ?? 0, baseServing?.Fat ?? 0,
                    baseServing?.Fiber ?? 0, baseServing?.Sugar ?? 0,
                    baseServing?.SaturatedFat ?? 0, baseServing?.Sodium ?? 0,
                    $"{baseServing?.MetricAmount} {baseServing?.MetricUnit}")
                );

                if (_diaryVM.IsProductAdded(p.ExternalId)) itemVm.IsAdded = true;

                IsScannerVisible = false;
                _searchVM.ClearSearchResults();
                _searchVM.AddSearchResult(itemVm);
                await _detailsVM.OpenFoodDetailsInternal(p, itemVm.Data, false);
            }
            else
            {
                IsScannerVisible = false;
                await _alertService.ShowToastAsync(new LocalizedString(() => result.Message).Localized);
            }
        }
        catch
        {
            IsScannerVisible = false;
            await _alertService.ShowToastAsync(_localizationManager["UnexpectedErrorMessage"]);
        }
        finally
        {
            _isProcessingBarcode = false;
        }
    }

    public void ResetState()
    {
        IsScannerVisible = false;
        IsTorchOn = false;
        _isProcessingBarcode = false;
    }
}