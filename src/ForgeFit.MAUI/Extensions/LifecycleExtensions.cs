using Microsoft.Maui.LifecycleEvents;

namespace ForgeFit.MAUI.Extensions;

public static class LifecycleExtensions
{
    public static MauiAppBuilder ConfigureLifecycle(this MauiAppBuilder builder)
    {
        builder.ConfigureLifecycleEvents(events =>
        {
#if ANDROID
            events.AddAndroid(android => android.OnCreate((activity, _) => MakeStatusBarTranslucent(activity)));
#endif
        });

        return builder;
    }

#if ANDROID
    private static void MakeStatusBarTranslucent(Android.App.Activity activity)
    {
        activity.Window!.SetFlags(
            Android.Views.WindowManagerFlags.LayoutNoLimits,
            Android.Views.WindowManagerFlags.LayoutNoLimits);

        activity.Window.ClearFlags(Android.Views.WindowManagerFlags.TranslucentStatus);
        activity.Window.ClearFlags(Android.Views.WindowManagerFlags.TranslucentNavigation);

        activity.Window.DecorView.SetBackgroundColor(Android.Graphics.Color.Black);

#pragma warning disable CA1422
        activity.Window.SetStatusBarColor(Android.Graphics.Color.Transparent);
        activity.Window.SetNavigationBarColor(Android.Graphics.Color.Transparent);
#pragma warning restore CA1422
    }
#endif
}
