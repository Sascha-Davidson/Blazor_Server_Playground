using Microsoft.AspNetCore.Components;
using Playground.FrontEnd.Base;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace Playground.FrontEnd.Components.Inputs;
public partial class AppEditor<TValue> : EditorBase<TValue>
{
    [Parameter] 
    public bool? ForceBlazor { get; set; }

    private Type ResolvedComponent
    {
        get
        {
            if (SelectList != null && SelectList.Any())
                return typeof(SelectEditor);

            if (DataTypeAttribute?.CustomDataType is { } custom)
            {
                var key = custom.ToLowerInvariant();
                if (EditorMap.TryGetValue(key, out var t))
                    return CloseGeneric(t);
            }

            if (DataTypeAttribute?.DataType is { } dt)
            {
                var key = dt.ToString().ToLowerInvariant();
                if (EditorMap.TryGetValue(key, out var t))
                    return CloseGeneric(t);
            }

            var clrKey = (Nullable.GetUnderlyingType(typeof(TValue)) ?? typeof(TValue)).Name.ToLowerInvariant();

            if (EditorMap.TryGetValue(clrKey, out var byClr))
                return CloseGeneric(byClr);

            return typeof(TextEditor);
        }
    }

    private static Type CloseGeneric(Type t) =>
        t.IsGenericTypeDefinition ? t.MakeGenericType(typeof(TValue)) : t;

    private static readonly Dictionary<string, Type> EditorMap = new()
    {
        ["text"] = typeof(TextEditor),
        ["textbox"] = typeof(TextEditor),
        ["string"] = typeof(TextEditor),

        ["datetime"] = typeof(DateTimeEditor),

        ["date"] = typeof(DateEditor),

        ["time"] = typeof(TimeEditor),
        ["timespan"] = typeof(TimeEditor),

        ["bool"] = typeof(CheckboxEditor),
        ["boolean"] = typeof(CheckboxEditor),

        ["int"] = typeof(NumberEditor<>),
        ["int16"] = typeof(NumberEditor<>),
        ["int32"] = typeof(NumberEditor<>),
        ["int64"] = typeof(NumberEditor<>),
        ["uint16"] = typeof(NumberEditor<>),
        ["uint32"] = typeof(NumberEditor<>),
        ["uint64"] = typeof(NumberEditor<>),
        ["long"] = typeof(NumberEditor<>),
        ["double"] = typeof(NumberEditor<>),

        // Decimal: Use DecimalEditor for proper decimal formatting
        // For currency formatting, use [DataType("Currency")] with nullable decimal
        ["decimal"] = typeof(DecimalEditor<>),

        ["currency"] = typeof(CurrencyEditor<TValue>),

        ["range"] = typeof(RangeEditor),
        ["percentage"] = typeof(PercentageEditor<>),

        ["password"] = typeof(PasswordEditor),

        ["email"] = typeof(EmailEditor),

        ["phone"] = typeof(PhoneEditor),

        ["textarea"] = typeof(TextAreaEditor),
        ["multilinetext"] = typeof(TextAreaEditor),
    };

    private Dictionary<string, object> Parameters
    {
        get
        {
            var parameters = new Dictionary<string, object>();

            parameters["Value"] = Value;

            if (Expression is not null)
                parameters["Expression"] = Expression;

            parameters["Required"] = IsRequired;
            parameters["Style"] = CheckboxStyle;

            parameters["ID"] = ID;
            parameters["Name"] = Name;
            parameters["PlaceHolder"] = PlaceHolder;

            parameters["ReadOnly"] = ReadOnly;
            parameters["Disabled"] = Disabled;

            if (SelectList is not null)
                parameters["SelectList"] = SelectList;

            if (ValueSelector is not null)
                parameters["ValueSelector"] = ValueSelector;

            if (OptionContent is not null)
                parameters["OptionContent"] = OptionContent;

            return parameters;
        }
    }

    private bool IsRequired =>
        Required || HasRequiredAttribute();

    private bool HasRequiredAttribute()
    {
        if (Expression?.Body is not MemberExpression memberExpression)
            return false;

        return Attribute.IsDefined(
            memberExpression.Member,
            typeof(RequiredAttribute));
    }
}