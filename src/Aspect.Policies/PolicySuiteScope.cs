using System.Collections.Generic;

namespace Aspect.Policies
{
    internal sealed class PolicySuiteScope
    {
        public string? Type { get; init; }
        public string? Name { get; init; }
        public IEnumerable<string>? Regions { get; init; }
        public IEnumerable<string>? Policies { get; init; }
    }
}
