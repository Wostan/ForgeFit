using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Views;
using Android.Widget;
using Google.Android.Material.BottomNavigation;
using Microsoft.Maui.Controls.Handlers.Compatibility;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Controls.Platform.Compatibility;
using Microsoft.Maui.Platform;
using Color = Microsoft.Maui.Graphics.Color;
using View = Android.Views.View;

namespace ForgeFit.MAUI;

public class CustomShellHandler(Context context) : ShellRenderer(context)
{
    protected override IShellBottomNavViewAppearanceTracker CreateBottomNavViewAppearanceTracker(ShellItem shellItem)
    {
        return new FloatingTabBarAppearanceTracker(this, shellItem);
    }
}

public class FloatingTabBarAppearanceTracker(IShellContext shellContext, ShellItem shellItem)
    : ShellBottomNavViewAppearanceTracker(shellContext, shellItem)
{
    private const float BorderWidthDp = 2f;
    private const int MarginHorizontalDp = 20;
    private const int MarginBottomDp = 20;
    private const float CornerRadiusDp = 16f;
    private const float ShadowRadiusDp = 4f;
    private const float ShadowOpacity = 0.6f;

    public override void SetAppearance(BottomNavigationView bottomView, IShellAppearanceElement appearance)
    {
        base.SetAppearance(bottomView, appearance);

        if (bottomView.Context?.Resources?.DisplayMetrics is null) return;
        var density = bottomView.Context.Resources.DisplayMetrics.Density;

        var backgroundColor = appearance.EffectiveTabBarBackgroundColor.ToPlatform();
        var borderColor = GetThemeColor("BorderColor") ?? Colors.Black;
        var cornerRadiusPx = DpToPx(CornerRadiusDp, density);

        bottomView.SetBackgroundColor(Android.Graphics.Color.Transparent);
        bottomView.SetBackground(CreateFloatingBackground(backgroundColor, borderColor, density, cornerRadiusPx));

        ApplyCustomFont(bottomView, "Montserrat-SemiBold.ttf");
        SetupElevationAndShadows(bottomView, borderColor, density, cornerRadiusPx);
        ApplyMargins(bottomView, density);
    }

    private static GradientDrawable CreateFloatingBackground(Android.Graphics.Color bgColor, Color borderColor,
        float density, float radiusPx)
    {
        var drawable = new GradientDrawable();
        drawable.SetShape(ShapeType.Rectangle);
        drawable.SetColor(bgColor);

        drawable.SetCornerRadii([
            radiusPx, radiusPx, radiusPx, radiusPx,
            radiusPx, radiusPx, radiusPx, radiusPx
        ]);

        var strokeWidthPx = (int)(BorderWidthDp * density);
        drawable.SetStroke(strokeWidthPx, borderColor.ToPlatform());

        return drawable;
    }

    private static void ApplyCustomFont(ViewGroup viewGroup, string fontName)
    {
        var context = viewGroup.Context;
        Typeface? typeface;
        try
        {
            typeface = Typeface.CreateFromAsset(context?.Assets, fontName);
        }
        catch (Exception)
        {
            return;
        }

        for (var i = 0; i < viewGroup.ChildCount; i++)
        {
            var child = viewGroup.GetChildAt(i);
            if (child is ViewGroup childGroup)
                ApplyCustomFont(childGroup, fontName);
            else if (child is TextView textView) textView.Typeface = typeface;
        }
    }

    private static void SetupElevationAndShadows(BottomNavigationView view, Color shadowBaseColor, float density,
        float radiusPx)
    {
        view.Elevation = ShadowRadiusDp * density;

        if (Build.VERSION.SdkInt >= BuildVersionCodes.P)
        {
            var shadowColor = shadowBaseColor.WithAlpha(ShadowOpacity).ToPlatform();

#pragma warning disable CA1416
            view.SetOutlineAmbientShadowColor(shadowColor);
            view.SetOutlineSpotShadowColor(shadowColor);
#pragma warning restore CA1416
        }

        view.OutlineProvider = new RoundedOutlineProvider(radiusPx);
        view.ClipToOutline = true;
    }

    private static void ApplyMargins(BottomNavigationView view, float density)
    {
        if (view.LayoutParameters is not ViewGroup.MarginLayoutParams layoutParams) return;

        var marginPx = DpToPx(MarginHorizontalDp, density);
        var bottomMarginPx = DpToPx(MarginBottomDp, density);

        layoutParams.SetMargins(marginPx, 0, marginPx, bottomMarginPx);
        view.LayoutParameters = layoutParams;
    }

    private static Color? GetThemeColor(string key)
    {
        if (Application.Current!.Resources.TryGetValue(key, out var obj) && obj is Color color) return color;
        return null;
    }

    private static float DpToPx(float dp, float density)
    {
        return dp * density;
    }

    private static int DpToPx(int dp, float density)
    {
        return (int)(dp * density);
    }

    private class RoundedOutlineProvider(float radius) : ViewOutlineProvider
    {
        public override void GetOutline(View? view, Outline? outline)
        {
            if (view is null || outline is null) return;
            outline.SetRoundRect(0, 0, view.Width, view.Height, radius);
            outline.Alpha = 1.0f;
        }
    }
}
