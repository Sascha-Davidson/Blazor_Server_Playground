using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Playground.FrontEnd.Components.BaseInput;

public partial class InputPhone : InputBase<string>
{
    private void HandleInput(ChangeEventArgs e)
    {
        CurrentValueAsString = e.Value?.ToString() ?? string.Empty;
    }

    protected override bool TryParseValueFromString(
        string value,
        out string result,
        out string validationErrorMessage)
    {
        result = value;
        validationErrorMessage = null;
        return true;
    }
}