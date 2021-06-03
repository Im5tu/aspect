using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Aspect.Abstractions
{
    internal sealed class ResourceTypeLocator : IResourceTypeLocator
    {
        private readonly Dictionary<string, Type> _resources = new(StringComparer.OrdinalIgnoreCase);

        public ResourceTypeLocator(IEnumerable<ICloudProvider> cloudProviders)
        {
            foreach (var provider in cloudProviders)
            {
                foreach (var (key, value) in provider.GetResources())
                    _resources[key] = value;
            }
        }

        public bool TryLocateType(string resourceType, [NotNullWhen(true)] out Type? type)
            => _resources.TryGetValue(resourceType, out type);
    }
}