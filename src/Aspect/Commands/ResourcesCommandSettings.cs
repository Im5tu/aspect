using Spectre.Console.Cli;

namespace Aspect.Commands
{
    internal sealed class ResourcesCommandSettings : CommandSettings
    {
        [CommandArgument(0, "[provider]")]
        public string? provider { get; init; }
    }
}