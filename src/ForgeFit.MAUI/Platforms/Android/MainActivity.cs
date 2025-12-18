using Android.App;
using Android.Content.PM;
using Android.OS;
using AndroidX.Core.View;
using View = Android.Views.View;
using Android.Views;
using Color = Android.Graphics.Color;

namespace ForgeFit.MAUI;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop,
    ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode |
                           ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        
        WindowCompat.SetDecorFitsSystemWindows(Window, false);

        var rootView = Window?.DecorView.FindViewById(Android.Resource.Id.Content);
        
        if (rootView != null)
        {
            ViewCompat.SetOnApplyWindowInsetsListener(rootView, new ZeroTopPaddingListener());
        }
    }

    private class ZeroTopPaddingListener : Java.Lang.Object, IOnApplyWindowInsetsListener
    {
        public WindowInsetsCompat? OnApplyWindowInsets(View? v, WindowInsetsCompat? insets)
        {
            if (insets == null || v == null) return null;

            var bars = insets.GetInsets(WindowInsetsCompat.Type.SystemBars());
            if (bars == null) return null;
            
            v.SetPadding(bars.Left, 0, bars.Right, bars.Bottom);
            v.SetBackgroundColor(Color.ParseColor("#0E0E0E"));

            return WindowInsetsCompat.Consumed;
        }
    }
}
