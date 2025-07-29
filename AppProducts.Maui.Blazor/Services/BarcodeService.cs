using AppProducts.Shared.Blazor.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppProducts.Maui.Blazor.Services
{
    public class BarcodeService : IBarcodeService
    {
        public string? BarcodeResult { get; private set; }

        // Event to notify Blazor components that a scan is complete
        public event Action<string>? OnBarcodeScanned;

        public void SetBarcodeResult(string result)
        {
            BarcodeResult = result;
            // Notify subscribers with the barcode value
            OnBarcodeScanned?.Invoke(result);
        }

        public void Reset()
        {
            BarcodeResult = null;
        }
    }
}