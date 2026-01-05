using System.Windows.Input;
using CommunityToolkit.Maui.PlatformConfiguration.AndroidSpecific;

namespace ForgeFit.MAUI.Views.Controls;

public partial class StateContainer : ContentView
{
    public StateContainer()
    {
        InitializeComponent();
        Loaded += (_, _) => UpdateTabBarVisibility();
    }

    public static readonly BindableProperty IsLoadingProperty =
        BindableProperty.Create(nameof(IsLoading), typeof(bool), typeof(StateContainer), false, propertyChanged: OnStateChanged);

    public bool IsLoading
    {
        get => (bool)GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }

    public static readonly BindableProperty IsErrorProperty =
        BindableProperty.Create(nameof(IsError), typeof(bool), typeof(StateContainer), false, propertyChanged: OnStateChanged);

    public bool IsError
    {
        get => (bool)GetValue(IsErrorProperty);
        set => SetValue(IsErrorProperty, value);
    }

    public static readonly BindableProperty ErrorMessageProperty =
        BindableProperty.Create(nameof(ErrorMessage), typeof(string), typeof(StateContainer), string.Empty);

    public string ErrorMessage
    {
        get => (string)GetValue(ErrorMessageProperty);
        set => SetValue(ErrorMessageProperty, value);
    }

    public static readonly BindableProperty RetryCommandProperty =
        BindableProperty.Create(nameof(RetryCommand), typeof(ICommand), typeof(StateContainer));

    public ICommand RetryCommand
    {
        get => (ICommand)GetValue(RetryCommandProperty);
        set => SetValue(RetryCommandProperty, value);
    }

    public bool IsContentVisible => !IsLoading && !IsError;

    private static void OnStateChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not StateContainer control) return;
        
        control.OnPropertyChanged(nameof(IsContentVisible));
        control.UpdateTabBarVisibility();
    }
    
    private void UpdateTabBarVisibility()
    {
        var current = Parent;
        
        while (current != null)
        {
            if (current is Page page)
            {
                Shell.SetTabBarIsVisible(page, IsContentVisible);
                return;
            }
            current = current.Parent;
        }
    }
}
