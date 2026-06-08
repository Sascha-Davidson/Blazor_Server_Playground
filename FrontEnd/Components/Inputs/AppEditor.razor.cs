using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Playground.FrontEnd.Base;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace Playground.FrontEnd.Components.Inputs;
public partial class AppEditor<TValue> : EditorBase<TValue>
{
    [Parameter] 
    public bool? ForceBlazor { get; set; }

    [CascadingParameter]
    private EditContext? EditContext { get; set; }

    private Type ModelType => typeof(TValue);
    private bool IsNullable =>
        Nullable.GetUnderlyingType(ModelType) != null;

    // Override EditorKey to handle nullable types properly
    private string? EditorKey =>
        (DataTypeAttribute?.CustomDataType
         ?? DataTypeAttribute?.DataType.ToString()
         ?? ModelType.Name)
        ?.ToLowerInvariant();

    private Type ResolvedComponent
    {
        get
        {
            if (SelectList != null && SelectList.Any())
                return typeof(SelectEditor);

            if (EditorKey is not null && EditorMap.TryGetValue(EditorKey, out var editorComponent))
            {
                if (editorComponent.IsGenericTypeDefinition)
                {
                    var type = typeof(TValue);
                    return editorComponent.MakeGenericType(type);
                }

                return editorComponent;
            }

            return ResolveByClrType();
        }
    }

    private static bool IsNullableType(Type type)
    {
        return Nullable.GetUnderlyingType(type) != null;
    }

    private Type ResolveByClrType()
    {
        var type = typeof(TValue);

        if (type == typeof(string))
            return typeof(TextEditor);

        if (type == typeof(int) ||
            type == typeof(int?) ||
            type == typeof(long) ||
            type == typeof(long?) ||
            type == typeof(short) ||
            type == typeof(short?))
            return typeof(NumberEditor<>).MakeGenericType(type);

        if (type == typeof(bool) || type == typeof(bool?))
            return typeof(CheckboxEditor);

        if (type == typeof(DateTime) || type == typeof(DateTime?))
            return typeof(DateEditor);

        return typeof(TextEditor);
    }

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

        // Decimal: Use generic NumberEditor (will be instantiated with TValue)
        // For currency formatting, use [DataType("Currency")] with nullable decimal
        ["decimal"] = typeof(NumberEditor<>),

        ["currency"] = typeof(CurrencyEditor<TValue>),

        ["range"] = typeof(RangeEditor),
        ["percentage"] = typeof(PercentageEditor<>),

        ["password"] = typeof(PasswordEditor),

        ["email"] = typeof(EmailEditor),
        ["emailaddress"] = typeof(EmailEditor),

        ["phone"] = typeof(PhoneEditor),
        ["phonenumber"] = typeof(PhoneEditor),

        ["textarea"] = typeof(TextAreaEditor),
        ["multilinetext"] = typeof(TextAreaEditor),
    };

    private Dictionary<string, object?> Parameters
    {
        get
        {
            var parameters = new Dictionary<string, object?>();

            parameters["Value"] = Value;

            // ❌ DO NOT PASS THIS:
            // parameters["ValueChanged"] = ValueChanged;

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