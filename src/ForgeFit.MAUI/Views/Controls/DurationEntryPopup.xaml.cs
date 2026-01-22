using System.Windows.Input;

namespace ForgeFit.MAUI.Views.Controls;

public partial class DurationEntryPopup : ContentView
{
    private bool _isUpdatingUi;

    public DurationEntryPopup()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty TitleProperty = BindableProperty.Create(
        nameof(Title), typeof(string), typeof(DurationEntryPopup), string.Empty);

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly BindableProperty DurationProperty = BindableProperty.Create(
        nameof(Duration), 
        typeof(TimeSpan), 
        typeof(DurationEntryPopup), 
        TimeSpan.Zero, 
        BindingMode.TwoWay, 
        propertyChanged: OnDurationChanged);

    public TimeSpan Duration
    {
        get => (TimeSpan)GetValue(DurationProperty);
        set => SetValue(DurationProperty, value);
    }

    public static readonly BindableProperty SaveCommandProperty = BindableProperty.Create(
        nameof(SaveCommand), typeof(ICommand), typeof(DurationEntryPopup));

    public ICommand SaveCommand
    {
        get => (ICommand)GetValue(SaveCommandProperty);
        set => SetValue(SaveCommandProperty, value);
    }

    public static readonly BindableProperty CancelCommandProperty = BindableProperty.Create(
        nameof(CancelCommand), typeof(ICommand), typeof(DurationEntryPopup));

    public ICommand CancelCommand
    {
        get => (ICommand)GetValue(CancelCommandProperty);
        set => SetValue(CancelCommandProperty, value);
    }

    private static void OnDurationChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is DurationEntryPopup popup && newValue is TimeSpan ts)
        {
            popup.UpdateEntriesFromDuration(ts);
        }
    }

    private void UpdateEntriesFromDuration(TimeSpan ts)
    {
        if (HoursEntry.IsFocused || MinutesEntry.IsFocused) return;

        _isUpdatingUi = true;
        
        HoursEntry.Text = Math.Floor(ts.TotalHours).ToString("0");
        MinutesEntry.Text = ts.Minutes.ToString("00");
        
        _isUpdatingUi = false;
    }

    private void OnTimePartsChanged(object sender, TextChangedEventArgs e)
    {
        if (_isUpdatingUi) return;

        if (int.TryParse(HoursEntry.Text, out var h) && int.TryParse(MinutesEntry.Text, out var m))
        {
            var newDuration = new TimeSpan(h, m, 0);
            
            if (Duration != newDuration)
            {
                Duration = newDuration;
            }
        }
        else
        {
            var hours = int.TryParse(HoursEntry.Text, out var hVal) ? hVal : 0;
            var mins = int.TryParse(MinutesEntry.Text, out var mVal) ? mVal : 0;
            Duration = new TimeSpan(hours, mins, 0);
        }
    }
}
