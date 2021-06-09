using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Aspect.Commands
{
    internal abstract class ExecutableCommand<T> : AsyncCommand<T>
        where T : CommandSettings
    {
        public sealed override async Task<int> ExecuteAsync([NotNull] CommandContext context, [NotNull] T settings)
        {
            foreach (var line in GetHelpText())
                Write(line, ColourPallet.Primary);

            var stages = GetCommandStages(settings).ToList();
            using var data = new ExecutionData();

            var index = 0;
            foreach (var stage in stages)
            {
                if (index++ > 0)
                    AnsiConsole.Clear();

                foreach (var line in stage.GetHelpText())
                    Write(line, ColourPallet.Primary);

                var exitCode = await stage.InvokeAsync(data, settings).ConfigureAwait(false);
                if (exitCode.HasValue)
                    return exitCode.Value;
            }

            return 0;
        }

        protected abstract IEnumerable<CommandStage<T>> GetCommandStages(T commandSettings);
        protected virtual IEnumerable<string> GetHelpText() => Enumerable.Empty<string>();
        protected void Write(string text, Style style)
        {
            if (!text.EndsWith(Environment.NewLine, StringComparison.Ordinal))
                text += Environment.NewLine;

            AnsiConsole.Render(new Text(text, style));
        }
    }
}
