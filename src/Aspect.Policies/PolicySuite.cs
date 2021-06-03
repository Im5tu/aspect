using System.Collections.Generic;

namespace Aspect.Policies
{
    internal sealed class PolicySuite
    {
        public string? Name { get; init; }
        public string? Description { get; init; }
        public IEnumerable<PolicySuiteScope>? Scopes { get; init; }
    }
}
