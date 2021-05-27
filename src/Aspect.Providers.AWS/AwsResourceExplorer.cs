using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Aspect.Abstractions;
using Aspect.Providers.AWS.Models;

namespace Aspect.Providers.AWS
{
    /// <summary>
    ///
    /// </summary>
    public abstract class AwsResourceExplorer : IResourceExplorer
    {
        /// <summary>
        ///     The AWS account associated with this resource explorer instance
        /// </summary>
        protected AwsAccount Account { get; }

        /// <param name="account">The AWS account associated with this resource explorer instance</param>
        protected AwsResourceExplorer(AwsAccount account)
        {
            Account = account;
        }

        /// <inheritDoc />
        public abstract Task<IEnumerable<IResource>> DiscoverResourcesAsync(CancellationToken cancellationToken);
    }
}