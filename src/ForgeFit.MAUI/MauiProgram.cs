using System.Globalization;
using BarcodeScanner.Mobile;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Markup;
using ForgeFit.MAUI.Resources.Strings;
using Microsoft.Extensions.Logging;
using ForgeFit.MAUI.Extensions;
using LocalizationResourceManager.Maui;

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
            .UseMauiCommunityToolkitCamera()
            .UseLocalizationResourceManager(settings =>
            {
                settings.AddResource(AppResources.ResourceManager);
                settings.InitialCulture(new CultureInfo("uk-UA"));
                settings.RestoreLatestCulture(true);
            })
            .ConfigureMauiHandlers(handlers =>
            {
                handlers.AddBarcodeScannerHandler();
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
