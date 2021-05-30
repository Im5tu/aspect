using System.ComponentModel;
using Spectre.Console.Cli;

namespace Aspect.Commands
{
    internal class PolicyDirectorySettings : CommandSettings
    {
        [Description("The directory to use")]
        [CommandArgument(0, "[directory]")]
        public string? Directory { get; init; }
    }
}
