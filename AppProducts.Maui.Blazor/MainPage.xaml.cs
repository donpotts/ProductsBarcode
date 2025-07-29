namespace AppProducts.Maui.Blazor;

public partial class MainPage : ContentPage
{
    public static MainPage? Instance { get; private set; }

	public MainPage()
	{
		InitializeComponent();
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
        Instance = this;
    }

    public async Task NavigateToScannerPage()
    {
        var services = App.Current?.Handler?.MauiContext?.Services;
        var barcodeService = services?.GetService(typeof(AppProducts.Maui.Blazor.Services.BarcodeService)) as AppProducts.Maui.Blazor.Services.BarcodeService;
        await Navigation.PushAsync(new AppProducts.Maui.Blazor.Pages.ScannerPage(barcodeService!));
    }
}
