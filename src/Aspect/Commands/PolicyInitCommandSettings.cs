using System.ComponentModel;
using Spectre.Console.Cli;

namespace Aspect.Commands
{
    internal class PolicyInitCommandSettings : PolicyFileSettings
    {
        [Description("The resource to build the policy for")]
        [CommandOption("-r|--resource")]
        public string? Resource { get; init; }

        [Description("Initialize a policy suite instead of a policy")]
        [CommandOption("-s|--suite")]
        public bool? InitializeSuite { get; init; }

        [Description("Display the generated policy in the console")]
        [CommandOption("-d|--display")]
        public bool? Display { get; init; }
    }
}
