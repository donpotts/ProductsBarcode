using AppProducts.Maui.Blazor.Services;
using AppProducts.Shared.Blazor;
using AppProducts.Shared.Blazor.Services;
using SkiaSharp.Views.Maui.Controls.Hosting;
using CommunityToolkit.Maui;
using Plugin.Maui.Audio;
using ZXing.Net.Maui.Controls;

namespace AppProducts.Maui.Blazor;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseSkiaSharp()
            .UseMauiCommunityToolkit()
            .UseBarcodeReader()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        builder.Services.AddMauiBlazorWebView();
        builder.Services.AddSingleton(AudioManager.Current);
        
#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
#endif

        builder.Services.AddBlazorServices("https://appproducts-g6d6dpghgebmdkdn.eastus-01.azurewebsites.net/");
        builder.Services.AddSingleton<IStorageService, MauiStorageService>();
        builder.Services.AddSingleton<IBarcodeService, BarcodeService>();
        builder.Services.AddSingleton<BarcodeService>();
        builder.Services.AddSingleton<IBarcodeScannerService, MauiBarcodeScannerService>();
        builder.Services.AddSingleton<ICustomPopupService, MauiPopupService>();

        return builder.Build();
    }
}
