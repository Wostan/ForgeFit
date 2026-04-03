using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using ForgeFit.MAUI.Models.Enums.ProfileEnums;
using ForgeFit.MAUI.ViewModels.Core;
using LocalizationResourceManager.Maui;

namespace ForgeFit.MAUI.ViewModels.Registration;

public record LanguageItem(string Name, string Code);

public partial class NavigationViewModel : BaseViewModel
{
    private readonly ILocalizationResourceManager _localizationManager;
    [ObservableProperty] private string _buttonText = string.Empty;
    [ObservableProperty] private string _commitmentSubtitle = string.Empty;

    [ObservableProperty] private string _commitmentTitle = string.Empty;
    [ObservableProperty] private int _currentPosition;
    [ObservableProperty] private bool _isMainNavigationVisible = true;
    [ObservableProperty] private double _progress;
    [ObservableProperty] private LanguageItem? _selectedLanguage;

    public NavigationViewModel(ILocalizationResourceManager localizationManager)
    {
        _localizationManager = localizationManager;

        var currentCode = _localizationManager.CurrentCulture.TwoLetterISOLanguageName;
        SelectedLanguage = Languages.FirstOrDefault(l => l.Code == currentCode)
                           ?? Languages.FirstOrDefault(l => l.Code == "en");

        UpdateState();
    }

    public ObservableCollection<string> Steps { get; } =
    [
        "Credentials", "Personal", "Measurements", "Goal", "Commitment"
    ];

    public List<LanguageItem> Languages { get; } =
    [
        new("English", "en"),
        new("Українська", "uk")
    ];

    partial void OnCurrentPositionChanged(int value)
    {
        UpdateState();
    }

    public void UpdateCommitmentText(string username, Gender gender)
    {
        var name = string.IsNullOrWhiteSpace(username) ? "User" : username;

        if (gender == Gender.Male)
        {
            CommitmentTitle = string.Format(_localizationManager["Reg_Commitment_Title_Male"], name);
            CommitmentSubtitle = _localizationManager["Reg_Commitment_Subtitle_Male"];
        }
        else
        {
            CommitmentTitle = string.Format(_localizationManager["Reg_Commitment_Title_Female"], name);
            CommitmentSubtitle = _localizationManager["Reg_Commitment_Subtitle_Female"];
        }
    }

    private void UpdateState()
    {
        Progress = (double)CurrentPosition / Steps.Count;

        var isFirstStep = CurrentPosition == 0;
        var isLastStep = CurrentPosition == Steps.Count - 1;

        IsMainNavigationVisible = !isFirstStep && !isLastStep;

        ButtonText = _localizationManager["Action_Next"];
    }
}