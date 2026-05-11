namespace ForgeFit.MAUI.Behaviors;

public class GridSlideBehavior
{
    private const uint AnimDuration = 400;
    private const double TranslationOffset = 100;

    public static readonly BindableProperty IsVisibleAnimatedProperty =
        BindableProperty.CreateAttached(
            "IsVisibleAnimated",
            typeof(bool),
            typeof(GridSlideBehavior),
            true,
            propertyChanged: OnIsVisibleAnimatedChanged);

    public static bool GetIsVisibleAnimated(BindableObject view)
    {
        return (bool)view.GetValue(IsVisibleAnimatedProperty);
    }

    public static void SetIsVisibleAnimated(BindableObject view, bool value)
    {
        view.SetValue(IsVisibleAnimatedProperty, value);
    }

    private static async void OnIsVisibleAnimatedChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not VisualElement view) return;

        var isVisible = (bool)newValue;

        view.CancelAnimations();

        if (isVisible)
        {
            view.IsVisible = true;
            view.InputTransparent = false;

            if (view is { TranslationY: 0, Opacity: 0 }) view.TranslationY = TranslationOffset;

            await Task.WhenAll(
                view.TranslateToAsync(0, 0, AnimDuration, Easing.CubicOut),
                view.FadeToAsync(1, AnimDuration, Easing.CubicOut)
            );
        }
        else
        {
            view.InputTransparent = true;

            await Task.WhenAll(
                view.TranslateToAsync(0, TranslationOffset, AnimDuration, Easing.CubicOut),
                view.FadeToAsync(0, AnimDuration, Easing.CubicOut)
            );
        }
    }
}
