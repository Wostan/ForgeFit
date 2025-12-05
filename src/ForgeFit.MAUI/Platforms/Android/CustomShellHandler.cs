using Android.Content;
using Android.Graphics.Drawables;
using Android.Views;
using Google.Android.Material.BottomNavigation;
using Microsoft.Maui.Controls.Handlers.Compatibility;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Controls.Platform.Compatibility;
using Microsoft.Maui.Platform;
using Color = Microsoft.Maui.Graphics.Color;

namespace ForgeFit.MAUI;

public class CustomShellHandler : ShellRenderer
{
    public CustomShellHandler(Context context) : base(context)
    {
    }

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
        
        base.SetAppearance(bottomView, appearance);
        if (bottomView.Context?.Resources?.DisplayMetrics is null) return;

        bottomView.SetBackgroundColor(Android.Graphics.Color.Transparent);

        var backgroundDrawable = new GradientDrawable();
        backgroundDrawable.SetShape(ShapeType.Rectangle);
        backgroundDrawable.SetColor(appearance.EffectiveTabBarBackgroundColor.ToPlatform());

        var density = bottomView.Context.Resources.DisplayMetrics.Density;
        var cornerRadius = 16f * density;

        backgroundDrawable.SetCornerRadii([
            cornerRadius, cornerRadius,
            cornerRadius, cornerRadius,
            cornerRadius, cornerRadius,
            cornerRadius, cornerRadius
        ]);

        if (Application.Current!.Resources.TryGetValue("BorderColor", out var borderObj))
            if (borderObj is Color borderColor)
            {
                var borderWidthPx = (int)(borderWidthDp * density);
                backgroundDrawable.SetStroke(borderWidthPx, borderColor.ToPlatform());
            }

        bottomView.SetBackground(backgroundDrawable);

        bottomView.Elevation = 20;

        if (bottomView.LayoutParameters is not ViewGroup.MarginLayoutParams layoutParams) return;
            
        var marginPx = (int)(marginDp * density);
        var bottomMarginPx = (int)(bottomMarginDp * density);

        layoutParams.SetMargins(marginPx, 0, marginPx, bottomMarginPx);
        bottomView.LayoutParameters = layoutParams;
    }
}
