using CommunityToolkit.Mvvm.ComponentModel;
using LocalizationResourceManager.Maui;

namespace ForgeFit.MAUI.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty] 
    [NotifyPropertyChangedFor(nameof(ShowContent))]
    private bool _isLoading;
    
    [ObservableProperty] 
    [NotifyPropertyChangedFor(nameof(IsError))]
    private LocalizedString? _error;

    public bool IsError => !string.IsNullOrWhiteSpace(Error.Localized);
    public bool ShowContent => !IsLoading && !IsError;
}
