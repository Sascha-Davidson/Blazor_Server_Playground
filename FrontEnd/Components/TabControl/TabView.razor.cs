using Microsoft.AspNetCore.Components;

namespace Playground.FrontEnd.Components.TabControl
{
	public partial class TabView : ComponentBase
	{
		[CascadingParameter]
		public TabContainer Parent { get; set; } = default!;

		[Parameter] 
		public string Title { get; set; } = string.Empty;

		[Parameter] 
		public string Tab { get; set; } = string.Empty;
		[Parameter] 
		public RenderFragment ChildContent { get; set; }
		[Parameter] 
		public EventCallback OnSelect { get; set; }

		internal TabContainer ChildTabContainer { get; set; }

		protected override void OnInitialized()
		{
			base.OnInitialized();
			Parent.AddTab(this);
		}
	}
}
