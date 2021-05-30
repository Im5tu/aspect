using Spectre.Console.Cli;

namespace Aspect.Commands
{
    internal class PolicyFileSettings : CommandSettings
    {
        [CommandArgument(0, "[filename]")]
        public string? FileName { get; init; }
    }
}