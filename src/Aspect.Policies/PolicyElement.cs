using System.Collections.Generic;

namespace Aspect.Policies
{
    public sealed class PolicyElement
    {
        public string? Type { get; init; }
        public string? Name { get; init; }
        public IEnumerable<string>? Regions { get; init; }
        public IEnumerable<string>? Policies { get; init; }
    }
}
