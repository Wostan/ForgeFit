using System.Globalization;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Markup;
using ForgeFit.MAUI.Extensions;
using ForgeFit.MAUI.Resources.Strings;
using LocalizationResourceManager.Maui;
using Microsoft.Extensions.Logging;
using ZXing.Net.Maui.Controls;

namespace ForgeFit.MAUI;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseMauiCommunityToolkitMarkup()
            .UseBarcodeReader()
            .UseLocalizationResourceManager(settings =>
            {
                settings.AddResource(AppResources.ResourceManager);
                settings.InitialCulture(new CultureInfo("uk-UA"));
                settings.RestoreLatestCulture(true);
            })
            .ConfigureUiSettings()
            .RegisterServices()
            .RegisterViewModels()
            .RegisterViews();

#if DEBUG
        builder.Logging.AddDebug();
#endif
        
        Microsoft.Maui.Handlers.ButtonHandler.Mapper.AppendToMapping("AutoSizeText", (handler, view) =>
        {
#if ANDROID
            AndroidX.Core.Widget.TextViewCompat.SetAutoSizeTextTypeWithDefaults(
                handler.PlatformView, 
                AndroidX.Core.Widget.TextViewCompat.AutoSizeTextTypeUniform);
#elif IOS
        if (handler.PlatformView.TitleLabel != null)
        {
            handler.PlatformView.TitleLabel.AdjustsFontSizeToFitWidth = true;
            handler.PlatformView.TitleLabel.MinimumScaleFactor = 0.5f; 
        }
#endif
        });

        return builder.Build();
    }
}
