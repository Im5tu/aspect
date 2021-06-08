using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Aspect.Commands
{
    internal abstract class CommandStage<T>
        where T : CommandSettings
    {
        internal abstract Task<int?> InvokeAsync(IExecutionData data, T settings);
        internal virtual IEnumerable<string> GetHelpText() => Enumerable.Empty<string>();

        protected void Write(string text, Style style)
        {
            if (!text.EndsWith(Environment.NewLine, StringComparison.Ordinal))
                text += Environment.NewLine;

            AnsiConsole.Render(new Text(text, style));
        }
    }
}
