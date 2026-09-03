using Microsoft.AspNetCore.Components;
using Playground.FrontEnd.Class;

namespace Playground.FrontEnd.Components.Button
{
    public partial class AppButton
    {
        [Parameter] public RenderFragment ChildContent { get; set; }
        [Parameter] public EventCallback OnClick { get; set; }
        [Parameter] public EventCallback OnConfirmed { get; set; }
        [Parameter] public bool Disabled { get; set; }
        [Parameter] public string TooltipText { get; set; }
        [Parameter] public string AriaLabel { get; set; }
        [Parameter] public string ConfirmMessage { get; set; } = "Are you sure you want to proceed?";
        [Parameter] public Color Color { get; set; } = Color.Default;
        [Parameter] public Variant Variant { get; set; } = Variant.Solid;
        [Parameter] public Size Size { get; set; } = Size.Medium;
        [Parameter] public AnchorOrigin Origin { get; set; } = AnchorOrigin.Top;
        [Parameter] public AnchorDirection Direction { get; set; } = AnchorDirection.Center;

        [Parameter(CaptureUnmatchedValues = true)]
        public Dictionary<string, object> AdditionalAttributes { get; set; }

        private readonly Guid Guid = Guid.NewGuid();
        private bool HasConfirmation => OnConfirmed.HasDelegate;
        private string InstanceId = System.Guid.NewGuid().ToString("N");
        private string popoverId => $"confirm-{InstanceId}";
        private string tooltipId => $"tip-{InstanceId}";
        private string ComputedPositionClass => $"origin-{Origin.ToString().ToLower()}-{Direction.ToString().ToLower()}";
        private string ButtonClass => $"app-button {Color.ToCssClass()} {Variant.ToCssClass()} {Size.ToCssClass()}";
        private string TooltipClass => $"tooltip {ComputedPositionClass}";

        private async Task HandleClick()
        {
            if (!HasConfirmation)
            {
                await OnClick.InvokeAsync();
            }
        }

        private async Task ExecuteConfirmedAction()
        {
            await OnConfirmed.InvokeAsync();
            // Optional: Programmatically close the popover here if needed via JS or state, 
            // though standard form submission/navigation usually handles re-rendering.
        }
    }

    public enum AnchorOrigin

    {
        Top,
        Bottom,
        Center,
        Left,
        Right
    }

    public enum AnchorDirection

    {
        Left,
        Center,
        Right,
        Top,
        Bottom
    }
}
