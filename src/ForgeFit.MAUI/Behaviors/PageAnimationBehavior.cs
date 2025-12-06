namespace ForgeFit.MAUI.Behaviors;

public class PageAnimationBehavior : Behavior<ContentPage>
{
    public static readonly BindableProperty IsAnimatedProperty =
        BindableProperty.CreateAttached(
            "IsAnimated",
            typeof(bool),
            typeof(PageAnimationBehavior),
            false,
            propertyChanged: OnIsAnimatedChanged);

    private const uint AnimationDuration = 300;
    private const double StartingScaleX = 0.95;
    private const double StartingScaleY = 1.15;
    private const double StartingTranslationY = 50.0;

    private ContentPage? _page;
    private bool _isAnimatingOut;

    public static bool GetIsAnimated(BindableObject view)
    {
        return (bool)view.GetValue(IsAnimatedProperty);
    }

    public static void SetIsAnimated(BindableObject view, bool value)
    {
        view.SetValue(IsAnimatedProperty, value);
    }

    private static void OnIsAnimatedChanged(BindableObject view, object oldValue, object newValue)
    {
        if (view is not ContentPage page) return;

        var isAnimated = (bool)newValue;
        if (isAnimated)
        {
            if (!page.Behaviors.Any(b => b is PageAnimationBehavior))
                page.Behaviors.Add(new PageAnimationBehavior());
        }
        else
        {
            var existing = page.Behaviors.FirstOrDefault(b => b is PageAnimationBehavior);
            if (existing != null)
                page.Behaviors.Remove(existing);
        }
    }

    protected override void OnAttachedTo(ContentPage page)
    {
        base.OnAttachedTo(page);
        _page = page;

        if (page.Content is { } content)
            InitializeState(content);
        else
            page.PropertyChanged += OnPagePropertyChanged;

        page.Appearing += OnPageAppearing;
        page.Disappearing += OnPageDisappearing;
    }

    protected override void OnDetachingFrom(ContentPage page)
    {
        page.Appearing -= OnPageAppearing;
        page.Disappearing -= OnPageDisappearing;
        page.PropertyChanged -= OnPagePropertyChanged;

        if (Shell.Current is not null) Shell.Current.Navigating -= OnShellNavigating;

        _page = null;
        base.OnDetachingFrom(page);
    }

    private static void OnPagePropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(ContentPage.Content) || sender is not ContentPage { Content: { } content } page)
            return;

        page.PropertyChanged -= OnPagePropertyChanged;
        InitializeState(content);
    }

    private static void InitializeState(View content)
    {
        content.IsVisible = false;
        content.Opacity = 0;
        content.AnchorX = 0.5;
        content.AnchorY = 0.5;
        content.ScaleX = StartingScaleX;
        content.ScaleY = StartingScaleY;
        content.TranslationY = StartingTranslationY;
    }

    private void OnPageAppearing(object? sender, EventArgs e)
    {
        if (Shell.Current is not null)
        {
            Shell.Current.Navigating -= OnShellNavigating;
            Shell.Current.Navigating += OnShellNavigating;
        }

        if (sender is not ContentPage { Content: { } content } page) return;

        content.Opacity = 0;
        content.ScaleX = StartingScaleX;
        content.ScaleY = StartingScaleY;
        content.TranslationY = StartingTranslationY;

        page.Dispatcher.Dispatch(async void () =>
        {
            content.IsVisible = true;

            await Task.Delay(15);

            content.CancelAnimations();

            await Task.WhenAll(
                content.FadeToAsync(1, AnimationDuration, Easing.CubicOut),
                content.ScaleXToAsync(1.0, (int)(AnimationDuration * 1.5), Easing.CubicOut),
                content.ScaleYToAsync(1.0, (int)(AnimationDuration * 1.5), Easing.CubicOut),
                content.TranslateToAsync(0, 0, AnimationDuration, Easing.CubicOut)
            );
        });
    }

    private void OnPageDisappearing(object? sender, EventArgs e)
    {
        if (Shell.Current is not null) Shell.Current.Navigating -= OnShellNavigating;
    }

    private async void OnShellNavigating(object? sender, ShellNavigatingEventArgs e)
    {
        if (_page == null || Shell.Current?.CurrentPage != _page) return;

        if (_isAnimatingOut) return;

        var source = e.Source;
        if (source is not (ShellNavigationSource.ShellSectionChanged or ShellNavigationSource.Pop
            or ShellNavigationSource.Push)) return;

        var content = _page.Content;
        if (content == null) return;

        e.Cancel();
        _isAnimatingOut = true;

        content.CancelAnimations();

        await Task.WhenAll(
            content.FadeToAsync(0, 200, Easing.CubicIn),
            content.ScaleXToAsync(StartingScaleX, 200, Easing.CubicIn),
            content.ScaleYToAsync(StartingScaleY, 200, Easing.CubicIn),
            content.TranslateToAsync(0, -50, 200, Easing.CubicIn)
        );

        await Shell.Current.GoToAsync(e.Target.Location, false);

        _isAnimatingOut = false;
    }
}
