namespace AppProducts.Shared.Blazor.Services
{
    public interface IBarcodeService
    {
        string? BarcodeResult { get; }
        event Action<string>? OnBarcodeScanned;
        void SetBarcodeResult(string result);
        void Reset();
    }
}
