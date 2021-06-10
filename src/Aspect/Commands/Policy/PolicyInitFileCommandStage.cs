using System.Threading.Tasks;

namespace Aspect.Commands.Policy
{
    internal sealed class PolicyInitFileCommandStage : PolicyInitCommandStage
    {
        internal override Task<int?> InvokeAsync(IExecutionData data, PolicyInitCommand.Settings settings)
        {
            var resource = data.Get<string>(CommandStageConstants.ResourceName);
            var content = $@"resource ""{resource}""

validate {{
    # Enter one or more statements like the following that should be validated, eg: input.Property == ""something""
}}";
            WriteFile(settings, content);
            return Task.FromResult<int?>(0);
        }
    }
}
