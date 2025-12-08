using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Markup;
using ForgeFit.MAUI.Extensions;
using ForgeFit.MAUI.Resources.Strings;
using LocalizationResourceManager.Maui; // Не забудьте подключить namespace
using Microsoft.Extensions.Logging;

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
            .UseLocalizationResourceManager(settings =>
            {
                settings.AddResource(AppResources.ResourceManager);
                settings.RestoreLatestCulture(true); 
            })
            .ConfigureUiSettings()
            .RegisterServices()
            .RegisterViewModels()
            .RegisterViews()
            .ConfigureLifecycle();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
