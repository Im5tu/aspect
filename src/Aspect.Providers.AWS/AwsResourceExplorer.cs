using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amazon;
using Aspect.Abstractions;
using Aspect.Providers.AWS.Models;

namespace Aspect.Providers.AWS
{
    internal abstract class AwsResourceExplorer : IResourceExplorer<AwsAccount, AwsAccountIdentifier>
    {
        protected AwsResourceExplorer(Type resourceType)
        {
            ResourceType = resourceType;
        }

        /// <inheritDoc />
        public Type ResourceType { get; }

        /// <inheritDoc />
        public async Task<IEnumerable<IResource>> DiscoverResourcesAsync(AwsAccount account, string region, CancellationToken cancellationToken)
        {
            var endpoint = RegionEndpoint.GetBySystemName(region);
            if (endpoint is null)
                throw new Exception($"Region '{region}' is invalid");

            return await DiscoverResourcesAsync(account, endpoint, cancellationToken);
        }

        protected abstract Task<IEnumerable<IResource>> DiscoverResourcesAsync(AwsAccount account, RegionEndpoint region, CancellationToken cancellationToken);
    }
}
