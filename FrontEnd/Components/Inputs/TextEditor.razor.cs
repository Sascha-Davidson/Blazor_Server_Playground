using Microsoft.AspNetCore.Components;

namespace Playground.FrontEnd.Components.Inputs;

public partial class TextEditor
{
    private async Task HandleInput(ChangeEventArgs e)
    {
        await ValueChanged.InvokeAsync(
            e.Value?.ToString());
    }
}