using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Aspect.Abstractions
{
    /// <summary>
    ///     Loads resources from a specified AWS account
    /// </summary>
    public interface IResourceExplorer
    {
        /// <summary>
        /// </summary>
        Task<IEnumerable<IResource>> DiscoverResourcesAsync(CancellationToken cancellationToken);
    }
}