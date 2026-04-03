using CommunityToolkit.Mvvm.ComponentModel;
using LocalizationResourceManager.Maui;

namespace ForgeFit.MAUI.ViewModels.Core;

public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty] [NotifyPropertyChangedFor(nameof(IsError))]
    private LocalizedString? _error;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(ShowContent))]
    private bool _isLoading;

    public bool IsError => !string.IsNullOrWhiteSpace(Error?.Localized);
    public bool ShowContent => !IsLoading && !IsError;
}