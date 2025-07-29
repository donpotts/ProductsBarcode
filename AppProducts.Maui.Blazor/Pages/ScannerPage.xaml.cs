using AppProducts.Maui.Blazor.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZXing;
using ZXing.Net.Maui;
using Microsoft.Maui.Controls;
using Plugin.Maui.Audio;

namespace AppProducts.Maui.Blazor.Pages
{
    public partial class ScannerPage : ContentPage
    {
        private readonly BarcodeService _barcodeService;
        private IAudioPlayer? _player;

        public ScannerPage(BarcodeService barcodeService)
        {
            InitializeComponent();
            _barcodeService = barcodeService;

            // Configure scanner options if needed
            barcodeReader.Options = new BarcodeReaderOptions
            {
                Formats = BarcodeFormats.All,
                AutoRotate = true,
                Multiple = false // We only want to detect one barcode at a time
            };
        }

        private async void BarcodesDetected_Handler(object sender, BarcodeDetectionEventArgs e)
        {
            barcodeReader.IsDetecting = false;
            await Task.Delay(1000); // Short delay to avoid duplicate scans

            var first = e.Results?.FirstOrDefault();
            if (first is null)
                return;

            await Dispatcher.DispatchAsync(async () =>
            {
                // Play beep sound
                if (_player == null)
                {
                    var audioManager = AudioManager.Current;
                    var stream = await FileSystem.OpenAppPackageFileAsync("beep.wav");
                    _player = audioManager.CreatePlayer(stream);
                }
                _player?.Play();

                // Show the detected barcode on the screen
                barcodeLabel.Text = $"Detected: {first.Value}";
                scanAgainButton.IsVisible = true;
                _barcodeService.SetBarcodeResult(first.Value);
            });
        }

        private void ScanAgainButton_Clicked(object sender, EventArgs e)
        {
            barcodeLabel.Text = string.Empty;
            scanAgainButton.IsVisible = false;
            barcodeReader.IsDetecting = true;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            // Start scanning when the page appears
            barcodeReader.IsDetecting = true;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            // Stop scanning when the page disappears
            barcodeReader.IsDetecting = false;
            _player?.Dispose();
            _player = null;
        }
    }
}