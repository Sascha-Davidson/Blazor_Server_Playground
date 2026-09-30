using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Playground.FrontEnd.Base.Functions;
using Playground.Lib.Helper;
using Playground.Services;
using Playground.Templating.Email;

namespace Playground.FrontEnd.Base
{
    public class BaseComponent : ComponentBase, IAsyncDisposable
    {
        [Inject] protected IJSRuntime JsRuntime { get; set; } = null!;
        [Inject] protected ToastService ToastService { get; set; } = null!;
        [Inject] protected NavigationManager Navigation { get; set; } = null!;
        [Inject] protected IDialogService DialogService { get; set; } = null!;
        [Inject] protected Mail Mail { get; set; } = null!;
        [Inject] protected IHttpContextAccessor HttpContextAccessor { get; set; } = null!;
        
        private QueryHelper _queryHelper;
        private BreakPoint _breakPoint;
        private Func<Task> _breakPointChangedHandler;

        protected string UserAgent =>
            HttpContextAccessor.HttpContext?.Request.Headers.UserAgent.ToString() ?? string.Empty;

        protected bool IsMobile => Utils.IsMobileUserAgent(UserAgent);

        protected QueryHelper QueryHelper => _queryHelper ??= new QueryHelper(Navigation);

        protected int CurrentWidth => _breakPoint?.CurrentWidth ?? 0;

        [Parameter]
        public int DefaultBreakpoint { get; set; } = 800;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (!firstRender)
                return;

            //_deviceDetected ??= new DeviceDetected(JsRuntime);
            //await _deviceDetected.DetectAsync();

            if (_breakPoint == null)
            {
                _breakPoint = new BreakPoint(JsRuntime);
                _breakPointChangedHandler = async () => await InvokeAsync(StateHasChanged);
                _breakPoint.OnChange += _breakPointChangedHandler;

                await _breakPoint.DetectAsync(DefaultBreakpoint);
            }

            await InvokeAsync(StateHasChanged);
        }
        
        protected IJSObjectReference JsModule { get; private set; }
        protected async Task ScopedJs()
        {
            if (JsModule is not null)
                return;
            Type type = GetType();

            string namespaceName = type.Namespace;

            if (string.IsNullOrWhiteSpace(namespaceName))
                throw new InvalidOperationException(
                    $"Cannot determine namespace for {type.Name}"
                );

            string rootNamespace = type.Assembly.GetName().Name!;

            string namespacePath = namespaceName
                .Replace(rootNamespace, "")
                .Trim('.')
                .Replace(".", "/");

            string className = type.Name.Split('`')[0];

            string path = $"./{namespacePath}/{className}.razor.js";

            JsModule = await JsRuntime.InvokeAsync<IJSObjectReference>(
                "import",
                path
            );
        }

        public async ValueTask DisposeAsync()
        {
            if (_breakPoint != null && _breakPointChangedHandler != null)
            {
                _breakPoint.OnChange -= _breakPointChangedHandler;
                await _breakPoint.DisposeAsync();
            }

            if (JsModule is not null)
            {
                try
                {
                    await JsModule.DisposeAsync();
                }
                catch (JSDisconnectedException)
                {
                    // Circuit already disconnected, nothing to clean up
                }
            }
        }
    }
    
    

}
