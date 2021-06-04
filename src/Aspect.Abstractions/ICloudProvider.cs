using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Aspect.Abstractions
{
    public interface ICloudProvider
    {
        Type AccountType { get; }
        string Name { get; }

        IReadOnlyDictionary<string, Type> GetResources();
        IEnumerable<string> GetAllRegions();
        Task<IEnumerable<IResource>> DiscoverResourcesAsync(string region, Type resourceType, Action<string> updater, CancellationToken cancellationToken);
    }
}
