using Microsoft.Extensions.Logging;
using PrivateChatAI.Pages;
using PrivateChatAI.ViewModels;

namespace PrivateChatAI
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Register ViewModels
            builder.Services.AddTransient<ChatViewModel>();

            // Register Pages
            builder.Services.AddTransient<ChatPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
