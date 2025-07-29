using AppProducts.Shared.Blazor;
using AppProducts.Shared.Blazor.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<Main>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBlazorServices(builder.HostEnvironment.BaseAddress);
builder.Services.AddBrowserStorageService();
builder.Services.AddScoped<ICustomPopupService, CustomPopupService>();
await builder.Build().RunAsync();
