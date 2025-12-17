#if ANDROID
using Android.Content;
using Android.Graphics.Drawables;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using Google.Android.Material.BottomNavigation;
using Microsoft.Maui.Controls.Handlers.Compatibility;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Controls.Platform.Compatibility;
using Microsoft.Maui.Platform;
using Color = Microsoft.Maui.Graphics.Color;

namespace ForgeFit.MAUI;

public class ShellHandler(Context context) : ShellRenderer(context)
{
    protected override IShellBottomNavViewAppearanceTracker CreateBottomNavViewAppearanceTracker(ShellItem shellItem)
    {
        return new BorderedBottomNavTracker(this, shellItem);
    }
}

public class BorderedBottomNavTracker(IShellContext shellContext, ShellItem shellItem)
    : ShellBottomNavViewAppearanceTracker(shellContext, shellItem)
{
    public override void SetAppearance(BottomNavigationView bottomView, IShellAppearanceElement appearance)
    {
        base.SetAppearance(bottomView, appearance);

        var backgroundColor = appearance.EffectiveTabBarBackgroundColor.ToPlatform();
        var borderColor = GetThemeColor("BorderColor")?.ToPlatform() ?? Android.Graphics.Color.ParseColor("#333333");
        
        bottomView.SetBackground(CreateTopBorderBackground(backgroundColor, borderColor, bottomView.Context));

        ApplyCustomFont(bottomView, "Montserrat-SemiBold.ttf");
    }

    private LayerDrawable? CreateTopBorderBackground(Android.Graphics.Color bgColor, Android.Graphics.Color borderColor, Context? context)
    {
        if (context?.Resources?.DisplayMetrics == null) return null;
        var density = context.Resources.DisplayMetrics.Density;
        var borderWidthPx = (int)(2 * density);

        var borderDrawable = new ColorDrawable(borderColor);
        var backgroundDrawable = new ColorDrawable(bgColor);

        var layers = new Drawable[] { borderDrawable, backgroundDrawable };
        var layerDrawable = new LayerDrawable(layers);
        
        layerDrawable.SetLayerInset(1, 0, borderWidthPx, 0, 0);

        return layerDrawable;
    }

    private void ApplyCustomFont(ViewGroup viewGroup, string fontName)
    {
        var context = viewGroup.Context;
        Typeface? typeface;
        try
        {
            typeface = Typeface.CreateFromAsset(context?.Assets, fontName);
        }
        catch
        {
            return;
        }

        for (var i = 0; i < viewGroup.ChildCount; i++)
        {
            var child = viewGroup.GetChildAt(i);
            if (child is ViewGroup childGroup)
                ApplyCustomFont(childGroup, fontName);
            else if (child is TextView textView) 
                textView.Typeface = typeface;
        }
    }
    
    private static Color? GetThemeColor(string key)
    {
        if (Application.Current!.Resources.TryGetValue(key, out var obj) && obj is Color color) return color;
        return null;
    }
}
#endif
