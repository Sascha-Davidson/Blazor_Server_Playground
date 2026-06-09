using Microsoft.JSInterop;

namespace Playground.FrontEnd.Base.Functions
{
    public sealed class BreakPoint : IAsyncDisposable
    {
        private readonly IJSRuntime _js;
        private DotNetObjectReference<BreakPoint> _objRef;
        private string _containerSelector;

        /// <summary>
        /// The currently detected container width in px
        /// </summary>
        public int CurrentWidth { get; private set; }

        /// <summary>
        /// Event triggered when container width changes
        /// </summary>
        public event Func<Task> OnChange;
        public BreakPoint(IJSRuntime js)
        {
            _js = js;
        }

        public async Task DetectAsync( int containerBreakpoint = 800, string containerSelector = ".main")
        {
            _containerSelector = containerSelector;
            _objRef = DotNetObjectReference.Create(this);

            await _js.InvokeVoidAsync( "responsiveLayout.registerContainer", containerSelector, _objRef, containerBreakpoint);
        }

        [JSInvokable]
        public async Task CheckBreakpoint(int containerWidth)
        {
            CurrentWidth = containerWidth;

            if (OnChange is not null)
                await OnChange.Invoke();
        }

        public async ValueTask DisposeAsync()
        {
            try { await _js.InvokeVoidAsync("responsiveLayout.unregisterContainer", ".main"); } catch { }
            _objRef?.Dispose();

            if (_containerSelector != null)
            {
                try
                {
                    await _js.InvokeVoidAsync( "responsiveLayout.unregisterContainer", _containerSelector);
                }
                catch { }
            }

            _objRef?.Dispose();
        }
    }
}