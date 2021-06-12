using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.EC2;
using Amazon.EC2.Model;
using Aspect.Abstractions;
using Aspect.Providers.AWS.Models;
using Aspect.Providers.AWS.Models.EC2;
using Aspect.Providers.AWS.Resources.EC2;
using FluentAssertions;
using Moq;
using Xunit;

namespace Aspect.Providers.AWS.Tests.Resources.EC2
{
    public class AwsNetworkInterfaceResourceExplorerTests
    {
        [Fact]
        public void ShouldHaveCorrectType()
        {
            GetTarget(out _).ResourceType.Should().Be(typeof(AwsNetworkInterface));
        }

        [Fact]
        public async Task ShouldMapAllPropertiesWhenNotNull()
        {
            var account = GetAccount();
            var target = GetTarget(out var ec2Client);
            ec2Client.Setup(x => x.DescribeNetworkInterfacesAsync(It.IsAny<DescribeNetworkInterfacesRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(new DescribeNetworkInterfacesResponse
                {
                    NetworkInterfaces = new List<NetworkInterface>
                    {
                        new()
                        {
                            Association = new NetworkInterfaceAssociation
                            {
                                AllocationId = nameof(NetworkInterfaceAssociation.AllocationId),
                                AssociationId = nameof(NetworkInterfaceAssociation.AssociationId),
                                CarrierIp = nameof(NetworkInterfaceAssociation.CarrierIp),
                                PublicIp = nameof(NetworkInterfaceAssociation.PublicIp),
                                CustomerOwnedIp = nameof(NetworkInterfaceAssociation.CustomerOwnedIp),
                                IpOwnerId = nameof(NetworkInterfaceAssociation.IpOwnerId),
                                PublicDnsName = nameof(NetworkInterfaceAssociation.PublicDnsName),
                            },
                            Attachment = new NetworkInterfaceAttachment
                            {
                                Status = AttachmentStatus.Attached,
                                AttachmentId = nameof(NetworkInterfaceAttachment.AttachmentId),
                                AttachTime = DateTime.UtcNow,
                                DeviceIndex = 1,
                                InstanceId = nameof(NetworkInterfaceAttachment.InstanceId),
                                DeleteOnTermination = true,
                                InstanceOwnerId = nameof(NetworkInterfaceAttachment.InstanceOwnerId),
                                NetworkCardIndex = 1,
                            },
                            AvailabilityZone = nameof(NetworkInterface.AvailabilityZone),
                            Description = nameof(NetworkInterface.Description),
                            Groups = new List<GroupIdentifier>
                            {
                                new() { GroupId = "Test", GroupName = "Test"}
                            },
                            InterfaceType = NetworkInterfaceType.Interface,
                            PrivateIpAddresses = new List<NetworkInterfacePrivateIpAddress>
                            {
                                new() { PrivateIpAddress = "10.0.0.0/8 "}
                            },
                            Ipv6Addresses = new List<NetworkInterfaceIpv6Address>
                            {
                                new() { Ipv6Address = "::1" }
                            },
                            MacAddress = nameof(NetworkInterface.MacAddress),
                            NetworkInterfaceId = nameof(NetworkInterface.NetworkInterfaceId),
                            OutpostArn = nameof(NetworkInterface.OutpostArn),
                            OwnerId = nameof(NetworkInterface.OwnerId),
                            PrivateDnsName = nameof(NetworkInterface.PrivateDnsName),
                            RequesterId = nameof(NetworkInterface.RequesterId),
                            RequesterManaged = true,
                            Status = NetworkInterfaceStatus.Available,
                            SourceDestCheck = true,
                            SubnetId = nameof(NetworkInterface.SubnetId),
                            VpcId = nameof(NetworkInterface.VpcId),
                            TagSet = new List<Tag>
                            {
                                new () { Key = "Name", Value = "Test" }
                            }
                        }
                    }
                });
            var resources = (await target.DiscoverResourcesAsync(account, "eu-west-1", default)).ToList();

            resources.Should().HaveCount(1).And.Subject.First().Should().BeOfType<AwsNetworkInterface>();
            var sut = (AwsNetworkInterface)resources[0];
            sut.Type.Should().Be(nameof(AwsNetworkInterface));
            sut.Name.Should().Be("Test");
            sut.Arn.Should().Be("arn:aws:ec2:eu-west-1:000000000000:network-interface/NetworkInterfaceId");
            sut.Region.Should().Be("eu-west-1");

            sut.Association.Should().NotBeNull();
            sut.Attachment.Should().NotBeNull();
            sut.AvailabilityZone.Should().Be(nameof(NetworkInterface.AvailabilityZone));
            sut.Description.Should().Be(nameof(NetworkInterface.Description));
            sut.Groups.Should().HaveCount(1);
            sut.InterfaceType.Should().Be(NetworkInterfaceType.Interface.Value);
            sut.IpAddresses.Should().HaveCount(1);
            sut.Ipv6Addresses.Should().HaveCount(1);
            sut.MacAddress.Should().Be(nameof(NetworkInterface.MacAddress));
            sut.Id.Should().Be(nameof(NetworkInterface.NetworkInterfaceId));
            sut.OutpostArn.Should().Be(nameof(NetworkInterface.OutpostArn));
            sut.OwnerId.Should().Be(nameof(NetworkInterface.OwnerId));
            sut.PrivateDnsName.Should().Be(nameof(NetworkInterface.PrivateDnsName));
            sut.RequesterId.Should().Be(nameof(NetworkInterface.RequesterId));
            sut.RequesterManaged.Should().Be(true);
            sut.SourceDestCheck.Should().Be(true);
            sut.Status.Should().Be("available");
            sut.SubnetId.Should().Be(nameof(NetworkInterface.SubnetId));
            sut.VpcId.Should().Be(nameof(NetworkInterface.VpcId));
        }

        private AwsAccount GetAccount() => new(new AwsAccount.AwsAccountIdentifier("000000000000", "Test"));

        private IResourceExplorer<AwsAccount, AwsAccount.AwsAccountIdentifier> GetTarget(out Mock<IAmazonEC2> client)
        {
            client = new Mock<IAmazonEC2>();
            var ec2Client = client.Object;
            return new AwsNetworkInterfaceResourceExplorer(_ => ec2Client);
        }
    }
}
