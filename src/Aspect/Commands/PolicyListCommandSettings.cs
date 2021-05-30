using System.ComponentModel;
using Spectre.Console.Cli;

namespace Aspect.Commands
{
    internal class PolicyListCommandSettings : PolicyDirectorySettings
    {
        [Description("Recurses through child directories to find policies.")]
        [CommandOption("-r|--recursive")]
        public bool? Recursive { get; init; }
    }
}