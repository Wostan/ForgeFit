namespace ForgeFit.MAUI.Behaviors;

public class FadeVisibilityBehavior : Behavior<VisualElement>
{
    private VisualElement? _element;

    public static readonly BindableProperty IsVisibleProperty = BindableProperty.Create(
        nameof(IsVisible),
        typeof(bool), 
        typeof(FadeVisibilityBehavior),
        false, 
        propertyChanged: OnIsVisibleChanged);

    public bool IsVisible
    {
        get => (bool)GetValue(IsVisibleProperty);
        set => SetValue(IsVisibleProperty, value);
    }

    protected override void OnAttachedTo(VisualElement bindable)
    {
        base.OnAttachedTo(bindable);
        _element = bindable;
        _element.Opacity = 0;
        _element.IsVisible = false;
    }

    protected override void OnDetachingFrom(VisualElement bindable)
    {
        base.OnDetachingFrom(bindable);
        _element = null;
    }

    private static async void OnIsVisibleChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not FadeVisibilityBehavior behavior || behavior._element is null) return;

        var isVisible = (bool)newValue;

        behavior._element.CancelAnimations();

        if (isVisible)
        {
            behavior._element.IsVisible = true;
            await behavior._element.FadeToAsync(1, 250, Easing.SinOut);
        }
        else
        {
            await behavior._element.FadeToAsync(0, 250, Easing.SinIn);
            behavior._element.IsVisible = false;
        }
    }
}
