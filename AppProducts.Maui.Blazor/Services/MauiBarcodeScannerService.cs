using AppProducts.Shared.Blazor.Services;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace AppProducts.Maui.Blazor.Services
{
    public class MauiBarcodeScannerService : IBarcodeScannerService
    {
        public async Task<string?> ScanBarcodeAsync()
        {
            var mainPage = Application.Current?.MainPage as NavigationPage;
            if (mainPage == null)
                return null;

            var tcs = new TaskCompletionSource<string?>();
            var barcodeService = mainPage.Handler.MauiContext.Services.GetService(typeof(BarcodeService)) as BarcodeService;
            var scannerPage = new AppProducts.Maui.Blazor.Pages.ScannerPage(barcodeService!);

            void Handler(string barcode)
            {
                tcs.TrySetResult(barcode);
                barcodeService!.OnBarcodeScanned -= Handler;
                mainPage.Navigation.PopAsync();
            }
            barcodeService!.OnBarcodeScanned += Handler;
            await mainPage.Navigation.PushAsync(scannerPage);
            return await tcs.Task;
        }
    }
}
