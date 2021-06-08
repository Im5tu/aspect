using System.Threading.Tasks;
using Aspect.Abstractions;
using Spectre.Console.Cli;

namespace Aspect.Commands
{
    internal class SelectRegionsCommandStage<T> : CommandStage<T>
        where T : CommandSettings
    {
        internal override Task<int?> InvokeAsync(IExecutionData data, T settings)
        {
            var provider = data.Get<ICloudProvider>(CommandStageConstants.CloudProvider);
            _ = data.AskToSelectMultiple(CommandStageConstants.Regions, "Select the regions to work with:", provider.GetAllRegions(), true);
            return Task.FromResult<int?>(null);
        }
    }
}
