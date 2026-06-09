using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System.Globalization;

namespace Playground.FrontEnd.Components.VideoPlayer
{
    public partial class VideoProgressBar
    {
        [CascadingParameter] public VideoPlayer Player { get; set; }

        [Inject] public IJSRuntime JS { get; set; } = default!;

        private double playPercent = 0.0;
        private double bufferPercent = 0.0;
        private double loadPercent = 0.0;
        private bool isDragging = false; 
        private ElementReference progressBarRef;

        private DotNetObjectReference<VideoProgressBar> _dotNetRef;

        [Parameter] public double Duration { get; set; } = 100;
        [Parameter] public EventCallback<double> OnSeek { get; set; }

        private static string FormatPercent(double value)
            => value.ToString("0.###", CultureInfo.InvariantCulture) + "%";
                protected override void OnInitialized()
        {
            _dotNetRef = DotNetObjectReference.Create(this);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender && Player is not null)
            {
                await Player.JS.InvokeVoidAsync("videoHelper.attachTimeUpdate", Player.VideoRef, DotNetObjectReference.Create(this));
            }
        }

        private async Task SeekVideo(MouseEventArgs e)
        {
            if (Player?.VideoRef is not null)
            {
                var rect = await Player.JS.InvokeAsync<JsRect>("getBoundingClientRect", progressBarRef);
                double clickPos = e.ClientX - rect.Left;
                double percent = Math.Clamp(clickPos / rect.Width, 0, 1);
                double newTime = (await Player.JS.InvokeAsync<double>("videoHelper.getDuration", Player.VideoRef)) * percent;
                await Player.JS.InvokeVoidAsync("videoHelper.seek", Player.VideoRef, newTime);
            }
        }

        [JSInvokable]
        public void UpdateProgress(double currentTime, double duration)
        {
            playPercent = duration > 0 ? (currentTime / duration) * 100.0 : 0.0;
            InvokeAsync(StateHasChanged);
        }

        [JSInvokable]
        public void UpdateBuffer(double bufferedEnd, double duration)
        {
            bufferPercent = duration > 0 ? (bufferedEnd / duration) * 100.0 : 0.0;
            InvokeAsync(StateHasChanged);
        }

        [JSInvokable]
        public void UpdateLoad(double readyState, double duration)
        {
            loadPercent = (duration > 0 && readyState >= 3) ? 100.0 : 0.0;
            InvokeAsync(StateHasChanged);
        }

        private async Task OnMouseDown(MouseEventArgs e)
        {
            if (e.Button != 0)
                return;

            if (isDragging)
            {
                isDragging = false;
                await JS.InvokeVoidAsync("progressInterop.unregisterMouseEvents");
            }

            isDragging = true;
            await UpdatePlayPercent(e);
            await JS.InvokeVoidAsync("progressInterop.registerMouseEvents", _dotNetRef);
        }

        [JSInvokable]
        public async Task OnMouseMove(double clientX)
        {
            if (!isDragging) return;

            var rect = await JS.InvokeAsync<JsRect>("progressInterop.getRect", progressBarRef);
            var offset = clientX - rect.Left;
            var percent = offset / rect.Width * 100;

            playPercent = Math.Clamp(percent, 0, 100);
            StateHasChanged();
        }

        [JSInvokable]
        public async Task OnMouseUp(double clientX)
        {
            if (!isDragging) return; // <-- guard
            isDragging = false;

            var rect = await JS.InvokeAsync<JsRect>("progressInterop.getRect", progressBarRef);
            var offset = clientX - rect.Left;
            var percent = offset / rect.Width * 100;

            playPercent = Math.Clamp(percent, 0, 100);

            if (Player?.VideoRef is not null)
            {
                var duration = await Player.JS.InvokeAsync<double>("videoHelper.getDuration", Player.VideoRef);
                var newTime = duration * playPercent / 100;
                await Player.JS.InvokeVoidAsync("videoHelper.seek", Player.VideoRef, newTime);
            }

            // Always clean up
            await JS.InvokeVoidAsync("progressInterop.unregisterMouseEvents");

            StateHasChanged();
        }

        private async Task UpdatePlayPercent(MouseEventArgs e)
        {
            var rect = await JS.InvokeAsync<JsRect>("progressInterop.getRect", progressBarRef);
            var offset = e.ClientX - rect.Left;
            var percent = offset / rect.Width * 100;

            playPercent = Math.Clamp(percent, 0, 100);
            StateHasChanged();
        }

        public class JsRect
        {
            public double Left { get; set; }
            public double Top { get; set; }
            public double Width { get; set; }
            public double Height { get; set; }
        }
    }
}
