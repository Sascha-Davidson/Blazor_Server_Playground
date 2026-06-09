using Microsoft.AspNetCore.Components;
using System.Collections.ObjectModel;

namespace Playground.FrontEnd.Components.TabControl
{
	public partial class TabContainer
    {
        [Inject] private NavigationManager Navigation { get; set; } = default!;


        [Parameter]
		public RenderFragment ChildContent { get; set; }

		[Parameter]
		public TabVariant Variant { get; set; } = TabVariant.Default;

		[Parameter]
		public object InitialPage { get; set; }

        [Parameter]
		public EventCallback<int> OnTabChanged { get; set; }

		[Parameter]
		public Func<int, bool> OnBeforeTabChange { get; set; }
		
		[Parameter]
		public bool AutoSortTabsByTabHeader { get; set; }

        [Parameter] public string TabKey { get; set; } // optional key for URL query
        private string UrlTabKey => !string.IsNullOrEmpty(TabKey) ? TabKey : "tab";

        [CascadingParameter] private TabView ParentTabView { get; set; }

		public void Refresh() => InvokeAsync(StateHasChanged);

		internal ObservableCollection<TabView> Tabs { get; } = [];
		internal TabView ActiveTab => Tabs.ElementAtOrDefault(ActiveTabIndex);
		internal int ActiveTabIndex { get; private set; } = 0;

        private string pendingInitialTab;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            ParentTabView?.ChildTabContainer = this;
        }

        protected override void OnParametersSet()
		{
			base.OnParametersSet();
            if (InitialPage is string title)
                pendingInitialTab = title;
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                SetInitialTab();
            }
        }

        private void SetInitialTab()
		{
			if (Tabs.Count == 0)
				return;

			var val = InitialPage switch
			{
				int i    => i,
				string s => Tabs.ToList().FindIndex(t => t.Tab == s),
				null     => 0,
				_        => throw new NotSupportedException()
			};

			if (val >= 0 && val < Tabs.Count)
				ActiveTabIndex = val;

            StateHasChanged();
        }

        internal void AddTab(TabView tab)
        {
            if (Tabs.Contains(tab))
                return;

            Tabs.Add(tab);

            if (!string.IsNullOrEmpty(pendingInitialTab))
            {
                var idx = Tabs.ToList().FindIndex(t => t.Title == pendingInitialTab);
                if (idx >= 0)
                {
                    ActiveTabIndex = idx;
                    pendingInitialTab = null;
                }
            }

            if (Tabs.Count == 1 && ActiveTabIndex == 0)
            {
                ActiveTabIndex = 0;
            }

            StateHasChanged();
        }

        internal void RemoveTab(TabView tab)
		{
			var index = Tabs.IndexOf(tab);
			if (index < 0)
				return;

			Tabs.RemoveAt(index);

			if (ActiveTabIndex >= Tabs.Count)
				ActiveTabIndex = Tabs.Count - 1;

			StateHasChanged();
		}

		private void ChangeTab(int index)
		{
			if (index < 0 || index >= Tabs.Count)
				return;

			var shouldChange = OnBeforeTabChange?.Invoke(index) ?? true;
			if (!shouldChange)
				return;

			ActiveTabIndex = index;
			OnTabChanged.InvokeAsync(index);

			if (Tabs[index].OnSelect.HasDelegate)
			{
				Tabs[index].OnSelect.InvokeAsync();
			}

            var uri = Navigation.ToAbsoluteUri(Navigation.Uri);
            var baseUri = uri.GetLeftPart(UriPartial.Path);
            var query = System.Web.HttpUtility.ParseQueryString(uri.Query);

            query[UrlTabKey] = Uri.EscapeDataString(Tabs[index].Tab);

            var newQuery = string.Join("&", query.AllKeys.Select(k => $"{k}={query[k]}"));
            var newUri = string.IsNullOrEmpty(newQuery) ? baseUri : $"{baseUri}?{newQuery}";

            Navigation.NavigateTo(newUri, forceLoad: false);

            StateHasChanged();
		}


		internal void SetActive(TabView tab)
		{
			var index = Tabs.IndexOf(tab);
			if (index < 0)
				return;

			ChangeTab(index);
			StateHasChanged();
		}
	}

	public enum TabVariant
	{
		Default = 0,
		VerticalSettings = 2,
		HorizontalSettings = 3,
	}
}
