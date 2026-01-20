using System.Globalization;

namespace ForgeFit.MAUI.Behaviors;

public class DurationInputBehavior : Behavior<Entry>
{
    public static readonly BindableProperty DurationProperty =
        BindableProperty.Create(nameof(Duration),
            typeof(TimeSpan), 
            typeof(DurationInputBehavior), 
            TimeSpan.Zero, 
            BindingMode.TwoWay, 
            propertyChanged: OnDurationChanged);

    public TimeSpan Duration
    {
        get => (TimeSpan)GetValue(DurationProperty);
        set => SetValue(DurationProperty, value);
    }

    private Entry? _entry;

    protected override void OnAttachedTo(Entry entry)
    {
        base.OnAttachedTo(entry);
        _entry = entry;
        _entry.Unfocused += OnUnfocused;
        _entry.BindingContextChanged += OnBindingContextChanged;
        
        UpdateEntryText();
    }

    protected override void OnDetachingFrom(Entry entry)
    {
        base.OnDetachingFrom(entry);
        if (_entry != null)
        {
            _entry.Unfocused -= OnUnfocused;
            _entry.BindingContextChanged -= OnBindingContextChanged;
        }
        _entry = null;
    }

    private void OnBindingContextChanged(object? sender, EventArgs e)
    {
        UpdateEntryText();
    }

    private static void OnDurationChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var behavior = (DurationInputBehavior)bindable;
        behavior.UpdateEntryText();
    }

    private void UpdateEntryText()
    {
        if (_entry == null || _entry.IsFocused) return;
        
        var formatted = FormatDuration(Duration);
        if (_entry.Text != formatted)
        {
            _entry.Text = formatted;
        }
    }

    private void OnUnfocused(object? sender, FocusEventArgs e)
    {
        if (_entry == null) return;

        if (TryParseDuration(_entry.Text, out var result))
        {
            Duration = result;
            _entry.Text = FormatDuration(result);
        }
        else
        {
            _entry.Text = FormatDuration(Duration);
        }
    }

    private bool TryParseDuration(string? input, out TimeSpan result)
    {
        result = TimeSpan.Zero;
        if (string.IsNullOrWhiteSpace(input)) return false;

        input = input.Trim();

        if (input.Contains(':') && TimeSpan.TryParseExact(input, ["h\\:mm", "hh\\:mm", "h\\:m"], CultureInfo.InvariantCulture, out result))
            return true;

        if (!double.TryParse(input, out var minutes)) return false;
        
        result = TimeSpan.FromMinutes(minutes);
        return true;

    }

    private static string FormatDuration(TimeSpan ts)
    {
        return $"{(int)ts.TotalHours}:{ts.Minutes:D2}";
    }
}
