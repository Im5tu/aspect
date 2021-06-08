using System.Threading.Tasks;
using Aspect.Abstractions;
using Spectre.Console.Cli;

namespace Aspect.Commands
{
    internal class SelectResourceCommandStage<T> : CommandStage<T>
        where T : CommandSettings
    {
        internal override Task<int?> InvokeAsync(IExecutionData data, T settings)
        {
            var provider = data.Get<ICloudProvider>(CommandStageConstants.CloudProvider);
            var resources = provider.GetResources();
            var resource = data.AskToSelect(CommandStageConstants.ResourceName, "Select resource:", resources.Keys);
            data.Set(CommandStageConstants.ResourceType, resources[resource]);
            return Task.FromResult<int?>(null);
        }
    }
}
