namespace AppProducts.Shared.Blazor.Services
{
    public interface IBarcodeScannerService
    {
        Task<string?> ScanBarcodeAsync();
    }
}
