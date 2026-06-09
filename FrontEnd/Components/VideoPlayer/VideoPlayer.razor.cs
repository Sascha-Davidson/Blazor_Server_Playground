using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Playground.FrontEnd.Components.VideoPlayer
{
    public partial class VideoPlayer
    {
        [Inject] public IJSRuntime JS { get; set; } = default!;

        public bool IsPaused { get; set; } = true;
        public bool IsPlaying => !IsPaused;

        private bool isFullscreen = false;
        private DateTime lastMouseMove = DateTime.Now;
        private System.Timers.Timer mouseIdleTimer;

        private ElementReference videoRef;
        public ElementReference VideoRef => videoRef;
        private ElementReference playerContainer;


        public double CurrentTime { get; private set; }
        public double Duration { get; private set; }
        [Parameter] public string Source { get; set; } = string.Empty;

        private DotNetObjectReference<VideoPlayer> objRef;


        public async Task PlayAsync()
        {
            await JS.InvokeVoidAsync("videoHelper.play", videoRef);
            IsPaused = false;
            StateHasChanged();
        }

        public async Task PauseAsync()
        {
            await JS.InvokeVoidAsync("videoHelper.pause", videoRef);
            IsPaused = true;
            StateHasChanged();
        }

        [JSInvokable]
        public void UpdateProgress(double current, double total)
        {
            CurrentTime = current;
            Duration = total;
            StateHasChanged();
        }

        public async Task TogglePlayPauseAsync()
        {
            bool isPaused = await JS.InvokeAsync<bool>("videoHelper.isPaused", videoRef);
            if (isPaused) await PlayAsync();
            else await PauseAsync();
        }

        public async Task<double> GetCurrentTimeAsync() =>
            await JS.InvokeAsync<double>("videoHelper.getCurrentTime", videoRef);

        public async Task SeekAsync(double time) =>
            await JS.InvokeVoidAsync("videoHelper.seek", videoRef, time);

        public async Task AttachTimeUpdateAsync(DotNetObjectReference<VideoPlayer> dotNetRef)
        {
            await JS.InvokeVoidAsync("videoHelper.attachTimeUpdate", videoRef, dotNetRef);
        }
        
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                objRef = DotNetObjectReference.Create(this);
                await AttachTimeUpdateAsync(objRef);

                var current = await JS.InvokeAsync<double>("videoHelper.getCurrentTime", videoRef);
                var total = await JS.InvokeAsync<double>("videoHelper.getDuration", videoRef);
                UpdateProgress(current, total);
            }
        }

        public ValueTask<ElementReference> GetVideoElementAsync()
        => ValueTask.FromResult(playerContainer);

        public void FullScreenMenuToggle()
        {
            if (!isFullscreen) return;

            if (mouseIdleTimer == null)
            {
                mouseIdleTimer = new System.Timers.Timer(100);
                mouseIdleTimer.Elapsed += (s, e) =>
                {
                    var idleTime = (DateTime.Now - lastMouseMove).TotalSeconds;

                    if (idleTime >= 4) // 4 seconds of no mouse movement
                    {
                        if (!IsPaused)
                        {
                            IsPaused = false; // keep playing if not paused
                        }
                    }
                    else
                    {
                        IsPaused = true; // mouse moving → pause controls?
                    }

                    InvokeAsync(StateHasChanged);
                };
                mouseIdleTimer.AutoReset = true;
                mouseIdleTimer.Start();
            }
        }
    }
}
