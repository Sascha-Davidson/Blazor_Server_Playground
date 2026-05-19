using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Playground.FrontEnd.Base.Functions;
using Playground.Services;
using System.Linq.Expressions;
using Playground.Lib.Extensions;

namespace Playground.FrontEnd.Base
{
    public class BaseComponent : ComponentBase, IAsyncDisposable
    {
        [Inject] protected IJSRuntime JsRuntime { get; set; } = default!;
        [Inject] protected ToastService ToastService { get; set; } = default!;
        [Inject] protected NavigationManager Navigation { get; set; } = default!;
        [Inject] protected IDialogService DialogService { get; set; } = default!;

        private DeviceDetected? _deviceDetected;
        private QueryHelper? _queryHelper;
        private BreakPoint? _breakPoint;
        private Func<Task>? _breakPointChangedHandler;

        protected bool IsDesktop =>
            _deviceDetected is { IsDetected: true, ShowDesktop: true };

        protected QueryHelper QueryHelper => _queryHelper ??= new QueryHelper(Navigation);

        protected int CurrentWidth => _breakPoint?.CurrentWidth ?? 0;
        
        [Parameter]
        public int DefaultBreakpoint { get; set; } = 800;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (!firstRender)
                return;

            _deviceDetected ??= new DeviceDetected(JsRuntime);
            await _deviceDetected.DetectAsync();

            if (_breakPoint == null)
            {
                _breakPoint = new BreakPoint(JsRuntime);
                _breakPointChangedHandler = async () => await InvokeAsync(StateHasChanged);
                _breakPoint.OnChange += _breakPointChangedHandler;

                await _breakPoint.DetectAsync(DefaultBreakpoint);
            }

            await InvokeAsync(StateHasChanged);
        }

        public async ValueTask DisposeAsync()
        {
            if (_breakPoint != null && _breakPointChangedHandler != null)
            {
                _breakPoint.OnChange -= _breakPointChangedHandler;
                await _breakPoint.DisposeAsync();
            }
        }
    }

    public class EditorBase<T> : ComponentBase
    {
        [Parameter]
        public T? Value { get; set; }

        [Parameter]
        public EventCallback<T?> ValueChanged { get; set; }

        [Parameter]
        public Expression<Func<T>>? Expression { get; set; }

        [Parameter]
        public bool Required { get; set; }

        [Parameter] 
        public int ID { get; set; } = Guid.NewGuid().GetHashCode();

        [Parameter]
        public string? Name { get; set; }

        [Parameter]
        public string? PlaceHolder { get; set; }

        [Parameter]
        public bool ReadOnly { get; set; }

        [Parameter]
        public bool Disabled { get; set; }

        [Parameter(CaptureUnmatchedValues = true)]
        public Dictionary<string, object>? Attributes { get; set; }

        [Parameter]
        public IEnumerable<T> SelectList { get; set; } = [];

        [Parameter, EditorRequired]
        public Func<T, string> ValueSelector { get; set; } = _ => string.Empty;

        [Parameter, EditorRequired]
        public RenderFragment<T> OptionContent { get; set; } = item => builder => builder.AddContent(0, item?.ToString());
    }
}
