using AppProducts.Maui.Blazor.Pages;
using AppProducts.Shared.Blazor.Services;

namespace AppProducts.Maui.Blazor;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

        Routing.RegisterRoute(nameof(ScannerPage), typeof(ScannerPage));

        MainPage = new NavigationPage(new MainPage());
	}

    protected override Window CreateWindow(IActivationState? activationState)
    {
        Window window = base.CreateWindow(activationState);

		window.Title = "Products Sample";

		return window;
    }
}
