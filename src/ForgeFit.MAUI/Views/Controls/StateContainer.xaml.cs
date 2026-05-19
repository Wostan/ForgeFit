using System.Windows.Input;

namespace ForgeFit.MAUI.Views.Controls;

public partial class StateContainer : ContentView
{
    public static readonly BindableProperty IsLoadingProperty =
        BindableProperty.Create(nameof(IsLoading), typeof(bool), typeof(StateContainer), false,
            propertyChanged: OnStateChanged);

    public static readonly BindableProperty IsErrorProperty =
        BindableProperty.Create(nameof(IsError), typeof(bool), typeof(StateContainer), false,
            propertyChanged: OnStateChanged);

    public static readonly BindableProperty ErrorMessageProperty =
        BindableProperty.Create(nameof(ErrorMessage), typeof(string), typeof(StateContainer), string.Empty);

    public static readonly BindableProperty RetryCommandProperty =
        BindableProperty.Create(nameof(RetryCommand), typeof(ICommand), typeof(StateContainer));

    public StateContainer()
    {
        InitializeComponent();
    }

    public bool IsLoading
    {
        get => (bool)GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }

    public bool IsError
    {
        get => (bool)GetValue(IsErrorProperty);
        set => SetValue(IsErrorProperty, value);
    }

    public string ErrorMessage
    {
        get => (string)GetValue(ErrorMessageProperty);
        set => SetValue(ErrorMessageProperty, value);
    }

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
    }
}
