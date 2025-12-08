namespace ForgeFit.MAUI.Behaviors;

public class ButtonAnimationBehavior : Behavior<Button>
{
    public static readonly BindableProperty IsAnimatedProperty =
        BindableProperty.CreateAttached(
            "IsAnimated",
            typeof(bool), typeof(ButtonAnimationBehavior),
            false,
            propertyChanged: OnIsAnimatedChanged);

    public static readonly BindableProperty IsLoadingProperty =
        BindableProperty.CreateAttached(
            "IsLoading",
            typeof(bool), typeof(ButtonAnimationBehavior),
            false,
            propertyChanged: OnIsLoadingChanged);

    public static bool GetIsAnimated(BindableObject view)
    {
        return (bool)view.GetValue(IsAnimatedProperty);
    }

    public static void SetIsAnimated(BindableObject view, bool value)
    {
        view.SetValue(IsAnimatedProperty, value);
    }

    public static bool GetIsLoading(BindableObject view)
    {
        return (bool)view.GetValue(IsLoadingProperty);
    }

    public static void SetIsLoading(BindableObject view, bool value)
    {
        view.SetValue(IsLoadingProperty, value);
    }

    private const uint FadeDuration = 400;
    private const uint PressDuration = 200;
    private const uint ReleaseDuration = 400;
    private const double PressedScaleX = 0.97;
    private const double PressedScaleY = 1.05;
    private const double ReleasedScaleX = 1.0;
    private const double ReleasedScaleY = 1.0;

    private static void OnIsAnimatedChanged(BindableObject view, object oldValue, object newValue)
    {
        if (view is not Button button) return;

        var isAnimated = (bool)newValue;

        if (isAnimated)
        {
            if (!button.Behaviors.Any(b => b is ButtonAnimationBehavior))
                button.Behaviors.Add(new ButtonAnimationBehavior());
        }
        else
        {
            var existingBehavior = button.Behaviors.FirstOrDefault(b => b is ButtonAnimationBehavior);
            if (existingBehavior != null) button.Behaviors.Remove(existingBehavior);
        }
    }

    private static void OnIsLoadingChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not Button button) return;

        var isLoading = (bool)newValue;

        button.Dispatcher.Dispatch(async void () =>
        {
            if (isLoading)
            {
                VisualStateManager.GoToState(button, "Normal");
                await button.FadeToAsync(0, FadeDuration, Easing.CubicOut);
                button.InputTransparent = true;
            }
            else
            {
                button.InputTransparent = false;
                button.ScaleX = ReleasedScaleX;
                button.ScaleY = ReleasedScaleY;
                VisualStateManager.GoToState(button, "Normal");
                await button.FadeToAsync(1, FadeDuration, Easing.CubicOut);
            }
        });
    }

    protected override void OnAttachedTo(Button button)
    {
        base.OnAttachedTo(button);
        button.Pressed += OnButtonPressed;
        button.Released += OnButtonReleased;
    }

    protected override void OnDetachingFrom(Button button)
    {
        button.Pressed -= OnButtonPressed;
        button.Released -= OnButtonReleased;
        base.OnDetachingFrom(button);
    }

    private static async void OnButtonPressed(object? sender, EventArgs e)
    {
        if (sender is not Button button) return;

        if (GetIsLoading(button)) return;

        button.CancelAnimations();

        await Task.WhenAll(
            button.ScaleXToAsync(PressedScaleX, PressDuration, Easing.CubicOut),
            button.ScaleYToAsync(PressedScaleY, PressDuration, Easing.CubicOut)
        );
    }

    private static async void OnButtonReleased(object? sender, EventArgs e)
    {
        if (sender is not Button button) return;

        if (GetIsLoading(button)) return;

        button.CancelAnimations();

        await Task.WhenAll(
            button.ScaleXToAsync(ReleasedScaleX, ReleaseDuration, Easing.CubicOut),
            button.ScaleYToAsync(ReleasedScaleY, ReleaseDuration, Easing.CubicOut)
        );
    }
}
