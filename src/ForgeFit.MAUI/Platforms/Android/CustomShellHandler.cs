using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Views;
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
    public override void SetAppearance(BottomNavigationView bottomView, IShellAppearanceElement appearance)
    {
        const float borderWidthDp = 2f;
        const int marginDp = 20;
        const int bottomMarginDp = 20;

        const float shadowRadiusDp = 4f;
        const float shadowOpacity = 0.6f;

        base.SetAppearance(bottomView, appearance);
        if (bottomView.Context?.Resources?.DisplayMetrics is null) return;

        var density = bottomView.Context.Resources.DisplayMetrics.Density;

        bottomView.SetBackgroundColor(Android.Graphics.Color.Transparent);

        var backgroundDrawable = new GradientDrawable();
        backgroundDrawable.SetShape(ShapeType.Rectangle);
        backgroundDrawable.SetColor(appearance.EffectiveTabBarBackgroundColor.ToPlatform());

        var cornerRadius = 16f * density;
        backgroundDrawable.SetCornerRadii([
            cornerRadius, cornerRadius,
            cornerRadius, cornerRadius,
            cornerRadius, cornerRadius,
            cornerRadius, cornerRadius
        ]);

        var targetBorderColor = Colors.Black;

        if (Application.Current!.Resources.TryGetValue("BorderColor", out var borderObj) && borderObj is Color foundColor)
        {
            targetBorderColor = foundColor;

            var borderWidthPx = (int)(borderWidthDp * density);
            backgroundDrawable.SetStroke(borderWidthPx, targetBorderColor.ToPlatform());
        }

        bottomView.SetBackground(backgroundDrawable);
        
        bottomView.Elevation = shadowRadiusDp * density;

        if (Build.VERSION.SdkInt >= BuildVersionCodes.P)
        {
            var shadowColor = targetBorderColor.WithAlpha(shadowOpacity).ToPlatform();
            
#pragma warning disable CA1416
            bottomView.SetOutlineAmbientShadowColor(shadowColor);
            bottomView.SetOutlineSpotShadowColor(shadowColor);
#pragma warning restore CA1416
        }

        bottomView.OutlineProvider = new RoundedOutlineProvider(cornerRadius);
        bottomView.ClipToOutline = true;

        if (bottomView.LayoutParameters is not ViewGroup.MarginLayoutParams layoutParams) return;
        var marginPx = (int)(marginDp * density);
        var bottomMarginPx = (int)(bottomMarginDp * density);

        layoutParams.SetMargins(marginPx, 0, marginPx, bottomMarginPx);
        bottomView.LayoutParameters = layoutParams;
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
