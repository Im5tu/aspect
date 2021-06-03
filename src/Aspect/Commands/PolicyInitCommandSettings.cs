using System.ComponentModel;
using Spectre.Console.Cli;

namespace Aspect.Commands
{
    internal class PolicyInitCommandSettings : PolicyFileSettings
    {
        [Description("The resource to build the policy for")]
        [CommandOption("-r|--resource")]
        public string? Resource { get; init; }
    }
}
