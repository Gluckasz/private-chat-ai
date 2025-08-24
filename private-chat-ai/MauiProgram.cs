using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Handlers;
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

#if ANDROID
            EntryHandler.Mapper.AppendToMapping(
                "BackgroundColor",
                (handler, view) =>
                {
                    if (
                        handler.PlatformView is AndroidX.AppCompat.Widget.AppCompatEditText editText
                    )
                    {
                        editText.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(
                            Android.Graphics.Color.Transparent
                        );
                    }
                }
            );

            EditorHandler.Mapper.AppendToMapping(
                "BackgroundColor",
                (handler, view) =>
                {
                    if (
                        handler.PlatformView is AndroidX.AppCompat.Widget.AppCompatEditText editText
                    )
                    {
                        editText.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(
                            Android.Graphics.Color.Transparent
                        );
                    }
                }
            );
#endif

            builder.Services.AddTransient<ChatViewModel>();
            builder.Services.AddTransient<SettingsViewModel>();

            builder.Services.AddTransient<ChatPage>();
            builder.Services.AddTransient<SettingsPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
