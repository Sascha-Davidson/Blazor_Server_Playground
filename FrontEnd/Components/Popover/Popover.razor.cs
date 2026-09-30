using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Playground.FrontEnd.Components.Forms.Controls;

namespace Playground.FrontEnd.Components.Popover;
public partial class Popover
{
    private ElementReference PopoverRef;

    [CascadingParameter]
    public SearchEditor SearchParent { get; set; } = default!;

    [Parameter]
    public RenderFragment<(string Item, bool IsSelected)> ChildContent { get; set; }

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            SearchParent.PopoverInstance = this;
        }
    }

    public async Task ShowAsync(ElementReference anchorEl)
    {
        await JS.InvokeVoidAsync("showPopover", anchorEl, PopoverRef);
    }

    public async Task HideAsync()
    {
        await JS.InvokeVoidAsync("hidePopover", PopoverRef);
    }
}
