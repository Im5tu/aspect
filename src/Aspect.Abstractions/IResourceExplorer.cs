using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Aspect.Abstractions
{
    /// <summary>
    ///     Loads resources from a cloud provider
    /// </summary>
    public interface IResourceExplorer
    {
        Type ResourceType { get; }
    }

    /// <summary>
    ///     Loads resources from a cloud provider
    /// </summary>
    public interface IResourceExplorer<in TAccount, TAccountIdentifier> : IResourceExplorer
        where TAccount : Account<TAccountIdentifier>
        where TAccountIdentifier : AccountIdentifier
    {
        /// <summary>
        /// </summary>
        Task<IEnumerable<IResource>> DiscoverResourcesAsync(TAccount account, string region, CancellationToken cancellationToken);
    }
}
