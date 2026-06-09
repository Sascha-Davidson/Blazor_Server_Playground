using Microsoft.AspNetCore.Components.Routing;

namespace Playground.FrontEnd.Components.Menu;
public partial class NavMenu
{
    public class NavItem
    {
        public required string Text { get; set; }
        public required string Url { get; set; }
        public string Icon { get; set; }
        public NavLinkMatch Match { get; set; } = NavLinkMatch.Prefix;
    }

    public List<NavItem> Navigation = new()
{
    new NavItem { Text = "Home", Url = "", Icon = "bi-house-door-fill-nav-menu", Match = NavLinkMatch.All },
    new NavItem { Text = "Higllight en bleur", Url = "HighlightendBleur", Icon = "bi-plus-square-fill-nav-menu" },
    new NavItem { Text = "Error component test", Url = "error/test", Icon = "bi-list-nested-nav-menu" },
    new NavItem { Text = "Dialog component", Url = "Dialog-Component", Icon = "bi-list-nested-nav-menu" },
    new NavItem { Text = "Editor test room", Url = "Editor-test-room", Icon = "bi-list-nested-nav-menu"},
};
}
