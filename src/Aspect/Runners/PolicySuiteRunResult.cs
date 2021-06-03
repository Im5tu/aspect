using System.Collections.Generic;
using Aspect.Abstractions;

namespace Aspect.Runners
{
    internal class PolicySuiteRunResult
    {
        public string? Error { get; set; }
        public List<FailedResource>? FailedResources { get; set; }

        public class FailedResource
        {
            public IResource? Resource { get; init; }
            public string? Source { get; init; }
        }
    }
}
