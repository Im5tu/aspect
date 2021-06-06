using System.Collections.Generic;

namespace Aspect.Policies.Suite
{
    public sealed class PolicyElement
    {
        public string? Type { get; set; }
        public string? Name { get; set; }
        public IEnumerable<string>? Regions { get; set; }
        public IEnumerable<string>? Policies { get; set; }
        public string? Description { get; set; }
    }
}
