using System.Diagnostics.CodeAnalysis;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Aspect.Commands.old
{
    internal class AutoCompleteCommand : Command<AutoCompleteCommand.Settings>
    {
        internal class Settings : CommandSettings
        {
        }

        public override int Execute([NotNull] CommandContext context, [NotNull] AutoCompleteCommand.Settings settings)
        {
            // https://iridakos.com/programming/2018/03/01/bash-programmable-completion-tutorial
            // https://docs.microsoft.com/en-us/powershell/module/microsoft.powershell.core/register-argumentcompleter?view=powershell-7.1
            AnsiConsole.MarkupLine("[red bold]This command is not implemented yet[/]");
            return 1;
        }
    }
}
