using System.Collections.Generic;
using System.Linq;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Aspect.Commands.AutoComplete
{
    internal class AutoCompleteCommand : ExecutableCommand<AutoCompleteCommand.Settings>
    {
        internal class Settings : CommandSettings
        {
        }

        protected override IEnumerable<CommandStage<Settings>> GetCommandStages(Settings commandSettings)
        {
            AnsiConsole.MarkupLine("[red bold]This command is not implemented yet[/]");
            return Enumerable.Empty<CommandStage<Settings>>();
        }
    }
}
