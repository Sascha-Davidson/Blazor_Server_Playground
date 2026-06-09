using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System.Globalization;

namespace Playground.FrontEnd.Components.BaseInput;

public partial class InputCurrency<TValue> : InputBase<TValue>
{
    private bool _isFocused;
    private string _inputText = string.Empty;
    private ElementReference _inputElement;

    [Inject]
    private IJSRuntime JSRuntime { get; set; } = default!;

    [Parameter]
    public bool ShowCurrencySymbol { get; set; }

    // Called once when the component first renders, or when Value changes externally
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        // Only sync external value → text when the user isn't actively typing
        if (!_isFocused)
            _inputText = FormatForDisplay(ToDecimal(CurrentValue), ShowCurrencySymbol);
    }

    private async Task HandleFocus(FocusEventArgs _)
    {
        _isFocused = true;
        // Switch to plain numeric text so the user can edit freely
        var decimalValue = ToDecimal(CurrentValue);
        _inputText = decimalValue.HasValue
            ? decimalValue.Value.ToString("0.00", CultureInfo.CurrentCulture)
            : string.Empty;

        await InvokeAsync(StateHasChanged);
        await Task.Yield();

        // Auto-select the text
        if (decimalValue.HasValue)
            await JSRuntime.InvokeVoidAsync("selectInputText", _inputElement);
    }

    private void HandleBlur(FocusEventArgs _)
    {
        _isFocused = false;
        // Commit whatever is in the box, then reformat for display
        CommitCurrentText();
        // Tell EditContext this field was touched, triggering validation
        EditContext?.NotifyFieldChanged(FieldIdentifier);
        _inputText = FormatForDisplay(ToDecimal(CurrentValue), ShowCurrencySymbol);
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
            SetCurrentValue(null);
            return;
        }

        if (decimal.TryParse(_inputText, NumberStyles.Number, CultureInfo.CurrentCulture, out var parsed))
            SetCurrentValue(parsed);
        // If it doesn't parse yet (e.g. user just typed "12,") leave CurrentValue alone
    }

    private static decimal? ToDecimal(TValue value)
    {
        if (value is null) return null;
        try { return Convert.ToDecimal(value); }
        catch { return null; }
    }

    private void SetCurrentValue(decimal? value)
    {
        if (value is null)
        {
            CurrentValue = default!;
            return;
        }
        var underlyingType = Nullable.GetUnderlyingType(typeof(TValue)) ?? typeof(TValue);
        try { CurrentValue = (TValue)Convert.ChangeType(value.Value, underlyingType)!; }
        catch { CurrentValue = default!; }
    }

    private static string FormatForDisplay(decimal? value, bool showCurrencySymbol) =>
        value.HasValue
            ? value.Value.ToString(showCurrencySymbol ? "C" : "N2", GetSpecificCulture(CultureInfo.CurrentCulture))
            : string.Empty;

    private static CultureInfo GetSpecificCulture(CultureInfo culture) =>
        culture.IsNeutralCulture
            ? CultureInfo.CreateSpecificCulture(culture.Name)
            : culture;

    // Still required by InputBase — only used if you ever call base rendering
    protected override string FormatValueAsString(TValue value) =>
        FormatForDisplay(ToDecimal(value), ShowCurrencySymbol);

    protected override bool TryParseValueFromString(
        string value,
        out TValue result,
        out string validationErrorMessage)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            result = default!;
            validationErrorMessage = null;
            return true;
        }

        if (decimal.TryParse(value, NumberStyles.Number, CultureInfo.CurrentCulture, out var parsed))
        {
            var underlyingType = Nullable.GetUnderlyingType(typeof(TValue)) ?? typeof(TValue);
            try
            {
                result = (TValue)Convert.ChangeType(parsed, underlyingType)!;
                validationErrorMessage = null;
                return true;
            }
            catch { }
        }

        result = default!;
        validationErrorMessage = "Please enter a valid currency value.";
        return false;
    }
}