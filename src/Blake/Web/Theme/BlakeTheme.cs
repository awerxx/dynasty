using MudBlazor;

namespace Dynasty.Carrington.Blake.Web.Theme;

public static class BlakeTheme
{
    public static readonly MudTheme Theme = new()
    {
        PaletteDark = new PaletteDark
        {
            Primary = "#7aa2f7",
            Secondary = "#bb9af7",
            Tertiary = "#7dcfff",
            Background = "#0f1115",
            Surface = "#181b22",
            AppbarBackground = "#181b22",
            AppbarText = "#e6e8eb",
            DrawerBackground = "#13161c",
            DrawerText = "#e6e8eb",
            DrawerIcon = "#9aa4b2",
            Success = "#3fb950",
            Error = "#f85149",
            Warning = "#d29922",
            Info = "#58a6ff",
            TextPrimary = "#e6e8eb",
            TextSecondary = "#9aa4b2",
            TextDisabled = "#6b7480",
            ActionDefault = "#9aa4b2",
            LinesDefault = "#2a2f3a",
            LinesInputs = "#3a4150",
            TableLines = "#2a2f3a",
            TableStriped = "#1c2028",
            TableHover = "#222733",
            Divider = "#2a2f3a"
        },
        LayoutProperties = new LayoutProperties
        {
            DefaultBorderRadius = "8px"
        }
    };
}
