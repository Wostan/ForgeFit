namespace ForgeFit.MAUI.Behaviors;

public class ButtonAnimationBehavior : Behavior<Button>
{
    public static readonly BindableProperty IsAnimatedProperty = 
        BindableProperty.CreateAttached(
            "IsAnimated", 
            typeof(bool), typeof(ButtonAnimationBehavior), 
            false, 
            propertyChanged: OnIsAnimatedChanged);
    
    private const uint PressDuration = 200;
    private const uint ReleaseDuration = 400;
    private const double PressedScaleX = 0.97;
    private const double PressedScaleY = 1.05;
    private const double ReleasedScaleX = 1.0;
    private const double ReleasedScaleY = 1.0;

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
        
        button.CancelAnimations();

        await Task.WhenAll(
            button.ScaleXTo(PressedScaleX, PressDuration, Easing.CubicOut), 
            button.ScaleYTo(PressedScaleY, PressDuration, Easing.CubicOut)
            );
    }

    private static async void OnButtonReleased(object? sender, EventArgs e)
    {
        if (sender is not Button button) return;
        
        button.CancelAnimations();
        
        await Task.WhenAll(
            button.ScaleXTo(ReleasedScaleX, ReleaseDuration, Easing.CubicOut), 
            button.ScaleYTo(ReleasedScaleY, ReleaseDuration, Easing.CubicOut)
            );
    }
    
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
        if (view is not Button button) return;

        var isAnimated = (bool)newValue;

        if (isAnimated)
        {
            if (!button.Behaviors.Any(b => b is ButtonAnimationBehavior))
            {
                button.Behaviors.Add(new ButtonAnimationBehavior());
            }
        }
        else
        {
            var existingBehavior = button.Behaviors.FirstOrDefault(b => b is ButtonAnimationBehavior);
            if (existingBehavior != null)
            {
                button.Behaviors.Remove(existingBehavior);
            }
        }
    }
}
