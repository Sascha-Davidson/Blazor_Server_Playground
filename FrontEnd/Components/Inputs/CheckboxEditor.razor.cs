using Microsoft.AspNetCore.Components;
using Playground.Lib.Enums;

namespace Playground.FrontEnd.Components.Inputs;

public partial class CheckboxEditor
{
    [Parameter]
    public CheckboxStyle Style { get; set; } = CheckboxStyle.Box;
}
