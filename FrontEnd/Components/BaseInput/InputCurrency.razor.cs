using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System.Globalization;

namespace Playground.FrontEnd.Components.BaseInput;

public partial class InputCurrency : InputBase<decimal?>
{
    private bool _isFocused;
    private string _inputText = string.Empty;
    private ElementReference _inputElement;

    [Inject]
    private IJSRuntime JSRuntime { get; set; } = default!;

    // Called once when the component first renders, or when Value changes externally
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        // Only sync external value → text when the user isn't actively typing
        if (!_isFocused)
            _inputText = FormatForDisplay(CurrentValue);
    }

    private async Task HandleFocus(FocusEventArgs _)
    {
        _isFocused = true;
        // Switch to plain numeric text so the user can edit freely
        _inputText = CurrentValue.HasValue
            ? CurrentValue.Value.ToString("0.00", CultureInfo.CurrentCulture)
            : string.Empty;

        await InvokeAsync(StateHasChanged);
        await Task.Yield();

        // Auto-select the text
        if (CurrentValue.HasValue)
            await JSRuntime.InvokeVoidAsync("selectInputText", _inputElement);
    }

    private void HandleBlur(FocusEventArgs _)
    {
        _isFocused = false;
        // Commit whatever is in the box, then reformat for display
        CommitCurrentText();
        // Tell EditContext this field was touched, triggering validation
        EditContext?.NotifyFieldChanged(FieldIdentifier);
        _inputText = FormatForDisplay(CurrentValue);
        StateHasChanged();
    }

    private void HandleInput(ChangeEventArgs e)
    {
        // Keep the raw string in sync so the cursor never jumps
        _inputText = e.Value?.ToString() ?? string.Empty;
        CommitCurrentText();
    }

    private void CommitCurrentText()
    {
        if (string.IsNullOrWhiteSpace(_inputText))
        {
            CurrentValue = null;
            return;
        }

        if (decimal.TryParse(_inputText, NumberStyles.Number, CultureInfo.CurrentCulture, out var parsed))
            CurrentValue = parsed;
        // If it doesn't parse yet (e.g. user just typed "12,") leave CurrentValue alone
    }

    private static string FormatForDisplay(decimal? value) =>
        value.HasValue
            ? value.Value.ToString("C", GetSpecificCulture(CultureInfo.CurrentCulture))
            : string.Empty;

    private static CultureInfo GetSpecificCulture(CultureInfo culture) =>
        culture.IsNeutralCulture
            ? CultureInfo.CreateSpecificCulture(culture.Name)
            : culture;

    // Still required by InputBase — only used if you ever call base rendering
    protected override string? FormatValueAsString(decimal? value) =>
        FormatForDisplay(value);

    protected override bool TryParseValueFromString(
        string? value,
        out decimal? result,
        out string? validationErrorMessage)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            result = null;
            validationErrorMessage = null;
            return true;
        }

        if (decimal.TryParse(value, NumberStyles.Number, CultureInfo.CurrentCulture, out var parsed))
        {
            result = parsed;
            validationErrorMessage = null;
            return true;
        }

        result = null;
        validationErrorMessage = "Please enter a valid currency value.";
        return false;
    }
}