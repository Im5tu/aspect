using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amazon;
using Aspect.Abstractions;
using Aspect.Providers.AWS.Models;

namespace Aspect.Providers.AWS
{
    internal abstract class AWSResourceExplorer : IResourceExplorer<AwsAccount, AwsAccount.AwsAccountIdentifier>
    {
        protected AWSResourceExplorer(Type resourceType)
        {
            ResourceType = resourceType;
        }

        /// <inheritDoc />
        public Type ResourceType { get; }

        /// <inheritDoc />
        public async Task<IEnumerable<IResource>> DiscoverResourcesAsync(AwsAccount account, string region, CancellationToken cancellationToken)
        {
            try
            {
                var endpoint = RegionEndpoint.GetBySystemName(region);
                if (endpoint is null)
                    throw new Exception($"Region '{region}' is invalid");

                return await DiscoverResourcesAsync(account, endpoint, cancellationToken);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        protected abstract Task<IEnumerable<IResource>> DiscoverResourcesAsync(AwsAccount account, RegionEndpoint region, CancellationToken cancellationToken);

        protected string GenerateArn(AwsAccount account, RegionEndpoint region, string service, string suffix, string awsType = "aws")
            => $"arn:{awsType}:{service}:{region.SystemName}:{account.Id.Id}:{suffix}";
    }
}
