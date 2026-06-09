using Microsoft.AspNetCore.Components;
using Playground.FrontEnd.Class;

namespace Playground.FrontEnd.Components.Button
{
    public partial class AppButton
    {
        [Parameter] public RenderFragment ChildContent { get; set; }
        [Parameter] public EventCallback OnClick { get; set; }
        [Parameter] public bool Disabled { get; set; }
        [Parameter] public Color Color { get; set; } = Color.Default;
        [Parameter] public Variant Variant { get; set; } = Variant.Solid;
        [Parameter] public Size Size { get; set; } = Size.Medium;

        private string ClassBuilder => $"app-button {Color.ToCssClass()} {Variant.ToCssClass()} {Size.ToCssClass()}";
	}
}
