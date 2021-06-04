using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Aspect.Abstractions;
using Aspect.Providers.Azure.Models;

namespace Aspect.Providers.Azure
{
    internal sealed class AzureCloudProvider : ICloudProvider
    {
        private readonly Dictionary<string,Type> _resources = new();

        public Type AccountType { get; } = typeof(AzureAccount);
        public string Name { get; } = "Azure";

        public IReadOnlyDictionary<string, Type> GetResources() => _resources;

        public IEnumerable<string> GetAllRegions() => Enumerable.Empty<string>();
        public Task<IEnumerable<IResource>> DiscoverResourcesAsync(string region, Type resourceType, Action<string> updater, CancellationToken cancellationToken)
        {
            return Task.FromResult(Enumerable.Empty<IResource>());
        }
    }
}
