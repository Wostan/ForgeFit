using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ForgeFit.MAUI.Constants;
using ForgeFit.MAUI.Messages;
using ForgeFit.MAUI.Models.DTOs.User;
using ForgeFit.MAUI.Models.Enums.ProfileEnums;
using ForgeFit.MAUI.Services.Interfaces;
using ForgeFit.MAUI.ViewModels.Core;
using LocalizationResourceManager.Maui;

namespace ForgeFit.MAUI.ViewModels.Profile.Main;

public partial class UserProfileViewModel(
    IUserService userService,
    IAlertService alertService,
    ILocalizationResourceManager localizationManager)
    : BaseViewModel
{
    [ObservableProperty] private string _avatarUrl = string.Empty;
    [ObservableProperty] private string _birthDate = string.Empty;
    private UserProfileDto? _currentUserProfile;
    [ObservableProperty] private DateTime _editBirthDate;
    [ObservableProperty] private string? _editCurrentWeight;
    [ObservableProperty] private Gender _editGender;
    [ObservableProperty] private string? _editHeight;
    [ObservableProperty] private string? _editUsername;
    [ObservableProperty] private string _gender = string.Empty;
    [ObservableProperty] private string _genderEmoji = string.Empty;
    [ObservableProperty] private string _height = string.Empty;
    [ObservableProperty] private HeightUnit _heightUnit;

    [ObservableProperty] private bool _isEditProfilePopupVisible;

    [ObservableProperty] private string _username = string.Empty;
    [ObservableProperty] private string _weight = string.Empty;
    [ObservableProperty] private WeightUnit _weightUnit;

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

        if (EditUsername.Length > AppConstants.ValidationLimits.MaxUsernameLength)
        {
            await alertService.ShowToastAsync(localizationManager["Error_UsernameTooLong"]);
            return;
        }

        var age = DateTime.Today.Year - EditBirthDate.Year;
        if (EditBirthDate.Date > DateTime.Today.AddYears(-age)) age--;
        if (age is < AppConstants.ValidationLimits.MinAgeYears or > AppConstants.ValidationLimits.MaxAgeYears)
        {
            await alertService.ShowToastAsync(localizationManager["Error_InvalidAge"]);
            return;
        }

        var wUnit = _currentUserProfile?.WeightUnit ?? WeightUnit.Kg;
        var isWeightValid = wUnit == WeightUnit.Kg
            ? weight is >= AppConstants.ValidationLimits.MinWeightKg and <= AppConstants.ValidationLimits.MaxWeightKg
            : weight is >= AppConstants.ValidationLimits.MinWeightLbs and <= AppConstants.ValidationLimits.MaxWeightLbs;

        if (!isWeightValid)
        {
            await alertService.ShowToastAsync(localizationManager["Error_InvalidWeightRange"]);
            return;
        }

        var hUnit = _currentUserProfile?.HeightUnit ?? HeightUnit.Cm;
        var isHeightValid = hUnit == HeightUnit.Cm
            ? height is >= AppConstants.ValidationLimits.MinHeightCm and <= AppConstants.ValidationLimits.MaxHeightCm
            : height is >= AppConstants.ValidationLimits.MinHeightInches and <= AppConstants.ValidationLimits.MaxHeightInches;

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
                WeakReferenceMessenger.Default.Send(new WeightChangedMessage(nameof(UserProfileViewModel)));
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