using Microsoft.AspNetCore.Components;

namespace Playground.FrontEnd.Components.TabControl
{
	public partial class TabMenu
	{
		[CascadingParameter]
		private TabContainer Parent { get; set; } = default!;

		[Parameter]
		public string Class { get; set; }

		private ElementReference TabsWrapperRef;

		private void HandleTabClick(TabView tab)
		{
			Parent.SetActive(tab);
		}
	}
}
