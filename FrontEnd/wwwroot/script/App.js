window.getBrowserCulture = () => {
    return navigator.language || navigator.userLanguage;
};

window.isDesktop = () => {
    const ua = navigator.userAgent;
    // Treat phones and tablets as non-desktop
    const isMobileOrTablet = /Mobi|Android|iPhone|iPad|iPod|Tablet/i.test(ua);
    return !isMobileOrTablet;
}

window.selectInputText = (element) => {
    if (!element) return;

    element.focus();

    if (typeof element.select === 'function') {
        element.select();
    }
};

window.copyToClipboard = async (text) => {
    await navigator.clipboard.writeText(text);
};

window.responsiveLayout = (function () {

    const observers = new Map();

    function registerContainer(selector, dotNetRef, breakpoint) {
        const element = document.querySelector(selector);
        if (!element) return;

        const observer = new ResizeObserver(entries => {
            for (const entry of entries) {
                const width = Math.round(entry.contentRect.width);
                dotNetRef.invokeMethodAsync("CheckBreakpoint", width);
            }
        });

        observer.observe(element);
        observers.set(selector, observer);
    }

    function unregisterContainer(selector) {
        const observer = observers.get(selector);
        if (observer) {
            observer.disconnect();
            observers.delete(selector);
        }
    }

    return {
        registerContainer,
        unregisterContainer
    };
})();

window.videoHelper = {
    play: (video) => video.play(),
    pause: (video) => video.pause(),
    isPaused: (video) => video.paused,
    getCurrentTime: (video) => video.currentTime,
    getDuration: (video) => video.duration || 0,
    seek: (video, time) => { video.currentTime = time; },

    attachEvents: (video, dotNetRef) => {
        // Time played
        video.addEventListener("timeupdate", () => {
            dotNetRef.invokeMethodAsync("UpdateProgress", video.currentTime, video.duration);
        });

        // Buffer progress
        video.addEventListener("progress", () => {
            if (video.buffered.length > 0) {
                let bufferedEnd = video.buffered.end(video.buffered.length - 1);
                dotNetRef.invokeMethodAsync("UpdateBuffer", bufferedEnd, video.duration);
            }
        });

        // Ready state (loading complete or not)
        video.addEventListener("loadeddata", () => {
            dotNetRef.invokeMethodAsync("UpdateLoad", video.readyState, video.duration);
        });
    }
};

window.videoHelper.attachTimeUpdate = (video, dotNetRef) => {
    window.videoHelper.attachEvents(video, dotNetRef);
};

window.progressInterop = {
    getRect: (element) => {
        if (!element) return null;
        const rect = element.getBoundingClientRect();
        return { left: rect.left, top: rect.top, width: rect.width, height: rect.height };
    },

    registerMouseEvents: (dotnetHelper) => {
        // Define handlers once
        const move = (e) => {
            dotnetHelper.invokeMethodAsync("OnMouseMove", e.clientX);
        };
        const up = (e) => {
            dotnetHelper.invokeMethodAsync("OnMouseUp", e.clientX);
            window.removeEventListener("mousemove", move);
            window.removeEventListener("mouseup", up);
        };

        // Save references so unregister can clean them
        window._progressMove = move;
        window._progressUp = up;

        window.addEventListener("mousemove", move);
        window.addEventListener("mouseup", up);
    },

    unregisterMouseEvents: () => {
        if (window._progressMove) {
            window.removeEventListener("mousemove", window._progressMove);
            window._progressMove = null;
        }
        if (window._progressUp) {
            window.removeEventListener("mouseup", window._progressUp);
            window._progressUp = null;
        }
    }
};

window.videoPlayer = {
    toggleFullScreen: function (element) {
        if (!document.fullscreenElement) {
            element.requestFullscreen?.();
        } else {
            document.exitFullscreen?.();
        }
    }
};


window.blazorDialog = {
    _handlers: new WeakMap(),

    open: function (element, dotNetHelper) {
        if (element && typeof element.showModal === 'function') {
            element.showModal();

            var handler = function (e) {
                var rect = element.getBoundingClientRect();
                if (e.clientX < rect.left || e.clientX > rect.right ||
                    e.clientY < rect.top || e.clientY > rect.bottom) {
                    dotNetHelper.invokeMethodAsync('OnBackdropClick');
                }
            };
            window.blazorDialog._handlers.set(element, handler);
            element.addEventListener('click', handler);
        }
    },

    close: function (element) {
        if (element && typeof element.close === 'function') {
            var handler = window.blazorDialog._handlers.get(element);
            if (handler) {
                element.removeEventListener('click', handler);
                window.blazorDialog._handlers.delete(element);
            }
            element.close();
        }
    }
};

window.showPopover = (inputEl, popoverEl) => {
    if (popoverEl && popoverEl.showPopover) {
        popoverEl.showPopover();

        // Position the popover relative to the input element
        const rect = inputEl.getBoundingClientRect();
        popoverEl.style.top = `${rect.bottom + window.scrollY}px`;
        popoverEl.style.left = `${rect.left + window.scrollX}px`;
        popoverEl.style.width = `${rect.width}px`;
    }
};

window.hidePopover = (popoverEl) => {
    if (popoverEl && popoverEl.hidePopover) {
        try {
            popoverEl.hidePopover();
        } catch (e) {
            // Popover might already be hidden
            console.debug('Popover already hidden');
        }
    }
};

document.addEventListener("keydown", function (e) {
    const el = e.target;

    if (!el.matches('input[data-datatype="currency"]')) return;

    const allowedKeys = [
        "Backspace", "Tab", "ArrowLeft", "ArrowRight", "Delete"
    ];

    if (allowedKeys.includes(e.key)) return;

    // allow digits and allowed symbols
    if (!/[0-9.,\/]/.test(e.key)) {
        e.preventDefault();
    }
});