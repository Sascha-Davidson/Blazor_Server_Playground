using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Playground.FrontEnd.Components.Menu;

public partial class HeaderMenu
{
    [Inject]
    public IJSRuntime JS { get; set; }

    private string CurrentTheme { get; set; } = "system";

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            CurrentTheme = await JS.InvokeAsync<string>("themeManager.get");
            StateHasChanged();
        }
    }

    private async Task SetTheme(string theme)
    {
        CurrentTheme = theme;
        await JS.InvokeVoidAsync("themeManager.set", theme);
    }
}
