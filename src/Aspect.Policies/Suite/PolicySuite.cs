using System.Collections.Generic;

namespace Aspect.Policies.Suite
{
    public sealed class PolicySuite
    {
        public string? Name { get; init; }
        public string? Description { get; init; }
        public IEnumerable<PolicyElement>? Policies { get; init; }
    }
}
