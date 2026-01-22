using System.Windows.Input;

namespace ForgeFit.MAUI.Views.Controls;

public partial class EntryPopup : ContentView
{
    public EntryPopup()
    {
        InitializeComponent();

        Content.BindingContext = this;
    }

    public static readonly BindableProperty EntryPopupTitleProperty = BindableProperty.Create(
        nameof(EntryPopupTitle), typeof(string), typeof(EntryPopup));

    public string EntryPopupTitle
    {
        get => (string)GetValue(EntryPopupTitleProperty);
        set => SetValue(EntryPopupTitleProperty, value);
    }

    public static readonly BindableProperty EntryPopupTextProperty = BindableProperty.Create(
        nameof(EntryPopupText), typeof(string), typeof(EntryPopup), null, BindingMode.TwoWay);

    public string EntryPopupText
    {
        get => (string)GetValue(EntryPopupTextProperty);
        set => SetValue(EntryPopupTextProperty, value);
    }

    public static readonly BindableProperty EntryPopupButtonCommandProperty = BindableProperty.Create(
        nameof(EntryPopupButtonCommand), typeof(ICommand), typeof(EntryPopup));

    public ICommand EntryPopupButtonCommand
    {
        get => (ICommand)GetValue(EntryPopupButtonCommandProperty);
        set => SetValue(EntryPopupButtonCommandProperty, value);
    }

    public static readonly BindableProperty CloseEntryPopupCommandProperty = BindableProperty.Create(
        nameof(CloseEntryPopupCommand), typeof(ICommand), typeof(EntryPopup));

    public ICommand CloseEntryPopupCommand
    {
        get => (ICommand)GetValue(CloseEntryPopupCommandProperty);
        set => SetValue(CloseEntryPopupCommandProperty, value);
    }

    public static readonly BindableProperty EntryPopupPlaceholderProperty = BindableProperty.Create(
        nameof(EntryPopupPlaceholder), typeof(string), typeof(EntryPopup), string.Empty);

    public string EntryPopupPlaceholder
    {
        get => (string)GetValue(EntryPopupPlaceholderProperty);
        set => SetValue(EntryPopupPlaceholderProperty, value);
    }
}
