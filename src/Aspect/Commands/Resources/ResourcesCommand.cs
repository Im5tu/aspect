using System.Collections.Generic;
using Aspect.Abstractions;
using Spectre.Console.Cli;

namespace Aspect.Commands.Resources
{
    internal sealed class ResourcesCommand : ExecutableCommand<ResourcesCommand.Settings>
    {
        internal sealed class Settings : CommandSettings
        {
            [CommandArgument(0, "[provider]")]
            public string? provider { get; init; }
        }

        private IReadOnlyDictionary<string,ICloudProvider> _cloudProviders;

        public ResourcesCommand(IReadOnlyDictionary<string, ICloudProvider> cloudProviders)
        {
            _cloudProviders = cloudProviders;
        }

        protected override IEnumerable<CommandStage<Settings>> GetCommandStages(Settings commandSettings)
        {
            yield return new SelectCloudProviderCommandStage<Settings>(_cloudProviders);
            yield return new ResourcesDisplayCommandStage();
        }
    }
}
