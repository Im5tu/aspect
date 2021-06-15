using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon;
using Amazon.EC2;
using Amazon.EC2.Model;
using Aspect.Abstractions;
using Aspect.Providers.AWS.Models;
using Aspect.Providers.AWS.Models.EC2;

namespace Aspect.Providers.AWS.Resources.EC2
{
    internal sealed class AwsNetworkInterfaceResourceExplorer : AWSResourceExplorer
    {
        private readonly Func<AmazonEC2Config,IAmazonEC2> _creator;

        public AwsNetworkInterfaceResourceExplorer(Func<AmazonEC2Config, IAmazonEC2> creator)
            : base(typeof(AwsNetworkInterface))
        {
            _creator = creator;
        }

        public AwsNetworkInterfaceResourceExplorer()
            : this(config => new AmazonEC2Client(config))
        {
        }

        protected override async Task<IEnumerable<IResource>> DiscoverResourcesAsync(AwsAccount account, RegionEndpoint region, CancellationToken cancellationToken)
        {
            using var ec2Client = _creator(new AmazonEC2Config { RegionEndpoint = region });
            var result = new List<IResource>();

            string? nextToken = null;
            do
            {
                var response = await ec2Client.DescribeNetworkInterfacesAsync(new DescribeNetworkInterfacesRequest { NextToken = nextToken, Filters = new List<Filter>
                {
                    new() { Name = "owner-id", Values = new() { account.Id.Id } }
                }}, cancellationToken);
                nextToken = response.NextToken;

                foreach (var ni in response.NetworkInterfaces)
                {
                    var arn = GenerateArn(account, region, "ec2", $"network-interface/{ni.NetworkInterfaceId}");
                    result.Add(new AwsNetworkInterface(account, arn, ni.TagSet.GetName(), ni.TagSet.Convert(), region.SystemName)
                    {
                        Association = Map(ni.Association),
                        Attachment = Map(ni.Attachment),
                        AvailabilityZone = ni.AvailabilityZone.ValueOrEmpty(),
                        Description = ni.Description.ValueOrEmpty(),
                        Groups = Map(ni.Groups),
                        InterfaceType = ni.InterfaceType?.Value.ValueOrEmpty(),
                        IpAddresses = ni.PrivateIpAddresses?.Select(x => x.PrivateIpAddress).ValueOrEmpty(),
                        Ipv6Addresses = ni.Ipv6Addresses?.Select(x => x.Ipv6Address).ValueOrEmpty(),
                        MacAddress = ni.MacAddress.ValueOrEmpty(),
                        Id = ni.NetworkInterfaceId.ValueOrEmpty(),
                        OutpostArn = ni.OutpostArn.ValueOrEmpty(),
                        OwnerId = ni.OwnerId.ValueOrEmpty(),
                        PrivateDnsName = ni.PrivateDnsName.ValueOrEmpty(),
                        RequesterId = ni.RequesterId.ValueOrEmpty(),
                        RequesterManaged = ni.RequesterManaged,
                        SourceDestCheck = ni.SourceDestCheck,
                        Status = ni.Status?.Value.ValueOrEmpty(),
                        SubnetId = ni.SubnetId.ValueOrEmpty(),
                        VpcId = ni.VpcId.ValueOrEmpty(),
                    });
                }
            } while (!string.IsNullOrWhiteSpace(nextToken));

            return result;
        }

        private AwsNetworkInterface.NetworkAttachment Map(NetworkInterfaceAttachment attach)
        {
            return new()
            {
                DeleteOnTermination = attach?.DeleteOnTermination ?? false,
                DeviceIndex = attach?.DeviceIndex ?? -1,
                Id = attach?.AttachmentId.ValueOrEmpty(),
                InstanceId = attach?.InstanceId.ValueOrEmpty(),
                InstanceOwnerId = attach?.InstanceOwnerId.ValueOrEmpty(),
                NetworkCardIndex = attach?.NetworkCardIndex ?? -1,
                Status = attach?.Status?.Value.ValueOrEmpty(),
            };
        }

        private IEnumerable<AwsNetworkInterface.Group> Map(List<GroupIdentifier> groups)
        {
            var result = new List<AwsNetworkInterface.Group>();

            foreach (var group in groups)
                result.Add(new AwsNetworkInterface.Group
                {
                    Id = group.GroupId.ValueOrEmpty(),
                    Name = group.GroupName.ValueOrEmpty()
                });

            return result;
        }

        private AwsNetworkInterface.NetworkAssociation Map(NetworkInterfaceAssociation assoc)
        {
            return new()
            {
                AllocationId = assoc?.AllocationId.ValueOrEmpty(),
                AssociationId = assoc?.AssociationId.ValueOrEmpty(),
                CarrierIp = assoc?.CarrierIp.ValueOrEmpty(),
                PublicIp = assoc?.PublicIp.ValueOrEmpty(),
                CustomerOwnedIp = assoc?.CustomerOwnedIp.ValueOrEmpty(),
                IpOwnerId = assoc?.IpOwnerId.ValueOrEmpty(),
                PublicDnsName = assoc?.PublicDnsName.ValueOrEmpty(),
            };
        }
    }
}
