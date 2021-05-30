using System.Threading.Tasks;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Aspect.Commands
{
    internal class RunCommand : AsyncCommand<RunCommandSettings>
    {
        public override Task<int> ExecuteAsync(CommandContext context, RunCommandSettings settings)
        {
            AnsiConsole.MarkupLine("[red bold]This command is not implemented yet[/]");
            return Task.FromResult(1);
        }
    }
}