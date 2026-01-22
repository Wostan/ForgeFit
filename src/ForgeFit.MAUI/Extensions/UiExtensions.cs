namespace ForgeFit.MAUI.Extensions;

public static class UiExtensions
{
    public static MauiAppBuilder ConfigureUiSettings(this MauiAppBuilder builder)
    {
        builder
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("Montserrat-Regular.ttf", "MontserratRegular");
                fonts.AddFont("Montserrat-SemiBold.ttf", "MontserratSemiBold");
            })
            .ConfigureMauiHandlers(handlers =>
            {
#if ANDROID
                handlers.AddHandler<Shell, ShellHandler>();
#endif
            });

        return builder;
    }
}
