using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace Playground.FrontEnd.Pages;
public partial class VideoComparison
{
    private IJSObjectReference? _module;

    private ElementReference _videoA;
    private ElementReference _videoB;

    private DotNetObjectReference<VideoComparison>? _dotNetRef;

    // Frames per second used to convert between time and frame numbers.
    private double Fps = 30;

    // Offset of video B relative to video A, expressed in whole frames.
    private int FrameOffset = 0;

    // Offset in seconds, derived from the frame offset and fps.
    private double OffsetSeconds => Fps > 0 ? FrameOffset / Fps : 0;

    // Live playback positions (seconds) reported from JS.
    private double CurrentTimeA = 0;
    private double CurrentTimeB = 0;

    private int CurrentFrameA => Fps > 0 ? (int)Math.Round(CurrentTimeA * Fps) : 0;
    private int CurrentFrameB => Fps > 0 ? (int)Math.Round(CurrentTimeB * Fps) : 0;

    // Maximum upload size for a video file (2 GB).
    private const long MaxVideoSizeBytes = 1024L * 1024L * 1024L * 2L;

    // Streaming a large video through JS interop can easily exceed the default
    // one-minute interop timeout, which surfaces as a TaskCanceledException.
    private static readonly TimeSpan VideoLoadTimeout = TimeSpan.FromMinutes(30);

    private VideoMetadata MetadataA = new();
    private VideoMetadata MetadataB = new();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
            return;

        _module = await JS.InvokeAsync<IJSObjectReference>(
                "import",
                "./Pages/VideoComparison.razor.js");

        _dotNetRef = DotNetObjectReference.Create(this);

        await _module.InvokeVoidAsync(
            "initialize",
            _dotNetRef,
            _videoA,
            _videoB);
    }

    private async Task LoadVideoA(InputFileChangeEventArgs e)
    {
        await EnsureJs();

        var file = e.File;

        MetadataA.FileSize = file.Size;

        await using var stream = file.OpenReadStream(maxAllowedSize: MaxVideoSizeBytes);

        var streamRef = new DotNetStreamReference(stream);

        var result = await _module!.InvokeAsync<VideoMetadata>(
            "setVideoSource",
            VideoLoadTimeout,
            _videoA,
            streamRef,
            file.ContentType);

        var fileSize = file.Size;
        MetadataA = result;
        MetadataA.FileSize = fileSize;

        // Auto-detect the frame rate so frame-based offsets are accurate.
        if (TryParseFps(result.FrameRate, out var detectedFps))
            Fps = detectedFps;

        StateHasChanged();
    }

    private async Task LoadVideoB(InputFileChangeEventArgs e)
    {
        await EnsureJs();

        var file = e.File;

        MetadataB.FileSize = file.Size;

        await using var stream = file.OpenReadStream(maxAllowedSize: MaxVideoSizeBytes);

        var streamRef = new DotNetStreamReference(stream);

        var result = await _module.InvokeAsync<VideoMetadata>(
            "setVideoSource",
            VideoLoadTimeout,
            _videoB,
            streamRef,
            file.ContentType);

        var fileSize = file.Size;
        MetadataB = result;
        MetadataB.FileSize = fileSize;

        StateHasChanged();
    }

    private static bool TryParseFps(string? frameRate, out double fps)
    {
        fps = 0;
        if (string.IsNullOrWhiteSpace(frameRate))
            return false;

        // frameRate looks like "30.000 fps"; take the leading number.
        var token = frameRate.Split(' ')[0];
        return double.TryParse(
            token,
            System.Globalization.NumberStyles.Float,
            System.Globalization.CultureInfo.InvariantCulture,
            out fps) && fps > 0;
    }

    private async Task ApplyOffset()
    {
        if (_module is null)
            return;

        await _module.InvokeVoidAsync(
            "syncSeek",
            OffsetSeconds);
    }

    // Shift video B by a number of frames relative to video A and re-sync.
    private async Task StepFrameOffset(int deltaFrames)
    {
        FrameOffset += deltaFrames;
        await ApplyOffset();
    }

    // Reset the offset so video B starts at the same moment as video A.
    private async Task ResetOffset()
    {
        FrameOffset = 0;
        await ApplyOffset();
    }

    // Called from JavaScript whenever a video's playback position changes.
    [JSInvokable]
    public void OnTimeUpdate(double timeA, double timeB)
    {
        CurrentTimeA = timeA;
        CurrentTimeB = timeB;
        StateHasChanged();
    }

    private async Task EnsureJs()
    {
        if (_module is null)
        {
            _module = await JS.InvokeAsync<IJSObjectReference>(
                "import",
                "./Pages/VideoComparison.razor.js");

            _dotNetRef ??= DotNetObjectReference.Create(this);

            await _module.InvokeVoidAsync(
                "initialize",
                _dotNetRef,
                _videoA,
                _videoB);
        }
    }

    private async Task PlayPause()
    {
        if (_module is null)
            return;

        await _module.InvokeVoidAsync(
            "playPause",
            _videoA,
            _videoB,
            OffsetSeconds);
    }

    private async Task Stop()
    {
        if (_module is null)
            return;

        await _module.InvokeVoidAsync(
            "stop",
            _videoA,
            _videoB);
    }

    public async ValueTask DisposeAsync()
    {
        _dotNetRef?.Dispose();

        if (_module is null)
            return;

        try
        {
            await _module.DisposeAsync();
        }
        catch (JSDisconnectedException)
        {
            // Circuit already disconnected, nothing to clean up
        }
    }
}

public class VideoMetadata
{
    public double Duration { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public long FileSize { get; set; }

    // Container / general
    public string? Container { get; set; }

    // Video track
    public string? FrameRate { get; set; }
    public string? VideoCodec { get; set; }
    public string? VideoBitrate { get; set; }
    public string? BitDepth { get; set; }
    public string? ColorSpace { get; set; }
    public string? Hdr { get; set; }
    public string? ChromaSubsampling { get; set; }

    // Audio track
    public string? AudioCodec { get; set; }
    public string? AudioBitrate { get; set; }
    public string? AudioSampleRate { get; set; }
    public string? AudioChannels { get; set; }

    // Populated when MediaInfo analysis fails.
    public string? AnalysisError { get; set; }
}