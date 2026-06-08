using Microsoft.AspNetCore.Components;
using System.Globalization;

namespace Playground.FrontEnd.Components.Inputs;

public partial class CurrencyEditor<TValue>
{
    [Parameter]
    public bool ShowInputCurrencySymbol { get; set; }

    private string EffectivePlaceholder
    {
        get
        {
            if (!string.IsNullOrEmpty(PlaceHolder))
                return PlaceHolder;

            if (!ShowInputCurrencySymbol)
                return (0m).ToString("F2", CultureInfo.CurrentCulture);

            var culture = GetSpecificCulture(CultureInfo.CurrentCulture);
            var currencySymbol = culture.NumberFormat.CurrencySymbol;
            return $"{currencySymbol} 0.00";

        }
    }

    private static CultureInfo GetSpecificCulture(CultureInfo culture) =>
        culture.IsNeutralCulture
            ? CultureInfo.CreateSpecificCulture(culture.Name)
            : culture;
}
