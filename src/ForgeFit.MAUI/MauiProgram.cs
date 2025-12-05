using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Markup;
using ForgeFit.MAUI.Extensions; // Не забудьте подключить namespace
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
