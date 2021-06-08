using System.ComponentModel;
using Spectre.Console.Cli;

namespace Aspect.Commands.old
{
    internal class DirectorySettings : CommandSettings
    {
        [Description("The directory to use")]
        [CommandArgument(0, "[directory]")]
        public string? Directory { get; init; }
    }
}
