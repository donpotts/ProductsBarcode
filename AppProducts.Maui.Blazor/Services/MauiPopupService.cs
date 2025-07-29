using AppProducts.Shared.Blazor.Services;
using Microsoft.Maui.Controls;
using System.Threading.Tasks;

namespace AppProducts.Maui.Blazor.Services
{
    public class MauiPopupService : ICustomPopupService
    {
        private readonly BarcodeService _barcodeService;

        public MauiPopupService(BarcodeService barcodeService)
        {
            _barcodeService = barcodeService;
        }

        public async Task<string?> ShowPopup(string message)
        {
            // Use MainPage.Instance to get the navigation context
            var mainPage = AppProducts.Maui.Blazor.MainPage.Instance;
            if (mainPage == null)
                return null;

            var tcs = new TaskCompletionSource<string?>();
            var scannerPage = new AppProducts.Maui.Blazor.Pages.ScannerPage(_barcodeService);

            void Handler(string barcode)
            {
                tcs.TrySetResult(barcode);
                _barcodeService.OnBarcodeScanned -= Handler;
                mainPage.Navigation.PopAsync();
            }
            _barcodeService.OnBarcodeScanned += Handler;
            await mainPage.Navigation.PushAsync(scannerPage);
            return await tcs.Task;
        }

        public async Task<AppPlatform> GetPlatform()
        {
            return AppPlatform.Maui;
        }
    }
}
