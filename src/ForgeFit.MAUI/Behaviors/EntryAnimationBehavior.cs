#if ANDROID
using Android.Graphics.Drawables;
#endif

namespace ForgeFit.MAUI.Behaviors;

public class EntryAnimationBehavior : Behavior<Entry>
{
    public static readonly BindableProperty IsAnimatedProperty =
        BindableProperty.CreateAttached(
            "IsAnimated",
            typeof(bool), typeof(EntryAnimationBehavior),
            false,
            propertyChanged: OnPropertyChanged);

    public static readonly BindableProperty IsErrorProperty =
        BindableProperty.CreateAttached(
            "IsError",
            typeof(bool), typeof(EntryAnimationBehavior),
            false,
            propertyChanged: OnPropertyChanged);

    public static bool GetIsAnimated(BindableObject view)
    {
        return (bool)view.GetValue(IsAnimatedProperty);
    }

    public static void SetIsAnimated(BindableObject view, bool value)
    {
        view.SetValue(IsAnimatedProperty, value);
    }

    public static bool GetIsError(BindableObject view)
    {
        return (bool)view.GetValue(IsErrorProperty);
    }

    public static void SetIsError(BindableObject view, bool value)
    {
        view.SetValue(IsErrorProperty, value);
    }

    private const uint AnimationDuration = 250;
    private const int AnimationRate = 16;
    private const double FocusedY = 3.0;
    private const double UnfocusedY = 0.0;

    private const string StrokeAnimationName = "StrokeColorAnim";
    private const string PrimaryKey = "Primary";
    private const string BorderKey = "BorderColor";
    private const string ErrorKey = "Error";

#if ANDROID
    private Drawable? _originalBackground;
#endif

    private static void OnPropertyChanged(BindableObject view, object oldValue, object newValue)
    {
        if (view is not Entry entry) return;

        if (!entry.Behaviors.Any(b => b is EntryAnimationBehavior))
        {
            entry.Behaviors.Add(new EntryAnimationBehavior());
        }
        else
        {
            var behavior = entry.Behaviors.FirstOrDefault(b => b is EntryAnimationBehavior) as EntryAnimationBehavior;
            behavior?.UpdateVisualState(entry);
        }
    }

    protected override void OnAttachedTo(Entry entry)
    {
        base.OnAttachedTo(entry);
        entry.Focused += OnFocusChanged;
        entry.Unfocused += OnFocusChanged;
        UpdateVisualState(entry);
    }

    protected override void OnDetachingFrom(Entry entry)
    {
        entry.Focused -= OnFocusChanged;
        entry.Unfocused -= OnFocusChanged;
        base.OnDetachingFrom(entry);
    }

    private void OnFocusChanged(object? sender, FocusEventArgs e)
    {
        if (sender is Entry entry) UpdateVisualState(entry);
    }

    private async void UpdateVisualState(Entry entry)
    {
        var isFocused = entry.IsFocused;
        var isError = GetIsError(entry);
        var isAnimated = GetIsAnimated(entry);

#if ANDROID
        if (entry.Handler?.PlatformView is Android.Widget.EditText nativeEditText)
        {
            if (isFocused)
            {
                _originalBackground ??= nativeEditText.Background;
                nativeEditText.Background = new ColorDrawable(Android.Graphics.Color.Transparent);
            }
            else if (_originalBackground != null)
            {
                nativeEditText.Background = _originalBackground;
            }
        }
#endif

        var translateTask = Task.CompletedTask;
        var borderStrokeTask = Task.CompletedTask;


        if (isAnimated)
        {
            var targetY = isFocused ? FocusedY : UnfocusedY;
            translateTask = entry.TranslateToAsync(0, targetY, AnimationDuration, Easing.CubicOut);
        }

        if (entry.Parent is Border border)
        {
            string colorKey;
            var startBrush = border.Stroke as SolidColorBrush ?? new SolidColorBrush(Colors.White);

            if (isError) colorKey = ErrorKey;
            else colorKey = isFocused ? PrimaryKey : BorderKey;

            if (Application.Current!.Resources.TryGetValue(colorKey, out var colorObj) && colorObj is Color targetColor)
                borderStrokeTask = AnimateBorderStrokeAsync(border, startBrush, targetColor);
        }

        await Task.WhenAll(translateTask, borderStrokeTask);
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
