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

    internal sealed class AwsElasticIpResourceExplorer : AWSResourceExplorer
    {
        public AwsElasticIpResourceExplorer()
            : base(typeof(AwsElasticIp))
        {
        }

        protected override async Task<IEnumerable<IResource>> DiscoverResourcesAsync(AwsAccount account, RegionEndpoint region, CancellationToken cancellationToken)
        {
            using var ec2Client = new AmazonEC2Client(new AmazonEC2Config { RegionEndpoint = region });
            var result = new List<IResource>();
            var response = await ec2Client.DescribeAddressesAsync(cancellationToken);

            foreach (var address in response.Addresses)
            {
                var arn = $"arn:aws:ec2:{region}:{account.Id.Id}:eip/{address.AllocationId}";
                var name = address.Tags.GetName();

                result.Add(new AwsElasticIp(account, arn, name, address.Tags.ConvertTags(), region.SystemName)
                {
                    AllocationId = address.AllocationId,
                    AssociationId = address.AssociationId,
                    CarrierIp = address.CarrierIp,
                    InstanceId = address.InstanceId,
                    PublicIp = address.PublicIp,
                    CustomerOwnedIp = address.CustomerOwnedIp,
                    NetworkBorderGroup = address.NetworkBorderGroup,
                    NetworkInterfaceId = address.NetworkInterfaceId,
                    PrivateIpAddress = address.PrivateIpAddress,
                    CustomerOwnedIpv4Pool = address.CustomerOwnedIpv4Pool,
                    NetworkInterfaceOwnerId = address.NetworkInterfaceOwnerId,
                    PublicIpv4Pool = address.PublicIpv4Pool,
                });
            }

            return result;
        }
    }
}
