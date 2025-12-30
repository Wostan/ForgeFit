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

        BindingContext = bindable.BindingContext;
        bindable.BindingContextChanged += OnAssociatedObjectBindingContextChanged;

        if (IsVisible)
        {
            _element.Opacity = 1;
            _element.IsVisible = true;
        }
        else
        {
            _element.Opacity = 0;
            _element.IsVisible = false;
        }
    }

    protected override void OnDetachingFrom(VisualElement bindable)
    {
        base.OnDetachingFrom(bindable);
        bindable.BindingContextChanged -= OnAssociatedObjectBindingContextChanged;
        _element = null;
    }

    private void OnAssociatedObjectBindingContextChanged(object? sender, EventArgs e)
    {
        if (_element != null)
            BindingContext = _element.BindingContext;
    }

    private static async void OnIsVisibleChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not FadeVisibilityBehavior behavior || behavior._element is null) return;

        var isVisible = (bool)newValue;
        
        behavior._element?.CancelAnimations();

        if (isVisible)
        {
            behavior._element?.IsVisible = true; 
            await behavior._element!.FadeToAsync(1, 200, Easing.SinOut);
        }
        else
        { 
            await behavior._element!.FadeToAsync(0, 200, Easing.SinIn);
            behavior._element!.IsVisible = false;
        }
    }
}
