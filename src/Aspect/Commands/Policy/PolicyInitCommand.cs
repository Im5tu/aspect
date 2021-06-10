using System.Collections.Generic;
using System.ComponentModel;
using Aspect.Abstractions;
using Aspect.Policies.Suite;
using Spectre.Console.Cli;

namespace Aspect.Commands.Policy
{
    internal sealed class PolicyInitCommand : ExecutableCommand<PolicyInitCommand.Settings>
    {
        internal class Settings : FileSettings
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

        private readonly IReadOnlyDictionary<string, ICloudProvider> _cloudProviders;
        private readonly IPolicySuiteSerializer _policySuiteSerializer;

        public PolicyInitCommand(IReadOnlyDictionary<string, ICloudProvider> cloudProviders, IPolicySuiteSerializer policySuiteSerializer)
        {
            _cloudProviders = cloudProviders;
            _policySuiteSerializer = policySuiteSerializer;
        }

        protected override IEnumerable<CommandStage<Settings>> GetCommandStages(Settings settings)
        {
            if (settings.InitializeSuite.GetValueOrDefault(false))
                yield return new PolicySuiteInitCommandStage(_policySuiteSerializer);
            else
            {
                if (string.IsNullOrWhiteSpace(settings.Resource))
                {
                    yield return new SelectCloudProviderCommandStage<Settings>(_cloudProviders);
                    yield return new SelectResourceCommandStage<Settings>();
                }

                yield return new PolicyInitFileCommandStage();
            }
        }
    }
}
