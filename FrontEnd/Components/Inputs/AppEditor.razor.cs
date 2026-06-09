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
        // typeparam
        ["string"] = typeof(TextEditor),
        ["decimal"] = typeof(DecimalEditor<>),
        ["double"] = typeof(NumberEditor<>),
        ["float"] = typeof(NumberEditor<>),
        ["int"] = typeof(NumberEditor<>),
        ["long"] = typeof(NumberEditor<>),
        ["short"] = typeof(NumberEditor<>),
        ["byte"] = typeof(NumberEditor<>),
        ["uint"] = typeof(NumberEditor<>),
        ["ulong"] = typeof(NumberEditor<>),
        ["ushort"] = typeof(NumberEditor<>),
        ["bool"] = typeof(CheckboxEditor),
        ["datetime"] = typeof(DateTimeEditor),
        ["datetimeoffset"] = typeof(DateTimeEditor),
        ["timespan"] = typeof(DateTimeEditor),

        // datatype
        ["text"] = typeof(TextEditor),
        ["textbox"] = typeof(TextEditor),
        ["date"] = typeof(DateEditor),
        ["boolean"] = typeof(CheckboxEditor),
        ["int16"] = typeof(NumberEditor<>),
        ["int32"] = typeof(NumberEditor<>),
        ["int64"] = typeof(NumberEditor<>),
        ["uint16"] = typeof(NumberEditor<>),
        ["uint32"] = typeof(NumberEditor<>),
        ["uint64"] = typeof(NumberEditor<>),
        ["currency"] = typeof(CurrencyEditor<TValue>),
        ["range"] = typeof(RangeEditor),
        ["percentage"] = typeof(PercentageEditor<>),
        ["password"] = typeof(PasswordEditor),
        ["email"] = typeof(EmailEditor),
        ["phone"] = typeof(PhoneEditor),
        ["textarea"] = typeof(TextAreaEditor),
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