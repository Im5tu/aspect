using System;
using System.Collections.Generic;
using System.Linq;
using Aspect.Abstractions;

namespace Aspect.Policies.Suite
{
    internal sealed class PolicySuiteValidator : IPolicySuiteValidator
    {
        private readonly IReadOnlyDictionary<string, ICloudProvider> _cloudProviders;

        public PolicySuiteValidator(IReadOnlyDictionary<string, ICloudProvider> cloudProviders)
        {
            _cloudProviders = cloudProviders;
        }

        public PolicySuiteValidationResult Validate(PolicySuite policySuite)
        {
            if (policySuite is null)
                return PolicySuiteValidationResult.Error("PolicySuite is null.");

            var result = new PolicySuiteValidationResult();

            if (string.IsNullOrWhiteSpace(policySuite.Name))
                result.AddError("A policy suite must have a top level name set.");

            if (policySuite.Policies is null)
                result.AddError("A policy suite must define a policies section.");
            else
            {
                var index = 0;
                foreach (var policy in policySuite.Policies)
                {
                    if (_cloudProviders.TryGetValue(policy.Type ?? string.Empty, out var provider))
                    {
                        var regionIndex = 0;
                        var regions = provider.GetAllRegions().ToList();
                        foreach (var region in policy.Regions ?? Enumerable.Empty<string>())
                        {
                            if (!regions.Contains(region, StringComparer.OrdinalIgnoreCase))
                            {
                                result.AddError($"Policy {index} contains an invalid region for {provider.Name}. Region: {region} Index: {regionIndex}");
                            }
                            regionIndex++;
                        }

                        if (!(policy.Policies ?? Enumerable.Empty<string>()).Any(x => !string.IsNullOrWhiteSpace(x)))
                        {
                            result.AddError($"Policy {index} does not define a policy to validate");
                        }

                    }
                    else
                    {
                        result.AddError($"Policy {index} does not define a type.");
                    }

                    index++;
                }
            }

            return result;
        }
    }
}
