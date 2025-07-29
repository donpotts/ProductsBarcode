using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using AppProducts.Shared.Models;
using MudBlazor;

namespace AppProducts.Shared.Blazor.Components.Themes;

public partial class ThemesMenu
{
    //MudOverlay not working!
    
    private readonly List<string> _primaryColors = new()
    {
        "#594AE2",
        MudBlazor.Colors.Green.Default,
        MudBlazor.Colors.Blue.Default,
        MudBlazor.Colors.BlueGray.Default,
        MudBlazor.Colors.Purple.Default,
        MudBlazor.Colors.Orange.Default,
        MudBlazor.Colors.Red.Default,
        MudBlazor.Colors.Brown.Default,
        MudBlazor.Colors.Cyan.Default,
        MudBlazor.Colors.DeepPurple.Default,
        MudBlazor.Colors.Yellow.Default,
        MudBlazor.Colors.Pink.Default,
        MudBlazor.Colors.Lime.Default,
        MudBlazor.Colors.Indigo.Default,
        MudBlazor.Colors.Teal.Default,
        MudBlazor.Colors.Amber.Default,
    };

    [EditorRequired] [Parameter] public bool ThemingDrawerOpen { get; set; }
    [EditorRequired] [Parameter] public EventCallback<bool> ThemingDrawerOpenChanged { get; set; }
    [EditorRequired] [Parameter] public ThemeManagerModel ThemeManager { get; set; }
    [EditorRequired] [Parameter] public EventCallback<ThemeManagerModel> ThemeManagerChanged { get; set; }

    private async Task UpdateThemePrimaryColor(string color)
    {
        ThemeManager.PrimaryColor = color;
        await ThemeManagerChanged.InvokeAsync(ThemeManager);
    }

    private async Task ToggleDarkLightMode(bool isDarkMode)
    {
        ThemeManager.IsDarkMode = isDarkMode;
        await ThemeManagerChanged.InvokeAsync(ThemeManager);
    }
}