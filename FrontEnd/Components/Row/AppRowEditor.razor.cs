using Microsoft.AspNetCore.Components;

namespace Playground.FrontEnd.Components.Row;
public partial class AppRowEditor<TValue>
{
    [Parameter]
    public RenderFragment ChildContent { get; set; }
}
