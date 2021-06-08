using System.Collections.Generic;
using Aspect.Abstractions;
using Aspect.Policies.CompilerServices;
using Spectre.Console.Cli;

namespace Aspect.Commands.Inspect
{
    internal class InspectCommand : ExecutableCommand<InspectCommand.Settings>
    {
        internal class Settings : CommandSettings
        {
        }

        private readonly IReadOnlyDictionary<string, ICloudProvider> _cloudProviders;
        private readonly IPolicyCompiler _policyCompiler;

        public InspectCommand(IReadOnlyDictionary<string, ICloudProvider> cloudProviders, IPolicyCompiler policyCompiler)
        {
            _cloudProviders = cloudProviders;
            _policyCompiler = policyCompiler;
        }

        protected override IEnumerable<CommandStage<Settings>> GetCommandStages()
        {
            yield return new SelectCloudProviderCommandStage<Settings>(_cloudProviders);
            yield return new SelectRegionsCommandStage<Settings>();
            yield return new SelectResourceCommandStage<Settings>();
            yield return new InspectREPLCommandStage(_policyCompiler);
        }
    }
}
