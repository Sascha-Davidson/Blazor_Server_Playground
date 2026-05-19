using Microsoft.AspNetCore.Components;

namespace Playground.FrontEnd.Components.Inputs;

public partial class SelectEditor<TValue>
{
    

    protected async Task HandleInput(ChangeEventArgs e)
    {
        Value = e.Value?.ToString();
        await ValueChanged.InvokeAsync(Value);
    }
}
