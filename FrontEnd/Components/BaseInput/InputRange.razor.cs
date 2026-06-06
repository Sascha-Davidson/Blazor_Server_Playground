using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Playground.FrontEnd.Components.BaseInput;

public partial class InputRange : InputBase<int>
{
    [Parameter]
    public int Min { get; set; }

    [Parameter]
    public int Max { get; set; }

    [Parameter]
    public int Step { get; set; } = 1;


    private void HandleInput(ChangeEventArgs e)
    {
        CurrentValueAsString = e.Value?.ToString() ?? string.Empty;
    }

    protected override bool TryParseValueFromString(
        string value,
        out int result,
        out string validationErrorMessage)
    {
        if (int.TryParse(value, out result))
        {
            validationErrorMessage = null;
            return true;
        }

        result = 0;
        validationErrorMessage = $"The value '{value}' is not a valid number.";
        return false;
    }
}