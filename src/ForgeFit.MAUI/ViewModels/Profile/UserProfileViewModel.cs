using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ForgeFit.MAUI.Models.DTOs.User;
using ForgeFit.MAUI.Models.Enums.ProfileEnums;
using ForgeFit.MAUI.Services.Interfaces;
using LocalizationResourceManager.Maui;

namespace ForgeFit.MAUI.ViewModels.Profile;

public partial class UserProfileViewModel(
    IUserService userService,
    IAlertService alertService,
    ILocalizationResourceManager localizationManager)
    : BaseViewModel
{
    private UserProfileDto? _currentUserProfile;

    [ObservableProperty] private string _username = string.Empty;
    [ObservableProperty] private string _avatarUrl = string.Empty;
    [ObservableProperty] private string _birthDate = string.Empty;
    [ObservableProperty] private string _genderEmoji = string.Empty;
    [ObservableProperty] private string _gender = string.Empty;
    [ObservableProperty] private string _weight = string.Empty;
    [ObservableProperty] private WeightUnit _weightUnit;
    [ObservableProperty] private string _height = string.Empty;
    [ObservableProperty] private HeightUnit _heightUnit;

    [ObservableProperty] private bool _isEditProfilePopupVisible;
    [ObservableProperty] private string? _editUsername;
    [ObservableProperty] private DateTime _editBirthDate;
    [ObservableProperty] private Gender _editGender;
    [ObservableProperty] private string? _editHeight;
    [ObservableProperty] private string? _editCurrentWeight;

    public ObservableCollection<Gender> Genders { get; } = new(Enum.GetValues<Gender>());

    public void UpdateState(UserProfileDto profile)
    {
        _currentUserProfile = profile;
        Username = profile.Username;
        AvatarUrl = profile.AvatarUrl ?? "avatar.svg";
        BirthDate = profile.DateOfBirth.ToString("dd.MM.yyyy");
        Gender = localizationManager[$"Gender_{profile.Gender}"];
        Weight = profile.Weight.ToString("F1");
        WeightUnit = profile.WeightUnit;
        Height = profile.Height.ToString("F0");
        HeightUnit = profile.HeightUnit;

        GenderEmoji = profile.Gender switch
        {
            Models.Enums.ProfileEnums.Gender.Male => "♂️",
            Models.Enums.ProfileEnums.Gender.Female => "♀️",
            _ => "❔"
        };
    }

    [RelayCommand]
    private void EditProfile()
    {
        if (_currentUserProfile == null) return;
        EditUsername = _currentUserProfile.Username;
        EditBirthDate = _currentUserProfile.DateOfBirth;
        EditGender = _currentUserProfile.Gender;
        EditHeight = _currentUserProfile.Height.ToString(CultureInfo.InvariantCulture);
        EditCurrentWeight = _currentUserProfile.Weight.ToString(CultureInfo.InvariantCulture);
        IsEditProfilePopupVisible = true;
    }

    [RelayCommand]
    private void CloseEditProfile()
    {
        IsEditProfilePopupVisible = false;
    }

    [RelayCommand]
    private async Task SaveProfile()
    {
        if (string.IsNullOrWhiteSpace(EditUsername) ||
            !double.TryParse(EditHeight, CultureInfo.InvariantCulture, out var height) ||
            !double.TryParse(EditCurrentWeight, CultureInfo.InvariantCulture, out var weight))
        {
            await alertService.ShowToastAsync(localizationManager["Error_InvalidInput"]);
            return;
        }

        if (EditUsername.Length > 20)
        {
            await alertService.ShowToastAsync(localizationManager["Error_UsernameTooLong"]);
            return;
        }

        var age = DateTime.Today.Year - EditBirthDate.Year;
        if (EditBirthDate.Date > DateTime.Today.AddYears(-age)) age--;
        if (age is < 13 or > 100)
        {
            await alertService.ShowToastAsync(localizationManager["Error_InvalidAge"]);
            return;
        }

        var wUnit = _currentUserProfile?.WeightUnit ?? WeightUnit.Kg;
        var isWeightValid = wUnit == WeightUnit.Kg
            ? weight is >= 30 and <= 300
            : weight is >= 66 and <= 660;

        if (!isWeightValid)
        {
            await alertService.ShowToastAsync(localizationManager["Error_InvalidWeightRange"]);
            return;
        }

        var hUnit = _currentUserProfile?.HeightUnit ?? HeightUnit.Cm;
        var isHeightValid = hUnit == HeightUnit.Cm
            ? height is >= 100 and <= 250
            : height is >= 40 and <= 98;

        if (!isHeightValid)
        {
            await alertService.ShowToastAsync(localizationManager["Error_InvalidHeightRange"]);
            return;
        }

        IsEditProfilePopupVisible = false;
        IsLoading = true;

        try
        {
            var dto = new UserProfileDto(
                EditUsername,
                _currentUserProfile?.AvatarUrl,
                EditBirthDate,
                EditGender,
                weight,
                wUnit,
                height,
                hUnit
            );

            var result = await userService.UpdateProfileAsync(dto, CancellationToken.None);

            if (result is { Success: true, Data: not null })
            {
                UpdateState(result.Data);
                await alertService.ShowToastAsync(localizationManager["Success_ProfileUpdated"]);
                OnProfileUpdated?.Invoke(result.Data);
            }
            else
            {
                await alertService.ShowToastAsync(result.Message);
            }
        }
        catch
        {
            await alertService.ShowToastAsync(localizationManager["UnexpectedErrorMessage"]);
        }
        finally
        {
            IsLoading = false;
        }
    }

    public event Action<UserProfileDto>? OnProfileUpdated;
}
