using Microsoft.AspNetCore.Components;
using Playground.Lib.Enums;
using Playground.Lib.Extensions;

namespace Playground.FrontEnd.Base
{
    public class FieldRowBase<T> : FieldEditorBase<T>
    {
        [Parameter]
        public string Label { get; set; }

        [Parameter]
        public bool? ForceBlazor { get; set; }

        [Parameter]
        public RowOrientation Orientation { get; set; } = RowOrientation.Auto;

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public string FieldClass { get; set; }

        [Parameter]
        public string FieldLayoutClass { get; set; }

        public string ComputedFieldClass() => ChildContent != null ? $"{FieldLayoutClass} has-additional-content" : FieldLayoutClass;
        
        public readonly int SetId = Guid.NewGuid().GetHashCode();

        [Parameter]
        public string Description { get; set; }

        public string ExpressionName => ExpressionMember?.GetDisplayValue();
        public string ExpressionDescription => ExpressionMember?.GetDescriptionValue();
        public string ExpressionValue => Expression?.Compile(false)?.Invoke()?.ToString();
        public string LabelText => Label ?? ExpressionName;
    }
}
