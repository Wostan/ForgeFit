namespace ForgeFit.MAUI.Behaviors;

public class SwitchAnimationBehavior : Behavior<Switch>
{
    public static readonly BindableProperty IsAnimatedProperty =
        BindableProperty.CreateAttached(
            "IsAnimated",
            typeof(bool),
            typeof(SwitchAnimationBehavior),
            false,
            propertyChanged: OnIsAnimatedChanged);

    private const uint AnimationDuration = 250;
    
    private const string ThumbColorAnimationName = "ThumbColorAnim";

    private const string PrimaryColorKey = "Primary";
    private const string SurfaceLightColorKey = "SurfaceLight";
    
    private const string ShadowPrimaryKey = "SharedPrimaryGlow";
    private const string ShadowSoftKey = "SharedSoftGlow";

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
        if (view is not Switch switchControl) return;

        var isAnimated = (bool)newValue;
        if (isAnimated)
        {
            if (!switchControl.Behaviors.Any(b => b is SwitchAnimationBehavior))
                switchControl.Behaviors.Add(new SwitchAnimationBehavior());
        }
        else
        {
            var existing = switchControl.Behaviors.FirstOrDefault(b => b is SwitchAnimationBehavior);
            if (existing != null)
                switchControl.Behaviors.Remove(existing);
        }
    }

    protected override void OnAttachedTo(Switch switchControl)
    {
        base.OnAttachedTo(switchControl);
        switchControl.Toggled += OnSwitchToggled;
        
        _ = UpdateVisualState(switchControl, switchControl.IsToggled, animate: false);
    }

    protected override void OnDetachingFrom(Switch switchControl)
    {
        switchControl.Toggled -= OnSwitchToggled;
        base.OnDetachingFrom(switchControl);
    }

    private static async void OnSwitchToggled(object? sender, ToggledEventArgs e)
    {
        if (sender is not Switch switchControl) return;
        
        await UpdateVisualState(switchControl, e.Value, animate: true);
    }

    private static async Task UpdateVisualState(Switch switchControl, bool isToggled, bool animate)
    {
        var targetColorKey = isToggled ? PrimaryColorKey : SurfaceLightColorKey;
        var targetShadowKey = isToggled ? ShadowPrimaryKey : ShadowSoftKey;

        if (!Application.Current!.Resources.TryGetValue(targetColorKey, out var colorObj) ||
            colorObj is not Color targetThumbColor ||
            !Application.Current.Resources.TryGetValue(targetShadowKey, out var shadowObj) ||
            shadowObj is not Shadow targetShadow)
        {
            return;
        }
        
        switchControl.Shadow = targetShadow;

        if (!animate || !switchControl.IsEnabled)
        {
            switchControl.AbortAnimation(ThumbColorAnimationName);
            switchControl.ThumbColor = targetThumbColor;
        }
        else
        {
            var startColor = switchControl.ThumbColor ?? Colors.Transparent;
            await AnimateThumbColorAsync(switchControl, startColor, targetThumbColor);
        }
    }

    private static Task<bool> AnimateThumbColorAsync(Switch switchControl, Color startColor, Color endColor)
    {
        var tcs = new TaskCompletionSource<bool>();

        switchControl.AbortAnimation(ThumbColorAnimationName);

        var animation = new Animation(v =>
        {
            var r = startColor.Red + (endColor.Red - startColor.Red) * v;
            var g = startColor.Green + (endColor.Green - startColor.Green) * v;
            var b = startColor.Blue + (endColor.Blue - startColor.Blue) * v;
            var a = startColor.Alpha + (endColor.Alpha - startColor.Alpha) * v;

            switchControl.ThumbColor = new Color((float)r, (float)g, (float)b, (float)a);
        });

        animation.Commit(switchControl, ThumbColorAnimationName, 16, AnimationDuration, Easing.CubicOut,
            (_, _) => tcs.SetResult(true));

        return tcs.Task;
    }
}
