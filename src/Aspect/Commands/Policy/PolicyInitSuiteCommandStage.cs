using System.Threading.Tasks;
using Aspect.Policies.Suite;

namespace Aspect.Commands.Policy
{
    internal sealed class PolicySuiteInitCommandStage : PolicyInitCommandStage
    {
        private readonly IPolicySuiteSerializer _policySuiteSerializer;

        public PolicySuiteInitCommandStage(IPolicySuiteSerializer policySuiteSerializer)
        {
            _policySuiteSerializer = policySuiteSerializer;
        }

        internal override Task<int?> InvokeAsync(IExecutionData data, PolicyInitCommand.Settings settings)
        {
            var content = GetPolicySuiteContent();
            WriteFile(settings, content);
            return Task.FromResult<int?>(0);
        }

        private string GetPolicySuiteContent()
        {
            return _policySuiteSerializer.Serialize(new PolicySuite
            {
                Name = "My Best Practises",
                Description = "Describe what the policy suite does",
                Policies = new[]
                {
                    new PolicyElement
                    {
                        Name = "AWS Best Practises",
                        Description = "Describing this section",
                        Type = "AWS",
                        Regions = new[] {"eu-west-1"},
                        Policies = new[] {"D:\\Policies\\MyPolicy.policy"}
                    },
                    new PolicyElement
                    {
                        Name = "Azure Best Practises",
                        Description = "Describing this section",
                        Type = "Azure",
                        Regions = new[] {"uk-south"},
                        Policies = new[] {"D:\\Policies\\MyPolicy.policy"}
                    }
                }
            });
        }
    }
}