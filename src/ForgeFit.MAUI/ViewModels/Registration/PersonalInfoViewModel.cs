using CommunityToolkit.Mvvm.ComponentModel;
using ForgeFit.MAUI.Models.Enums.ProfileEnums;
using ForgeFit.MAUI.ViewModels.Core;
using LocalizationResourceManager.Maui;

namespace ForgeFit.MAUI.ViewModels.Registration;

public partial class PersonalInfoViewModel(ILocalizationResourceManager localizationManager) : BaseViewModel
{
    [ObservableProperty] private DateTime _birthDate = DateTime.Today.AddYears(-20);
    [ObservableProperty] private Gender _gender = Gender.Male;
    [ObservableProperty] private bool _isUsernameError;
    [ObservableProperty] private string _username = string.Empty;

    [ObservableProperty] private LocalizedString? _validationError;

    public DateTime MaxDate => DateTime.Today.AddYears(-13);
    public DateTime MinDate => DateTime.Today.AddYears(-100);

    public bool ValidateStep()
    {
        if (string.IsNullOrWhiteSpace(Username))
        {
            IsUsernameError = true;
            ValidationError = new LocalizedString(() => localizationManager["EmptyFieldsMessage"]);
            return false;
        }

        if (Username.Length > 20)
        {
            IsUsernameError = true;
            ValidationError = new LocalizedString(() => localizationManager["Error_UsernameTooLong"]);
            return false;
        }

        var minAgeDate = DateTime.Today.AddYears(-13);
        if (BirthDate > minAgeDate)
        {
            ValidationError = new LocalizedString(() => localizationManager["Error_InvalidAge"]);
            return false;
        }

        return true;
    }

    public void ClearErrors()
    {
        IsUsernameError = false;
        ValidationError = null;
    }
}