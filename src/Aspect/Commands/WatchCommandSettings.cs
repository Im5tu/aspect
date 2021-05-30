using System.ComponentModel;
using Spectre.Console.Cli;

namespace Aspect.Commands
{
    internal class WatchCommandSettings : PolicyDirectorySettings
    {
        [CommandOption("--delay")]
        [Description("The period of time in milliseconds that needs to elapse between changes to a file.")]
        public int? DelayInterval { get; init; }
    }
}