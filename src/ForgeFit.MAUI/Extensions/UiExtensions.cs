using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Markup;
using Sharpnado.MaterialFrame;

namespace ForgeFit.MAUI.Extensions;

public static class UiExtensions
{
    public static MauiAppBuilder ConfigureUiSettings(this MauiAppBuilder builder)
    {
        builder
            .UseSharpnadoMaterialFrame(false)
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("Montserrat-Regular.ttf", "MontserratRegular");
                fonts.AddFont("Montserrat-SemiBold.ttf", "MontserratSemiBold");
            })
            .ConfigureMauiHandlers(handlers =>
            {
#if ANDROID
                handlers.AddHandler<Shell, CustomShellHandler>();
#endif
            });

        return builder;
    }
}
