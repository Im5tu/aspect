using Spectre.Console.Cli;

namespace Aspect.Commands
{
    internal class PolicyFileOrDirectorySettings : CommandSettings
    {
        [CommandArgument(0, "[file or directory]")]
        public string? FileOrDirectory { get; init; }
    }
}
