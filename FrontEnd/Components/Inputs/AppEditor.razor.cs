using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Playground.FrontEnd.Base;
using Playground.Lib.Enums;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace Playground.FrontEnd.Components.Inputs;
public partial class AppEditor<TValue> : EditorBase<TValue>
{
    [Parameter] 
    public bool? ForceBlazor { get; set; }

    [CascadingParameter]
    private EditContext? EditContext { get; set; }

    private bool UseBlazorInputs =>
        ForceBlazor ?? (EditContext is not null);

    private Type ResolvedComponent
    {
        get
        {
            if (EditorKey is null)
                return typeof(TextEditor);

            if (SelectList != null && SelectList.Any())
            {
                return UseBlazorInputs
                    ? typeof(SelectEditor)
                    : typeof(SelectEditor);
            }

            // Runtime-dependent case
            if (EditorKey == "string")
            {
                return UseBlazorInputs
                    ? typeof(TextEditor)
                    : typeof(TextEditor);
            }

            return EditorMap.TryGetValue(EditorKey, out var component)
                ? component
                : typeof(TextEditor);
        }
    }

    private static readonly Dictionary<string, Type> EditorMap = new()
    {
        ["text"] = typeof(TextEditor),
        ["textbox"] = typeof(TextEditor),

        ["datetime"] = typeof(DateEditor),
        ["date"] = typeof(DateEditor),

        ["bool"] = typeof(CheckboxEditor),
        ["boolean"] = typeof(CheckboxEditor),

        ["int"] = typeof(NumberEditor),
        ["int32"] = typeof(NumberEditor),
    };

    private Dictionary<string, object?> Parameters =>
        new()
        {
            ["Value"] = Value,
            ["ValueChanged"] = ValueChanged,
            ["Expression"] = Expression,
            ["Required"] = IsRequired,
            ["Style"] = CheckboxStyle,
            ["ID"] = ID,
            ["Name"] = Name,
            ["PlaceHolder"] = PlaceHolder,
            ["ReadOnly"] = ReadOnly,
            ["Disabled"] = Disabled,
            ["SelectList"] = SelectList,
            ["ValueSelector"] = ValueSelector,
            ["OptionContent"] = OptionContent,
        };

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