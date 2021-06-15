using System.Collections.Generic;
using Aspect.Abstractions;

namespace Aspect.Services
{
    internal class PolicySuiteRunResult
    {
        public IEnumerable<string>? Errors { get; init; }
        public IEnumerable<FailedResource>? FailedResources { get; set; }

        internal class FailedResource
        {
            public IResource? Resource { get; init; }
            public IEnumerable<string>? FailedPolicies { get; init; }
        }
    }
}
