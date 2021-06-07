using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amazon;
using Amazon.EC2;
using Aspect.Abstractions;
using Aspect.Providers.AWS.Models;
using Aspect.Providers.AWS.Models.EC2;

namespace Aspect.Providers.AWS.Resources.EC2
{
    internal sealed class AwsNetworkInterfaceResourceExplorer : AWSResourceExplorer
    {
        public AwsNetworkInterfaceResourceExplorer()
            : base(typeof(AwsNetworkInterface))
        {
        }

        protected override async Task<IEnumerable<IResource>> DiscoverResourcesAsync(AwsAccount account, RegionEndpoint region, CancellationToken cancellationToken)
        {
            using var ec2Client = new AmazonEC2Client(new AmazonEC2Config { RegionEndpoint = region });
            var result = new List<IResource>();

            await Task.Yield();

            return result;
        }
    }
}
