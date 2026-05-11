using System.Globalization;
using AndroidX.Core.Widget;
using BarcodeScanner.Mobile;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Markup;
using ForgeFit.MAUI.Extensions;
using ForgeFit.MAUI.Handlers;
using ForgeFit.MAUI.Resources.Strings;
using ForgeFit.MAUI.Views.Controls;
using LocalizationResourceManager.Maui;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Handlers;

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
                handlers.AddHandler<NoSwipeCarouselView, NoSwipeCarouselViewHandler>();
            })
            .ConfigureUiSettings()
            .RegisterServices()
            .RegisterViewModels()
            .RegisterViews();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        ButtonHandler.Mapper.AppendToMapping("AutoSizeText", (handler, view) =>
        {
#if ANDROID
            TextViewCompat.SetAutoSizeTextTypeWithDefaults(
                handler.PlatformView,
                TextViewCompat.AutoSizeTextTypeUniform);
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
