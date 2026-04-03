using CommunityToolkit.Mvvm.ComponentModel;
using ForgeFit.MAUI.Services.Interfaces;
using ForgeFit.MAUI.ViewModels.Core;
using LocalizationResourceManager.Maui;

namespace ForgeFit.MAUI.ViewModels.Registration;

public partial class MeasurementsViewModel : BaseViewModel
{
    private readonly IBmiService _bmiService;
    private readonly ILocalizationResourceManager _localizationManager;
    [ObservableProperty] private LocalizedString? _bmiCategoryText;
    [ObservableProperty] private Color _bmiColor = Colors.Gray;
    [ObservableProperty] private LocalizedString? _bmiDescription;
    [ObservableProperty] private double _bmiValue;

    [ObservableProperty] private double _height = 175;

    [ObservableProperty] private LocalizedString? _validationError;
    [ObservableProperty] private double _weight = 75;

    public MeasurementsViewModel(IBmiService bmiService, ILocalizationResourceManager localizationManager)
    {
        _bmiService = bmiService;
        _localizationManager = localizationManager;
        RecalculateBmi();
    }

    partial void OnHeightChanged(double value)
    {
        var rounded = Math.Round(value, 0, MidpointRounding.AwayFromZero);

        if (Math.Abs(Height - rounded) > 0.01) Height = rounded;

        RecalculateBmi();
        TargetLimitsChanged?.Invoke();
    }

    partial void OnWeightChanged(double value)
    {
        var rounded = Math.Round(value, 0, MidpointRounding.AwayFromZero);

        if (Math.Abs(Weight - rounded) > 0.01) Weight = rounded;

        RecalculateBmi();
        TargetLimitsChanged?.Invoke();
    }

    private void RecalculateBmi()
    {
        var bmi = _bmiService.CalculateBmi(Weight, Height);

        BmiValue = Math.Round(bmi, 1);

        switch (bmi)
        {
            case < 18.5:
                BmiCategoryText = new LocalizedString(() => _localizationManager["BMI_Underweight"]);
                BmiDescription = new LocalizedString(() => _localizationManager["BMI_Desc_Underweight"]);
                BmiColor = (Color)Application.Current?.Resources["BmiUnderweight"]!;
                break;
            case >= 18.5 and < 25:
                BmiCategoryText = new LocalizedString(() => _localizationManager["BMI_Normal"]);
                BmiDescription = new LocalizedString(() => _localizationManager["BMI_Desc_Normal"]);
                BmiColor = (Color)Application.Current?.Resources["BmiNormal"]!;
                break;
            case >= 25 and < 30:
                BmiCategoryText = new LocalizedString(() => _localizationManager["BMI_Overweight"]);
                BmiDescription = new LocalizedString(() => _localizationManager["BMI_Desc_Overweight"]);
                BmiColor = (Color)Application.Current?.Resources["BmiOverweight"]!;
                break;
            default:
                BmiCategoryText = new LocalizedString(() => _localizationManager["BMI_Obesity"]);
                BmiDescription = new LocalizedString(() => _localizationManager["BMI_Desc_Obesity"]);
                BmiColor = (Color)Application.Current?.Resources["BmiObesity"]!;
                break;
        }
    }

    public bool ValidateStep()
    {
        if (Height > 0 && Weight > 0) return true;

        ValidationError = new LocalizedString(() => _localizationManager["EmptyFieldsMessage"]);
        return false;
    }

    public void ClearErrors()
    {
        ValidationError = null;
    }

    public event Action? TargetLimitsChanged;
}