let videoA;
let videoB;
let dotNetRef;

const MEDIAINFO_VERSION = "0.3.4";
let mediaInfoFactoryPromise;

async function getMediaInfoFactory() {
    if (!mediaInfoFactoryPromise) {
        // jsDelivr's "+esm" endpoint bundles all internal dependencies into a single
        // importable module, which works reliably with browser dynamic import.
        // Fall back to unpkg if jsDelivr is unavailable.
        mediaInfoFactoryPromise = import(
            `https://cdn.jsdelivr.net/npm/mediainfo.js@${MEDIAINFO_VERSION}/+esm`
        )
            .catch(() => import(
                `https://unpkg.com/mediainfo.js@${MEDIAINFO_VERSION}/dist/esm/index.js`
            ))
            .then(m => m.default ?? m.mediaInfoFactory ?? m);
    }
    return mediaInfoFactoryPromise;
}

function pick(track, ...keys) {
    for (const key of keys) {
        const value = track[key];
        if (value !== undefined && value !== null && value !== "") {
            return value;
        }
    }
    return null;
}

function formatBitrate(bps) {
    const n = Number(bps);
    if (!Number.isFinite(n) || n <= 0)
        return null;

    if (n >= 1_000_000)
        return (n / 1_000_000).toFixed(2) + " Mbps";

    return Math.round(n / 1000) + " kbps";
}

function formatChannels(count, layout) {
    const n = Number(count);
    if (layout)
        return layout;
    if (n === 1) return "Mono";
    if (n === 2) return "Stereo";
    if (Number.isFinite(n) && n > 0) return n + " ch";
    return null;
}

function extractFrameRate(video) {
    // MediaInfo may expose the frame rate under different fields depending on the container.
    let fps = Number(pick(video, "FrameRate", "FrameRate_Original", "FrameRate_Nominal"));

    // Fall back to numerator/denominator pair (common for MKV/WebM).
    if (!Number.isFinite(fps) || fps <= 0) {
        const num = Number(pick(video, "FrameRate_Num"));
        const den = Number(pick(video, "FrameRate_Den"));
        if (Number.isFinite(num) && Number.isFinite(den) && den > 0)
            fps = num / den;
    }

    if (!Number.isFinite(fps) || fps <= 0)
        return null;

    const mode = pick(video, "FrameRate_Mode"); // "CFR" / "VFR"
    const suffix = mode === "VFR" ? " fps (VFR)" : " fps";
    return fps.toFixed(3) + suffix;
}

function detectHdr(video) {
    const hdrFormat = pick(video, "HDR_Format", "HDR_Format_String", "HDR_Format_Commercial");
    if (hdrFormat)
        return hdrFormat;

    const transfer = pick(video, "transfer_characteristics", "transfer_characteristics_Original");
    if (transfer) {
        if (/2084|PQ/i.test(transfer)) return "HDR10 (PQ)";
        if (/HLG/i.test(transfer)) return "HLG";
    }
    return "SDR";
}

async function analyzeMedia(arrayBuffer) {
    try {
        const factory = await getMediaInfoFactory();

        const mediaInfo = await factory({
            format: "object",
            locateFile: (path) =>
                `https://cdn.jsdelivr.net/npm/mediainfo.js@${MEDIAINFO_VERSION}/dist/${path}`
        });

        const getSize = () => arrayBuffer.byteLength;
        const readChunk = (chunkSize, offset) =>
            new Uint8Array(arrayBuffer, offset, chunkSize);

        const result = await mediaInfo.analyzeData(getSize, readChunk);
        mediaInfo.close();

        // Log the raw parsed tracks so unexpected field names can be diagnosed.
        console.log("MediaInfo result:", result);

        const tracks = result?.media?.track ?? [];
        const general = tracks.find(t => t["@type"] === "General") ?? {};
        const video = tracks.find(t => t["@type"] === "Video") ?? {};
        const audio = tracks.find(t => t["@type"] === "Audio") ?? {};

        return {
            container: pick(general, "Format"),
            frameRate: extractFrameRate(video),
            videoCodec: pick(video, "Format", "CodecID"),
            videoBitrate: formatBitrate(pick(video, "BitRate", "BitRate_Nominal")),
            bitDepth: pick(video, "BitDepth") ? `${pick(video, "BitDepth")}-bit` : null,
            colorSpace: pick(video, "colour_primaries", "ColorSpace"),
            hdr: detectHdr(video),
            chromaSubsampling: pick(video, "ChromaSubsampling"),
            audioCodec: pick(audio, "Format", "CodecID"),
            audioBitrate: formatBitrate(pick(audio, "BitRate", "BitRate_Nominal")),
            audioSampleRate: pick(audio, "SamplingRate")
                ? `${(Number(pick(audio, "SamplingRate")) / 1000).toFixed(1)} kHz`
                : null,
            audioChannels: formatChannels(pick(audio, "Channels"), pick(audio, "ChannelLayout"))
        };
    } catch (e) {
        console.error("MediaInfo analysis failed:", e);
        return { analysisError: (e && e.message) ? e.message : String(e) };
    }
}

function reportTime() {
    if (!dotNetRef)
        return;

    const tA = videoA && Number.isFinite(videoA.currentTime) ? videoA.currentTime : 0;
    const tB = videoB && Number.isFinite(videoB.currentTime) ? videoB.currentTime : 0;

    dotNetRef.invokeMethodAsync("OnTimeUpdate", tA, tB);
}

// Estimate frame rate natively using requestVideoFrameCallback by measuring the
// mediaTime delta between two consecutive presented frames. Used as a fallback
// when MediaInfo does not report a frame rate.
function measureFrameRate(videoElement) {
    return new Promise(resolve => {
        if (typeof videoElement.requestVideoFrameCallback !== "function") {
            resolve(null);
            return;
        }

        let last = null;
        const deltas = [];
        let finished = false;

        // Briefly play muted so frames get presented, then restore the original state.
        const wasMuted = videoElement.muted;
        const wasPaused = videoElement.paused;
        videoElement.muted = true;

        const cleanup = (fps) => {
            if (finished)
                return;
            finished = true;

            if (wasPaused)
                videoElement.pause();
            videoElement.muted = wasMuted;
            videoElement.currentTime = 0;

            resolve(fps);
        };

        const onFrame = (_now, metadata) => {
            if (finished)
                return;

            if (last !== null) {
                const dt = metadata.mediaTime - last;
                if (dt > 0)
                    deltas.push(dt);
            }
            last = metadata.mediaTime;

            if (deltas.length >= 5) {
                deltas.sort((a, b) => a - b);
                const median = deltas[Math.floor(deltas.length / 2)];
                const fps = 1 / median;
                cleanup(Number.isFinite(fps) && fps > 0 ? fps.toFixed(3) + " fps (est.)" : null);
                return;
            }

            videoElement.requestVideoFrameCallback(onFrame);
        };

        videoElement.requestVideoFrameCallback(onFrame);
        videoElement.play().catch(() => cleanup(null));

        // Give up after a short time if not enough frames are decoded.
        setTimeout(() => cleanup(null), 2000);
    });
}

export async function setVideoSource(videoElement, streamReference, contentType) {

    if (!videoElement) {
        console.error("setVideoSource: videoElement is null/undefined");
        return { duration: 0, width: 0, height: 0 };
    }

    const arrayBuffer = await streamReference.arrayBuffer();

    // Fall back to a generic mp4 type if the browser reports an empty/unknown content type,
    // otherwise the blob may fail to decode and the <video> stays blank.
    const type = contentType && contentType.length > 0 ? contentType : "video/mp4";

    const blob = new Blob(
        [arrayBuffer],
        { type: type }
    );

    // Release any previously assigned object URL to avoid leaks.
    if (videoElement.dataset.objectUrl) {
        URL.revokeObjectURL(videoElement.dataset.objectUrl);
    }

    const url = URL.createObjectURL(blob);
    videoElement.dataset.objectUrl = url;

    videoElement.preload = "auto";
    videoElement.src = url;
    videoElement.load();

    // Analyze the media file for detailed properties (codec, bitrate, HDR, audio, ...).
    const analysisPromise = analyzeMedia(arrayBuffer);

    return new Promise(resolve => {

        let settled = false;

        const finish = async () => {
            if (settled)
                return;
            settled = true;

            const details = await analysisPromise;

            // If MediaInfo could not determine the frame rate, estimate it natively.
            if (!details.frameRate) {
                const estimated = await measureFrameRate(videoElement);
                if (estimated)
                    details.frameRate = estimated;
            }

            resolve({
                duration: Number.isFinite(videoElement.duration) ? videoElement.duration : 0,
                width: videoElement.videoWidth,
                height: videoElement.videoHeight,
                ...details
            });
        };

        videoElement.onloadedmetadata = () => {
            // Nudge currentTime so the browser paints the first frame instead of a blank box.
            try {
                videoElement.currentTime = 0;
            } catch { /* ignore */ }

            finish();
        };

        videoElement.onerror = () => {
            const err = videoElement.error;
            console.error(
                "Video failed to load.",
                "code:", err ? err.code : "unknown",
                "message:", err ? err.message : "",
                "contentType:", type);
            finish();
        };
    });
}

export function initialize(dotNetReference, videoAElement, videoBElement) {
    dotNetRef = dotNetReference;
    videoA = videoAElement;
    videoB = videoBElement;

    // Report playback position back to .NET so the frame counters stay in sync.
    for (const v of [videoA, videoB]) {
        if (!v)
            continue;

        v.addEventListener("timeupdate", reportTime);
        v.addEventListener("seeked", reportTime);
    }
}

export async function playPause(videoAElement, videoBElement, offset) {

    if (!videoAElement || !videoBElement)
        return;

    if (!Number.isFinite(offset)) {
        offset = 0;
    }

    if (!Number.isFinite(videoAElement.currentTime)) {
        videoAElement.currentTime = 0;
    }

    if (!videoAElement.duration || !Number.isFinite(videoAElement.duration)) {
        console.warn("Video A is not loaded yet");
        return;
    }

    const targetTime = videoAElement.currentTime + offset;

    videoBElement.currentTime = Math.max(0, targetTime);


    if (videoAElement.paused) {

        await Promise.all([
            videoAElement.play(),
            videoBElement.play()
        ]);

    } else {

        videoAElement.pause();
        videoBElement.pause();

    }
}

export function stop() {
    if (!videoA || !videoB)
        return;

    videoA.pause();
    videoB.pause();

    videoA.currentTime = 0;
    videoB.currentTime = 0;

    reportTime();
}

export function syncSeek(offset) {
    if (!videoA || !videoB)
        return;

    if (!Number.isFinite(offset))
        offset = 0;

    videoB.currentTime = Math.max(0, videoA.currentTime + offset);

    reportTime();
}

export function registerSync(offset) {
    if (!videoA || !videoB)
        return;

    videoA.addEventListener("seeked", () => {
        videoB.currentTime = videoA.currentTime + offset;
    });
}