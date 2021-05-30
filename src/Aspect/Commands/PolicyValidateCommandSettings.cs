using System.ComponentModel;
using Spectre.Console.Cli;

namespace Aspect.Commands
{
    internal class PolicyValidateCommandSettings : PolicyFileOrDirectorySettings
    {
        [Description("Recurses through child directories to find policies.")]
        [CommandOption("-r|--recursive")]
        public bool? Recursive { get; init; }

        [Description("Only displays policies that failed to validate.")]
        [CommandOption("--failed-only")]
        public bool? FailedOnly { get; init; }
    }
}
