using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aspect.Abstractions;
using Aspect.Policies;
using Aspect.Policies.Suite;
using Aspect.Services;

namespace Aspect.Commands.Run
{
    internal sealed class RunPolicyLoaderCommandStage : CommandStage<RunCommand.Settings>
    {
        private readonly IPolicyLoader _policyLoader;
        private readonly IReadOnlyDictionary<string, ICloudProvider> _cloudProviders;

        public RunPolicyLoaderCommandStage(IPolicyLoader policyLoader,
            IReadOnlyDictionary<string, ICloudProvider> cloudProviders)
        {
            _policyLoader = policyLoader;
            _cloudProviders = cloudProviders;
        }

        internal override Task<int?> InvokeAsync(IExecutionData data, RunCommand.Settings settings)
        {
            if (string.IsNullOrWhiteSpace(settings.Source))
                return Task.FromResult<int?>(1);

            if (settings.Source.EndsWith(FileExtensions.PolicySuiteExtension, StringComparison.OrdinalIgnoreCase))
            {
                var policySuite = _policyLoader.LoadPolicySuite(settings.Source);
                if (policySuite is null)
                    return Task.FromResult<int?>(1);

                data.Set(nameof(PolicySuite), policySuite);
            }
            else
            {
                var policySuite = new PolicySuite
                {
                    Name = "Policy: " + settings.Source,
                    Policies = _cloudProviders.Select(x => new PolicyElement { Type = x.Key, Name = x.Key, Regions = x.Value.GetDefaultRegions(), Policies = new [] { settings.Source }}).ToList()
                };

                data.Set(nameof(PolicySuite), policySuite);
            }

            return Task.FromResult<int?>(null);
        }
    }
}