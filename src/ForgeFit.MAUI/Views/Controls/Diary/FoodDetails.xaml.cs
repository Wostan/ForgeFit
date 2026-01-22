using System.Runtime.CompilerServices;

namespace ForgeFit.MAUI.Views.Controls.Diary;

public partial class FoodDetails : ContentView
{
    public FoodDetails()
    {
        InitializeComponent();

        Unloaded += OnUnloaded;
    }

    protected override void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        if (propertyName == nameof(IsVisible)) UpdatePickerState();
    }

    private void UpdatePickerState()
    {
        if (IsVisible)
        {
            UnitPicker.IsEnabled = true;
        }
        else
        {
            UnitPicker.Unfocus();
            UnitPicker.IsEnabled = false;
        }
    }

    private void OnUnloaded(object? sender, EventArgs e)
    {
        UnitPicker.Unfocus();
        UnitPicker.IsEnabled = false;
    }
}
