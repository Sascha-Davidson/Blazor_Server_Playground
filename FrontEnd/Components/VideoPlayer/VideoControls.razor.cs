using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Playground.FrontEnd.Components.VideoPlayer
{
    public partial class VideoControls
    {
        [CascadingParameter] public VideoPlayer Player { get; set; }
        [Inject] public IJSRuntime JS { get; set; } = default!;
        private string CurrentTime => FormatTime(Player?.CurrentTime ?? 0);
        private string Duration => FormatTime(Player?.Duration ?? 0);
        private bool showSettings = false;
        private bool isFullscreen = false;


        private double volume = 1.0;
        private bool isMuted = false;

        private VideoPlayerIcon.VideoIcon VolumeIcon =>
            isMuted || volume == 0
                ? VideoPlayerIcon.VideoIcon.VolumeOff
                : volume <= 0.5
                    ? VideoPlayerIcon.VideoIcon.VolumeLow
                    : VideoPlayerIcon.VideoIcon.VolumeHigh;

        private async Task TogglePlay()
        {
            if (Player is not null)
            {
                await Player.TogglePlayPauseAsync();
                StateHasChanged();
            }
        }

        private static string FormatTime(double seconds)
        {
            var ts = TimeSpan.FromSeconds(seconds);
            return ts.Hours > 0
                ? $"{ts.Hours}:{ts.Minutes:D2}:{ts.Seconds:D2}"
                : $"{ts.Minutes}:{ts.Seconds:D2}";
        }

        private void ToggleSettings()
        {
            showSettings = !showSettings;
        }

        private void CloseSettings()
        {
            showSettings = false;
        }

        private async Task ToggleFullScreen()
        {
            if (Player is not null)
            {
                await JS.InvokeVoidAsync("videoPlayer.toggleFullScreen",
                    await Player.GetVideoElementAsync());
                isFullscreen = !isFullscreen;
            }
        }
    }
}
