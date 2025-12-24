using System.Windows.Input;

namespace ForgeFit.MAUI.Views.Controls;

public partial class NavigationHeader : ContentView
{
    public NavigationHeader()
    {
        DefaultBackCommand = new Command(OnDefaultBack);
        InitializeComponent();
    }

    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(NavigationHeader), string.Empty);

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly BindableProperty CustomTitleViewProperty =
        BindableProperty.Create(nameof(CustomTitleView), typeof(View), typeof(NavigationHeader), null,
            propertyChanged: OnCustomTitleViewChanged);

    public View CustomTitleView
    {
        get => (View)GetValue(CustomTitleViewProperty);
        set => SetValue(CustomTitleViewProperty, value);
    }

    public static readonly BindableProperty ShowBackButtonProperty =
        BindableProperty.Create(nameof(ShowBackButton), typeof(bool), typeof(NavigationHeader), true);

    public bool ShowBackButton
    {
        get => (bool)GetValue(ShowBackButtonProperty);
        set => SetValue(ShowBackButtonProperty, value);
    }

    public static readonly BindableProperty BackCommandProperty =
        BindableProperty.Create(nameof(BackCommand), typeof(ICommand), typeof(NavigationHeader));

    public ICommand BackCommand
    {
        get => (ICommand)GetValue(BackCommandProperty);
        set => SetValue(BackCommandProperty, value);
    }

    public ICommand DefaultBackCommand { get; }

    private static void OnCustomTitleViewChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is NavigationHeader control) control.UpdateVisibility();
    }

    private void UpdateVisibility()
    {
        if (CustomTitleView != null)
        {
            TitleLabel.IsVisible = false;
            CustomContentContainer.IsVisible = true;
        }
        else
        {
            TitleLabel.IsVisible = true;
            CustomContentContainer.IsVisible = false;
        }
    }

    private async void OnDefaultBack()
    {
        if (BackCommand != null && BackCommand.CanExecute(null))
        {
            BackCommand.Execute(null);
            return;
        }

        await Shell.Current.GoToAsync("..", false);
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();
        UpdateVisibility();
    }
}
