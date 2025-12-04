#if ANDROID
using Android.Graphics.Drawables;
#endif

namespace ForgeFit.MAUI.Behaviors;

public class EntryAnimationBehavior : Behavior<Entry>
{
    public static readonly BindableProperty IsAnimatedProperty =
        BindableProperty.CreateAttached(
            "IsAnimated",
            typeof(bool),
            typeof(EntryAnimationBehavior),
            false,
            propertyChanged: OnIsAnimatedChanged);

    private const uint AnimationDuration = 250;
    private const int AnimationRate = 16;
    private const double FocusedTranslationY = 4.0;
    private const double UnfocusedTranslationY = 0.0;

    private const string StrokeAnimationName = "StrokeColorAnim";
    private const string PrimaryColorKey = "Primary";
    private const string BorderColorKey = "BorderColor";

#if ANDROID
    private Drawable? _originalBackground;
#endif

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
        if (view is not Entry entry) return;

        var isAnimated = (bool)newValue;
        if (isAnimated)
        {
            if (!entry.Behaviors.Any(b => b is EntryAnimationBehavior))
                entry.Behaviors.Add(new EntryAnimationBehavior());
        }
        else
        {
            var existing = entry.Behaviors.FirstOrDefault(b => b is EntryAnimationBehavior);
            if (existing != null)
                entry.Behaviors.Remove(existing);
        }
    }

    protected override void OnAttachedTo(Entry entry)
    {
        base.OnAttachedTo(entry);
        entry.Focused += OnEntryFocused;
        entry.Unfocused += OnEntryUnfocused;
    }

    protected override void OnDetachingFrom(Entry entry)
    {
        entry.Focused -= OnEntryFocused;
        entry.Unfocused -= OnEntryUnfocused;
        base.OnDetachingFrom(entry);
    }

    private async void OnEntryFocused(object? sender, FocusEventArgs e)
    {
        if (sender is not Entry entry) return;

#if ANDROID
        if (entry.Handler?.PlatformView is Android.Widget.EditText nativeEditText)
        {
            _originalBackground ??= nativeEditText.Background;
            nativeEditText.Background = new ColorDrawable(Android.Graphics.Color.Transparent);
        }
#endif

        var translationTask = entry.TranslateToAsync(0, FocusedTranslationY, AnimationDuration, Easing.CubicOut);

        var colorTask = Task.CompletedTask;
        if (entry.Parent is Border border &&
            Application.Current!.Resources.TryGetValue(PrimaryColorKey, out var primaryObj) &&
            primaryObj is Color primaryColor &&
            Application.Current.Resources.TryGetValue(BorderColorKey, out var borderObj) &&
            borderObj is Color borderColor)
        {
            var startBrush = border.Stroke as SolidColorBrush ?? new SolidColorBrush(borderColor);
            colorTask = AnimateBorderStrokeAsync(border, startBrush, primaryColor);
        }

        await Task.WhenAll(translationTask, colorTask);
    }

    private async void OnEntryUnfocused(object? sender, FocusEventArgs e)
    {
        if (sender is not Entry entry) return;

#if ANDROID
        if (entry.Handler?.PlatformView is Android.Widget.EditText nativeEditText && _originalBackground != null)
            nativeEditText.Background = _originalBackground;
#endif

        var translationTask = entry.TranslateToAsync(0, UnfocusedTranslationY, AnimationDuration, Easing.CubicOut);

        var colorTask = Task.CompletedTask;
        if (entry.Parent is Border border &&
            Application.Current!.Resources.TryGetValue(PrimaryColorKey, out var primaryObj) &&
            primaryObj is Color primaryColor &&
            Application.Current.Resources.TryGetValue(BorderColorKey, out var borderObj) &&
            borderObj is Color borderColor)
        {
            var startBrush = border.Stroke as SolidColorBrush ?? new SolidColorBrush(primaryColor);
            colorTask = AnimateBorderStrokeAsync(border, startBrush, borderColor);
        }

        await Task.WhenAll(translationTask, colorTask);
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
