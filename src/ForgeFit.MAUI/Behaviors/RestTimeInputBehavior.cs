using System.Globalization;
using ForgeFit.MAUI.Constants;

namespace ForgeFit.MAUI.Behaviors;

public class RestTimeInputBehavior : Behavior<Entry>
{
    private static readonly TimeSpan MaxTime = TimeSpan.FromMinutes(AppConstants.ValidationLimits.MaxRestTimeMinutes);

    public static readonly BindableProperty TimeProperty =
        BindableProperty.Create(nameof(Time),
            typeof(TimeSpan),
            typeof(RestTimeInputBehavior),
            TimeSpan.Zero,
            BindingMode.TwoWay,
            propertyChanged: OnTimeChanged);

    private Entry? _entry;

    public TimeSpan Time
    {
        get => (TimeSpan)GetValue(TimeProperty);
        set => SetValue(TimeProperty, value);
    }

    protected override void OnAttachedTo(Entry entry)
    {
        base.OnAttachedTo(entry);
        _entry = entry;
        _entry.TextChanged += OnTextChanged;
        _entry.Unfocused += OnUnfocused;
        _entry.BindingContextChanged += OnBindingContextChanged;

        UpdateEntryText();
    }

    protected override void OnDetachingFrom(Entry entry)
    {
        base.OnDetachingFrom(entry);
        if (_entry != null)
        {
            _entry.TextChanged -= OnTextChanged;
            _entry.Unfocused -= OnUnfocused;
            _entry.BindingContextChanged -= OnBindingContextChanged;
        }

        _entry = null;
    }

    private void OnBindingContextChanged(object? sender, EventArgs e)
    {
        UpdateEntryText();
    }

    private static void OnTimeChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var behavior = (RestTimeInputBehavior)bindable;
        behavior.UpdateEntryText();
    }

    private void UpdateEntryText()
    {
        if (_entry == null || _entry.IsFocused) return;

        var safeTime = Time > MaxTime ? MaxTime : Time;
        var formatted = FormatTime(safeTime);

        if (_entry.Text != formatted) _entry.Text = formatted;
    }

    private void OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(e.NewTextValue)) return;
        if (!TryParseTime(e.NewTextValue, out var result)) return;

        if (result > MaxTime) result = MaxTime;
        if (Time != result) Time = result;
    }

    private void OnUnfocused(object? sender, FocusEventArgs e)
    {
        UpdateEntryText();
    }

    private bool TryParseTime(string input, out TimeSpan result)
    {
        input = input.Trim();
        if (input.Contains(':') && TimeSpan.TryParseExact(input, ["m\\:ss", "mm\\:ss", "m\\:s"],
                CultureInfo.InvariantCulture, out result))
            return true;

        if (double.TryParse(input, out var seconds))
        {
            result = TimeSpan.FromSeconds(seconds);
            return true;
        }

        result = TimeSpan.Zero;
        return false;
    }

    private static string FormatTime(TimeSpan ts)
    {
        return $"{(int)ts.TotalMinutes:D2}:{ts.Seconds:D2}";
    }
}
