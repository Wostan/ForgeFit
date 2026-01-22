using System.Globalization;
using Microsoft.Maui.Controls;

namespace ForgeFit.MAUI.Behaviors;

public class NumericClampBehavior : Behavior<Entry>
{
    public double MaxValue { get; set; } = double.MaxValue;
    public double MinValue { get; set; } = 0;
    public int Decimals { get; set; } = 0;

    private Entry? _entry;

    protected override void OnAttachedTo(Entry entry)
    {
        _entry = entry;
        _entry.Unfocused += OnUnfocused;
        _entry.TextChanged += OnTextChanged;
        base.OnAttachedTo(entry);
    }

    protected override void OnDetachingFrom(Entry entry)
    {
        if (_entry != null)
        {
            _entry.Unfocused -= OnUnfocused;
            _entry.TextChanged -= OnTextChanged;
        }

        _entry = null;
        base.OnDetachingFrom(entry);
    }

    private void OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrEmpty(e.NewTextValue)) return;

        var isValid =
            Decimals == 0
                ? int.TryParse(e.NewTextValue, out _)
                : double.TryParse(e.NewTextValue.Replace(',', '.'), out _);

        if (!isValid && e.NewTextValue != "." && e.NewTextValue != ",") ((Entry)sender!).Text = e.OldTextValue;
    }

    private void OnUnfocused(object? sender, FocusEventArgs e)
    {
        if (_entry == null || string.IsNullOrWhiteSpace(_entry.Text))
        {
            _entry?.Text = MinValue.ToString(CultureInfo.InvariantCulture);
            return;
        }

        if (!double.TryParse(_entry.Text.Replace(',', '.'), out var value)) return;

        var clamped = Math.Clamp(value, MinValue, MaxValue);

        clamped = Math.Round(clamped, Decimals);

        if (Math.Abs(value - clamped) > 0.001)
            _entry.Text = clamped.ToString(CultureInfo.InvariantCulture);
    }
}
