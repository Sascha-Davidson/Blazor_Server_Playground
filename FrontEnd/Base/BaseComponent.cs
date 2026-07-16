using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Playground.FrontEnd.Base.Functions;
using Playground.Lib.Enums;
using Playground.Lib.Extensions;
using Playground.Services;
using Playground.Templating.Email;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;

namespace Playground.FrontEnd.Base
{
    public class BaseComponent : ComponentBase, IAsyncDisposable
    {
        [Inject] protected IJSRuntime JsRuntime { get; set; } = null!;
        [Inject] protected ToastService ToastService { get; set; } = null!;
        [Inject] protected NavigationManager Navigation { get; set; } = null!;
        [Inject] protected IDialogService DialogService { get; set; } = null!;
        [Inject] protected Mail Mail { get; set; } = null!;

        private DeviceDetected _deviceDetected;
        private QueryHelper _queryHelper;
        private BreakPoint _breakPoint;
        private Func<Task> _breakPointChangedHandler;

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
        
        protected IJSObjectReference? JsModule { get; private set; }
        protected async Task ScopedJs()
        {
            if (JsModule is not null)
                return;
            Type type = GetType();

            string? namespaceName = type.Namespace;

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

    public class EditorBase<T> : ComponentBase
    {
        [Parameter, EditorRequired]
        public T Value { get; set; }

        [Parameter]
        public EventCallback<T> ValueChanged { get; set; }

        [Parameter, EditorRequired]
        public Expression<Func<T>> Expression { get; set; }

        [Parameter]
        public bool Required { get; set; }

        [Parameter]
        public string AutoComplete { get; set; }

        public string AutoCompleteValue =>
            !string.IsNullOrWhiteSpace(AutoComplete)
                ? AutoComplete
                : ExpressionMember?
                    .GetCustomAttribute<AutoCompleteAttribute>()?
                    .Type
                    .ToHtmlValue();

        [Parameter]
        public int ID { get; set; } = Guid.NewGuid().GetHashCode();

        [Parameter]
        public string Name { get; set; }

        [Parameter]
        public string PlaceHolder { get; set; }

        [Parameter]
        public bool ReadOnly { get; set; }

        [Parameter]
        public bool Disabled { get; set; }

        [Parameter]
        public T MinValue { get; set; }

        [Parameter]
        public T MaxValue { get; set; }

        [Parameter(CaptureUnmatchedValues = true)]
        public Dictionary<string, object> Attributes { get; set; }

        [Parameter]
        public IEnumerable<T> SelectList { get; set; } = [];

        [Parameter]
        public Func<T, string> ValueSelector { get; set; } = _ => string.Empty;

        [Parameter]
        public RenderFragment<T> OptionContent { get; set; } = item => builder => builder.AddContent(0, item?.ToString());

        [Parameter]
        public CheckboxStyle CheckboxStyle { get; set; } = CheckboxStyle.Box;

        [Parameter]
        public CheckboxStyle Style { get; set; } = CheckboxStyle.Box;

        public MemberInfo ExpressionMember => (Expression?.Body as MemberExpression)?.Member;

        public DataTypeAttribute DataTypeAttribute => ExpressionMember?.GetCustomAttribute<DataTypeAttribute>();
        public RangeAttribute RangeAttribute => ExpressionMember?.GetCustomAttribute<RangeAttribute>();
        public MaxLengthAttribute MaxLengthAttribute => ExpressionMember?.GetCustomAttribute<MaxLengthAttribute>();
        public bool IsRequired =>
            Required || (ExpressionMember?.GetCustomAttributes<RequiredAttribute>(true).Any() ?? false);
        
        public decimal? RangeMin => (RangeAttribute?.Minimum != null ? Convert.ToDecimal(RangeAttribute.Minimum) : Convert.ToDecimal(MinValue)).NullIfEquals(0);
        public decimal? RangeMax => (RangeAttribute?.Maximum != null ? Convert.ToDecimal(RangeAttribute.Maximum) : Convert.ToDecimal(MaxValue)).NullIfEquals(0);

        private readonly Type _modelType = typeof(T);
        public string EditorKey => (DataTypeAttribute?.CustomDataType ?? DataTypeAttribute?.DataType.ToString() ?? _modelType.Name)?.ToLowerInvariant();

    }

    public class EditorRowBase<T> : EditorBase<T>
    {
        [Parameter]
        public string Label { get; set; }

        [Parameter]
        public bool? ForceBlazor { get; set; }

        [Parameter]
        public RowOrientation Orientation { get; set; } = RowOrientation.Auto;

        private string ExpressionName => ExpressionMember?.GetDisplayValue();
        public string LabelText => Label ?? ExpressionName;
    }
}
