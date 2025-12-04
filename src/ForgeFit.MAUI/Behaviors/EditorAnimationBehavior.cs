namespace ForgeFit.MAUI.Behaviors;

public class EditorAnimationBehavior : Behavior<Editor>
{
    public static readonly BindableProperty IsAnimatedProperty =
        BindableProperty.CreateAttached(
            "IsAnimated",
            typeof(bool),
            typeof(EditorAnimationBehavior),
            false,
            propertyChanged: OnIsAnimatedChanged);

    private const uint AnimationDuration = 250;
    private const int AnimationRate = 16;

    private const string StrokeAnimationName = "StrokeColorAnim";
    private const string PrimaryColorKey = "Primary";
    private const string BorderColorKey = "BorderColor";

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
        if (view is not Editor editor) return;

        var isAnimated = (bool)newValue;
        if (isAnimated)
        {
            if (!editor.Behaviors.Any(b => b is EditorAnimationBehavior))
                editor.Behaviors.Add(new EditorAnimationBehavior());
        }
        else
        {
            var existing = editor.Behaviors.FirstOrDefault(b => b is EditorAnimationBehavior);
            if (existing != null)
                editor.Behaviors.Remove(existing);
        }
    }

    protected override void OnAttachedTo(Editor editor)
    {
        base.OnAttachedTo(editor);
        editor.Focused += OnEditorFocused;
        editor.Unfocused += OnEditorUnfocused;
    }

    protected override void OnDetachingFrom(Editor editor)
    {
        editor.Focused -= OnEditorFocused;
        editor.Unfocused -= OnEditorUnfocused;
        base.OnDetachingFrom(editor);
    }

    private static async void OnEditorFocused(object? sender, FocusEventArgs e)
    {
        if (sender is not Editor editor) return;

        if (editor.Parent is not Border border ||
            !Application.Current!.Resources.TryGetValue(PrimaryColorKey, out var primaryObj) ||
            primaryObj is not Color primaryColor ||
            !Application.Current.Resources.TryGetValue(BorderColorKey, out var borderObj) ||
            borderObj is not Color borderColor) return;

        var startBrush = border.Stroke as SolidColorBrush ?? new SolidColorBrush(borderColor);
        await AnimateBorderStrokeAsync(border, startBrush, primaryColor);
    }

    private static async void OnEditorUnfocused(object? sender, FocusEventArgs e)
    {
        if (sender is not Editor editor) return;

        if (editor.Parent is not Border border ||
            !Application.Current!.Resources.TryGetValue(PrimaryColorKey, out var primaryObj) ||
            primaryObj is not Color primaryColor ||
            !Application.Current.Resources.TryGetValue(BorderColorKey, out var borderObj) ||
            borderObj is not Color borderColor) return;

        var startBrush = border.Stroke as SolidColorBrush ?? new SolidColorBrush(primaryColor);
        await AnimateBorderStrokeAsync(border, startBrush, borderColor);
    }

    private static Task<bool> AnimateBorderStrokeAsync(Border border, SolidColorBrush startBrush, Color endColor)
    {
        var tcs = new TaskCompletionSource<bool>();
        var startColor = startBrush.Color;

        border.AbortAnimation(StrokeAnimationName);

        var animation = new Animation(v =>
        {
            var r = startColor.Red + (endColor.Red - startColor.Red) * v;
            var g = startColor.Green + (endColor.Green - startColor.Green) * v;
            var b = startColor.Blue + (endColor.Blue - startColor.Blue) * v;
            var a = startColor.Alpha + (endColor.Alpha - startColor.Alpha) * v;

            border.Stroke = new Color((float)r, (float)g, (float)b, (float)a);
        });

        animation.Commit(border, StrokeAnimationName, AnimationRate, AnimationDuration, Easing.CubicOut,
            (_, _) => tcs.SetResult(true));

        return tcs.Task;
    }
}
