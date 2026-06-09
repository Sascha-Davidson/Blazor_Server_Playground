using Playground.Lib.Enums;
using Microsoft.AspNetCore.Components;

namespace Playground.FrontEnd.Components.Icons;

public partial class Icon
{
    public enum IconType
    {
        Authorization,
        Bell,
        Close,
        Cross,
        Contact,
        DashBoard,
        Document,
        Home,
        Information,
        LogOut,
        Note,
        Plus,
        Search,
        Setting,
        User,
    }

    private const string Fill = "inherit";

    [Parameter]
    public string Color { get; set; } = "black";

    [Parameter]
    public IconType Type { get; set; }

    [Parameter]
    public string Title { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object> Attributes { get; set; }

    [Parameter]
    public string AdditionalClasses { get; set; }

    [Parameter]
    public bool? SuperUser { get; set; }

    private string GetCssClasses()
    {
        var classes = new List<string>();
        if (!string.IsNullOrWhiteSpace(AdditionalClasses))
            classes.Add(AdditionalClasses);

        if (SuperUser == true)
            classes.Add("super-user");

        classes.Add(Type.GetDisplayValue());

        return string.Join(" ", classes);
    }
}
