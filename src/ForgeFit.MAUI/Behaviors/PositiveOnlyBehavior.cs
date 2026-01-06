using Microsoft.Maui.Controls;

namespace ForgeFit.MAUI.Behaviors;

public class PositiveOnlyBehavior : Behavior<Entry>
{
    protected override void OnAttachedTo(Entry entry)
    {
        base.OnAttachedTo(entry);
        entry.TextChanged += OnTextChanged;
    }

    protected override void OnDetachingFrom(Entry entry)
    {
        base.OnDetachingFrom(entry);
        entry.TextChanged -= OnTextChanged;
    }

    private bool _isInternalChange;

    private void OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (_isInternalChange) return;
        if (sender is not Entry entry) return;

        var newText = e.NewTextValue;
        if (string.IsNullOrEmpty(newText)) return;
        
        var sanitized = newText.Replace(',', '.').Replace("-", "");

        var cleanChars = new List<char>(sanitized.Length);
        var hasDot = false;

        foreach (var c in sanitized)
        {
            if (char.IsDigit(c))
            {
                cleanChars.Add(c);
            }
            else if (c == '.')
            {
                if (hasDot) continue;
                
                cleanChars.Add('.');
                hasDot = true;
            }
        }

        var result = new string(cleanChars.ToArray());

        if (result.StartsWith('.'))
            result = "0" + result;

        if (result == newText) return;

        _isInternalChange = true;
        entry.Text = result;
        
        entry.CursorPosition = result.Length;
        _isInternalChange = false;
    }
}
