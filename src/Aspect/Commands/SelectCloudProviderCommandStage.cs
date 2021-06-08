using System.Collections.Generic;
using System.Threading.Tasks;
using Aspect.Abstractions;
using Spectre.Console.Cli;

namespace Aspect.Commands
{
    internal class SelectCloudProviderCommandStage<T> : CommandStage<T>
        where T : CommandSettings
    {
        private readonly IReadOnlyDictionary<string, ICloudProvider> _cloudProviders;

        public SelectCloudProviderCommandStage(IReadOnlyDictionary<string, ICloudProvider> cloudProviders)
        {
            _cloudProviders = cloudProviders;
        }

        internal override Task<int?> InvokeAsync(IExecutionData data, T settings)
        {
            var providerKey = data.AskToSelect("Select cloud provider:", _cloudProviders.Keys);

            data.Set(CommandStageConstants.CloudProvider, _cloudProviders[providerKey]);
            return Task.FromResult<int?>(null);
        }
    }
}
