using CommunityToolkit.Mvvm.ComponentModel;

namespace ForgeFit.MAUI.Models;

public partial class SelectableItem<T>(T value, string name) : ObservableObject
{
    public T Value { get; } = value;
    public string Name { get; } = name;

    [ObservableProperty] private bool _isSelected;
}
