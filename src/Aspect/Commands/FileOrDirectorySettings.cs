using Spectre.Console.Cli;

namespace Aspect.Commands
{
    internal class FileOrDirectorySettings : CommandSettings
    {
        [CommandArgument(0, "[source]")]
        public string? Source { get; set; }
    }
}
