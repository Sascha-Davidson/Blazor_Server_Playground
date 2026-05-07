using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace Playground.FrontEnd.Components.Inputs;
public partial class AppEditor<TValue>
{
    [Parameter]
    public TValue? Value { get; set; }

    [Parameter]
    public EventCallback<TValue?> ValueChanged { get; set; }

    [Parameter]
    public Expression<Func<TValue>>? Expression { get; set; }

    [Parameter] 
    public bool Required { get; set; }

    private Type ResolvedComponent => typeof(TValue) switch
    {
        var t when t == typeof(string)
            => typeof(TextEditor),

        var t when t == typeof(DateTime)
                   || t == typeof(DateTime?)
            => typeof(DateEditor),

        _ => typeof(TextEditor)
    };

    private Dictionary<string, object?> Parameters =>
        new()
        {
            ["Value"] = Value,
            ["ValueChanged"] = ValueChanged,
            ["Expression"] = Expression,
            ["Required"] = IsRequired,
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