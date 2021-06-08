using Spectre.Console;

namespace Aspect.Commands
{
    internal static class ColourPallet
    {
        internal static readonly Style Primary = new Style(Color.Grey54, decoration: Decoration.Italic);
        internal static readonly Style Selection = new Style(Color.NavajoWhite1);
        internal static readonly Style Prompt = new Style(Color.NavajoWhite3);
    }
}
