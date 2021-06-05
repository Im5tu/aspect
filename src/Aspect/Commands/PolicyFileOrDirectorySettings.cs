using Spectre.Console.Cli;

namespace Aspect.Commands
{
    internal class PolicyFileOrDirectorySettings : CommandSettings
    {
        [CommandArgument(0, "[source]")]
        public string? Source { get; set; }
    }
}
