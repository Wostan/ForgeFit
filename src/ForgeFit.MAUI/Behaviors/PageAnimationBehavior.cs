namespace ForgeFit.MAUI.Behaviors;

public class PageAnimationBehavior : Behavior<ContentPage>
{
    public static readonly BindableProperty AnimationTypeProperty =
        BindableProperty.CreateAttached(
            "AnimationType",
            typeof(PageAnimationType),
            typeof(PageAnimationBehavior),
            PageAnimationType.SlideUp);

    public static PageAnimationType GetAnimationType(BindableObject view)
    {
        return (PageAnimationType)view.GetValue(AnimationTypeProperty);
    }

    public static void SetAnimationType(BindableObject view, PageAnimationType value)
    {
        view.SetValue(AnimationTypeProperty, value);
    }

    public static readonly BindableProperty IsAnimatedProperty =
        BindableProperty.CreateAttached("IsAnimated", typeof(bool), typeof(PageAnimationBehavior), false,
            propertyChanged: OnIsAnimatedChanged);

    public static bool GetIsAnimated(BindableObject view)
    {
        return (bool)view.GetValue(IsAnimatedProperty);
    }

    public static void SetIsAnimated(BindableObject view, bool value)
    {
        view.SetValue(IsAnimatedProperty, value);
    }

    private const uint AnimationDuration = 200;
    private const uint ExitDuration = 200;

    private const double SlideScaleX = 0.95;
    private const double SlideScaleY = 1.05;
    private const double SlideTransY = 30.0;

    private const double PopScale = 0.90;

    private ContentPage? _page;
    private bool _isAnimatingOut;

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
            if (existing != null) page.Behaviors.Remove(existing);
        }
    }

    protected override void OnAttachedTo(ContentPage page)
    {
        base.OnAttachedTo(page);
        _page = page;
        if (page.Content is { } content) InitializeState(page, content);
        else page.PropertyChanged += OnPagePropertyChanged;

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
        if (e.PropertyName != nameof(ContentPage.Content) ||
            sender is not ContentPage { Content: { } content } page) return;
        page.PropertyChanged -= OnPagePropertyChanged;
        InitializeState(page, content);
    }

    private static void InitializeState(ContentPage page, View content)
    {
        var type = GetAnimationType(page);

        content.Opacity = 0;
        content.AnchorX = 0.5;
        content.AnchorY = 0.5;

        content.Scale = 1;
        content.ScaleX = 1;
        content.ScaleY = 1;
        content.TranslationY = 0;

        switch (type)
        {
            case PageAnimationType.SlideUp:
                content.ScaleX = SlideScaleX;
                content.ScaleY = SlideScaleY;
                content.TranslationY = SlideTransY;
                break;
            case PageAnimationType.Pop:
                content.Scale = PopScale;
                break;
            case PageAnimationType.Fade:
                break;
        }
    }

    private void OnPageAppearing(object? sender, EventArgs e)
    {
        if (Shell.Current is not null)
        {
            Shell.Current.Navigating -= OnShellNavigating;
            Shell.Current.Navigating += OnShellNavigating;
        }

        if (sender is not ContentPage { Content: { } content } page) return;

        InitializeState(page, content);

        page.Dispatcher.Dispatch(async void () =>
        {
            await Task.Delay(100);
            content.CancelAnimations();

            var type = GetAnimationType(page);

            switch (type)
            {
                case PageAnimationType.SlideUp:
                    await Task.WhenAll(
                        content.FadeToAsync(1, AnimationDuration, Easing.CubicOut),
                        content.ScaleXToAsync(1.0, (int)(AnimationDuration * 1.5), Easing.CubicOut),
                        content.ScaleYToAsync(1.0, AnimationDuration, Easing.CubicOut),
                        content.TranslateToAsync(0, 0, AnimationDuration, Easing.CubicOut)
                    );
                    break;

                case PageAnimationType.Pop:
                    await Task.WhenAll(
                        content.FadeToAsync(1, AnimationDuration, Easing.CubicOut),
                        content.ScaleToAsync(1.0, AnimationDuration, Easing.CubicOut)
                    );
                    break;

                case PageAnimationType.Fade:
                    await content.FadeToAsync(1, AnimationDuration, Easing.CubicOut);
                    break;
            }
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

        var content = _page.Content;
        if (content == null) return;

        e.Cancel();
        _isAnimatingOut = true;

        content.CancelAnimations();

        var type = GetAnimationType(_page);
        Task exitAnimationTask;

        switch (type)
        {
            case PageAnimationType.SlideUp:
                exitAnimationTask = Task.WhenAll(
                    content.FadeToAsync(0, ExitDuration, Easing.CubicIn),
                    content.ScaleXToAsync(SlideScaleX, ExitDuration, Easing.CubicIn),
                    content.ScaleYToAsync(SlideScaleY, ExitDuration, Easing.CubicIn),
                    content.TranslateToAsync(0, -25, ExitDuration, Easing.CubicIn)
                );
                break;

            case PageAnimationType.Pop:
                exitAnimationTask = Task.WhenAll(
                    content.FadeToAsync(0, ExitDuration, Easing.CubicIn),
                    content.ScaleToAsync(PopScale, ExitDuration, Easing.CubicIn)
                );
                break;

            default:
                exitAnimationTask = content.FadeToAsync(0, ExitDuration);
                break;
        }

        await exitAnimationTask;
        await Shell.Current.GoToAsync(e.Target.Location, false);

        _isAnimatingOut = false;
    }
}

public enum PageAnimationType
{
    SlideUp,
    Pop,
    Fade
}
