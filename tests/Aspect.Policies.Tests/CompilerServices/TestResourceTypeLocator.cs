using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Aspect.Abstractions;

namespace Aspect.Policies.Tests.CompilerServices
{
    internal class TestResourceTypeLocator : IResourceTypeLocator
    {
        private readonly Dictionary<string,Type> _resources;

        public TestResourceTypeLocator()
        {
            _resources = typeof(TestResource).Assembly.GetResourceTypes().ToDictionary(x => x.Name, x => x);
        }

        public bool TryLocateType(string resourceType, [NotNullWhen(true)] out Type? type) => _resources.TryGetValue(resourceType, out type);
    }
}
