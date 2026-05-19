using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ForgeFit.MAUI.Constants;
using ForgeFit.MAUI.Models.DTOs.Food;
using ForgeFit.MAUI.Services.Interfaces;
using ForgeFit.MAUI.ViewModels.Core;
using LocalizationResourceManager.Maui;

namespace ForgeFit.MAUI.ViewModels.Diary.MyProducts;

public partial class CreateCustomFoodViewModel(
    PopupManagerViewModel popupManager,
    ICustomFoodService customFoodService,
    IAlertService alertService,
    ILocalizationResourceManager localizationManager) : ObservableObject
{
    [ObservableProperty] private string? _barcode;
    [ObservableProperty] private string? _brand;
    [ObservableProperty] private double _calories;
    [ObservableProperty] private double _carbs;

    private Guid? _editingFoodId;
    [ObservableProperty] private double _fat;
    [ObservableProperty] private double _fiber;
    [ObservableProperty] private string _name = string.Empty;
    [ObservableProperty] private double _protein;
    [ObservableProperty] private double _saturatedFat;
    [ObservableProperty] private double _servingSize = 100;
    [ObservableProperty] private string _servingUnit = "g";
    [ObservableProperty] private double _sodium;
    [ObservableProperty] private double _sugar;

    public Func<CustomFoodDto, Task>? FoodCreatedCallback;
    public Func<CustomFoodDto, Task>? FoodUpdatedCallback;

    public ObservableCollection<string> ServingUnits { get; } = ["g", "ml"];

    public void InitializeForCreate()
    {
        Name = string.Empty;
        Brand = null;
        Barcode = null;
        Calories = 0;
        Carbs = 0;
        Protein = 0;
        Fat = 0;
        Fiber = 0;
        Sugar = 0;
        SaturatedFat = 0;
        Sodium = 0;
        ServingSize = 100;
        _editingFoodId = null;
    }

    public void InitializeForEdit(CustomFoodDto food)
    {
        Name = food.Name;
        Brand = food.Brand;
        Barcode = food.Barcode;
        Calories = food.Calories;
        Carbs = food.Carbs;
        Protein = food.Protein;
        Fat = food.Fat;
        Fiber = food.Fiber;
        Sugar = food.Sugar;
        SaturatedFat = food.SaturatedFat;
        Sodium = food.Sodium;
        ServingSize = food.ServingSize;
        ServingUnit = food.ServingUnit;
        _editingFoodId = food.Id;
    }

    [RelayCommand]
    private void Close()
    {
        popupManager.CloseCreateFoodPopup();
    }

    [RelayCommand]
    private async Task Save()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                await alertService.ShowToastAsync(localizationManager["Error_FoodNameRequired"]);
                return;
            }

            if (Name.Length > AppConstants.ValidationLimits.MaxFoodLabelLength)
            {
                await alertService.ShowToastAsync(localizationManager["Error_FoodNameTooLong"]);
                return;
            }

            if (ServingSize is <= 0 or > AppConstants.ValidationLimits.MaxFoodAmount)
            {
                await alertService.ShowToastAsync(localizationManager["Error_InvalidServingSize"]);
                return;
            }

            if (_editingFoodId.HasValue)
            {
                var updateRequest = new CustomFoodUpdateRequest(
                    Name,
                    Brand,
                    Barcode,
                    Calories,
                    Carbs,
                    Protein,
                    Fat,
                    Fiber,
                    Sugar,
                    SaturatedFat,
                    Sodium,
                    ServingSize,
                    ServingUnit
                );

                var updateResult = await customFoodService.UpdateAsync(_editingFoodId.Value, updateRequest);

                if (updateResult is { Success: true, Data: not null })
                {
                    popupManager.CloseCreateFoodPopup();
                    await alertService.ShowToastAsync(localizationManager["Success_CustomFoodUpdated"]);

                    if (FoodUpdatedCallback != null) await FoodUpdatedCallback(updateResult.Data);
                }
                else
                {
                    await alertService.ShowToastAsync(updateResult.Message);
                }
            }
            else
            {
                var createRequest = new CustomFoodCreateRequest(
                    Name,
                    Brand,
                    Barcode,
                    Calories,
                    Carbs,
                    Protein,
                    Fat,
                    Fiber,
                    Sugar,
                    SaturatedFat,
                    Sodium,
                    ServingSize,
                    ServingUnit
                );

                var createResult = await customFoodService.CreateAsync(createRequest);

                if (createResult.Success)
                {
                    popupManager.CloseCreateFoodPopup();
                    await alertService.ShowToastAsync(localizationManager["Success_CustomFoodCreated"]);

                    if (FoodCreatedCallback != null && createResult.Data != null)
                        await FoodCreatedCallback(createResult.Data);
                }
                else
                {
                    await alertService.ShowToastAsync(createResult.Message);
                }
            }
        }
        catch
        {
            await alertService.ShowToastAsync(localizationManager["UnexpectedErrorMessage"]);
        }
    }
}
