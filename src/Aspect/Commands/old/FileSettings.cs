using Spectre.Console.Cli;

namespace Aspect.Commands.old
{
    internal class FileSettings : CommandSettings
    {
        [CommandArgument(0, "[filename]")]
        public string? FileName { get; init; }
    }
}
