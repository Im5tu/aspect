using System.Collections.Generic;

namespace Aspect.Policies.Suite
{
    public sealed class PolicySuite
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public IEnumerable<PolicyElement>? Policies { get; set; }
    }
}
