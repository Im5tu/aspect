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
    public class AwsElasticIpResourceExplorerTests
    {
        [Fact]
        public void ShouldHaveCorrectType()
        {
            GetTarget(out _).ResourceType.Should().Be(typeof(AwsElasticIp));
        }

        [Fact]
        public async Task ShouldMapAllPropertiesWhenNotNull()
        {
            var account = GetAccount();
            var target = GetTarget(out var ec2Client);
            ec2Client.Setup(x => x.DescribeAddressesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new DescribeAddressesResponse
                {
                    Addresses = new List<Address>
                    {
                        new()
                        {
                            AllocationId = nameof(Address.AllocationId),
                            AssociationId = nameof(Address.AssociationId),
                            CarrierIp = nameof(Address.CarrierIp),
                            CustomerOwnedIp = nameof(Address.CustomerOwnedIp),
                            CustomerOwnedIpv4Pool = nameof(Address.CustomerOwnedIpv4Pool),
                            InstanceId = nameof(Address.InstanceId),
                            NetworkBorderGroup = nameof(Address.NetworkBorderGroup),
                            NetworkInterfaceId = nameof(Address.NetworkInterfaceId),
                            NetworkInterfaceOwnerId = nameof(Address.NetworkInterfaceOwnerId),
                            PrivateIpAddress = nameof(Address.PrivateIpAddress),
                            PublicIp = nameof(Address.PublicIp),
                            PublicIpv4Pool = nameof(Address.PublicIpv4Pool),
                            Tags = new List<Tag>
                            {
                                new() { Key = "Name", Value = "Test" }
                            }
                        }
                    }
                });
            var resources = (await target.DiscoverResourcesAsync(account, "eu-west-1", default)).ToList();

            resources.Should().HaveCount(1).And.Subject.First().Should().BeOfType<AwsElasticIp>();
            var sut = (AwsElasticIp)resources[0];
            sut.Type.Should().Be(nameof(AwsElasticIp));
            sut.Name.Should().Be("Test");
            sut.Arn.Should().Be("arn:aws:ec2:eu-west-1:000000000000:eip/AllocationId");
            sut.Region.Should().Be("eu-west-1");
            sut.Id.Should().Be(nameof(Address.AllocationId));
            sut.AssociationId.Should().Be(nameof(Address.AssociationId));
            sut.CarrierIp.Should().Be(nameof(Address.CarrierIp));
            sut.CustomerOwnedIp.Should().Be(nameof(Address.CustomerOwnedIp));
            sut.CustomerOwnedIpv4Pool.Should().Be(nameof(Address.CustomerOwnedIpv4Pool));
            sut.InstanceId.Should().Be(nameof(Address.InstanceId));
            sut.NetworkBorderGroup.Should().Be(nameof(Address.NetworkBorderGroup));
            sut.NetworkInterfaceId.Should().Be(nameof(Address.NetworkInterfaceId));
            sut.NetworkInterfaceOwnerId.Should().Be(nameof(Address.NetworkInterfaceOwnerId));
            sut.PrivateIpAddress.Should().Be(nameof(Address.PrivateIpAddress));
            sut.PublicIp.Should().Be(nameof(Address.PublicIp));
            sut.PublicIpv4Pool.Should().Be(nameof(Address.PublicIpv4Pool));
        }

        private AwsAccount GetAccount() => new(new AwsAccount.AwsAccountIdentifier("000000000000", "Test"));

        private IResourceExplorer<AwsAccount, AwsAccount.AwsAccountIdentifier> GetTarget(out Mock<IAmazonEC2> client)
        {
            client = new Mock<IAmazonEC2>();
            var ec2Client = client.Object;
            return new AwsElasticIpResourceExplorer(_ => ec2Client);
        }
    }
}
