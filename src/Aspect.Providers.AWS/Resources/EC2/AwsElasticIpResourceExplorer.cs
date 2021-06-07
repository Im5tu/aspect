using System;
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
        private readonly Func<AmazonEC2Config, IAmazonEC2> _creator;

        public AwsElasticIpResourceExplorer(Func<AmazonEC2Config, IAmazonEC2> creator)
            : base(typeof(AwsElasticIp))
        {
            _creator = creator;
        }

        public AwsElasticIpResourceExplorer()
            : this(config => new AmazonEC2Client(config))
        {
        }

        protected override async Task<IEnumerable<IResource>> DiscoverResourcesAsync(AwsAccount account, RegionEndpoint region, CancellationToken cancellationToken)
        {
            using var ec2Client = _creator(new AmazonEC2Config { RegionEndpoint = region });
            var result = new List<IResource>();
            var response = await ec2Client.DescribeAddressesAsync(cancellationToken);

            foreach (var address in response.Addresses)
            {
                var name = address.Tags.GetName();
                var arn = GenerateArn(account, region, "ec2", $"eip/{address.AllocationId}");
                result.Add(new AwsElasticIp(account, arn, name, address.Tags.Convert(), region.SystemName)
                {
                    Id = address.AllocationId,
                    AssociationId = address.AssociationId,
                    CarrierIp = address.CarrierIp,
                    CustomerOwnedIp = address.CustomerOwnedIp,
                    CustomerOwnedIpv4Pool = address.CustomerOwnedIpv4Pool,
                    InstanceId = address.InstanceId,
                    NetworkBorderGroup = address.NetworkBorderGroup,
                    NetworkInterfaceId = address.NetworkInterfaceId,
                    NetworkInterfaceOwnerId = address.NetworkInterfaceOwnerId,
                    PrivateIpAddress = address.PrivateIpAddress,
                    PublicIp = address.PublicIp,
                    PublicIpv4Pool = address.PublicIpv4Pool,
                });
            }

            return result;
        }
    }
}
