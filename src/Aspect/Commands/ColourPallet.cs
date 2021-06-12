using Spectre.Console;

namespace Aspect.Commands
{
    internal static class ColourPallet
    {
        internal static readonly Style Primary = new(Color.Grey54, decoration: Decoration.Italic);
        internal static readonly Style Selection = new(Color.NavajoWhite1);
        internal static readonly Style Prompt = new(Color.NavajoWhite3);
    }
}
