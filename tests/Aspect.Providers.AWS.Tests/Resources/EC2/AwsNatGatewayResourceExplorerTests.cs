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
    public class AwsNatGatewayResourceExplorerTests
    {
        [Fact]
        public void ShouldHaveCorrectType()
        {
            GetTarget(out _).ResourceType.Should().Be(typeof(AwsNatGateway));
        }

        [Fact]
        public async Task ShouldGetAllValuesUsingNextTokens()
        {
            var account = GetAccount();
            var target = GetTarget(out var ec2Client);

            ec2Client.Setup(x => x.DescribeNatGatewaysAsync(It.Is<DescribeNatGatewaysRequest>(x => x.NextToken == null), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new DescribeNatGatewaysResponse
                {
                    NextToken = nameof(DescribeInstancesResponse.NextToken),
                    NatGateways = new List<NatGateway>
                    {
                        new()
                    }
                });
            ec2Client.Setup(x => x.DescribeNatGatewaysAsync(It.Is<DescribeNatGatewaysRequest>(x => x.NextToken == nameof(DescribeNatGatewaysRequest.NextToken)), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new DescribeNatGatewaysResponse
                {
                    NatGateways = new List<NatGateway>
                    {
                        new()
                    }
                });

            var resources = (await target.DiscoverResourcesAsync(account, "eu-west-1", default)).ToList();
            resources.Should().HaveCount(2);
        }

        [Fact]
        public async Task ShouldMapAllPropertiesWhenNotNull()
        {
            var account = GetAccount();
            var target = GetTarget(out var ec2Client);
            ec2Client.Setup(x => x.DescribeNatGatewaysAsync(It.Is<DescribeNatGatewaysRequest>(y => y.NextToken == null), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new DescribeNatGatewaysResponse
                {
                    NatGateways = new List<NatGateway>
                    {
                        new()
                        {
                            NatGatewayId = nameof(NatGateway.NatGatewayId),
                            State = NatGatewayState.Available,
                            FailureCode = nameof(NatGateway.FailureCode),
                            ProvisionedBandwidth = new ProvisionedBandwidth { Provisioned = nameof(ProvisionedBandwidth.Provisioned) },
                            SubnetId = nameof(NatGateway.SubnetId),
                            VpcId = nameof(NatGateway.VpcId),
                            NatGatewayAddresses = new List<NatGatewayAddress>
                            {
                                new()
                            },
                            Tags = new List<Tag>
                            {
                                new() { Key = "Name", Value = "Test" }
                            }
                        }
                    }
                });
            var resources = (await target.DiscoverResourcesAsync(account, "eu-west-1", default)).ToList();

            resources.Should().HaveCount(1).And.Subject.First().Should().BeOfType<AwsNatGateway>();
            var sut = (AwsNatGateway)resources[0];
            sut.Type.Should().Be(nameof(AwsNatGateway));
            sut.Name.Should().Be("Test");
            sut.Arn.Should().Be("arn:aws:ec2:eu-west-1:000000000000:nat-gateway/NatGatewayId");
            sut.Region.Should().Be("eu-west-1");
            sut.Id.Should().Be(nameof(NatGateway.NatGatewayId));
            sut.State.Should().Be("available");
            sut.FailureCode.Should().Be(nameof(NatGateway.FailureCode));
            sut.ProvisionedBandwidth.Should().Be(nameof(ProvisionedBandwidth.Provisioned));
            sut.SubnetId.Should().Be(nameof(NatGateway.SubnetId));
            sut.VpcId.Should().Be(nameof(NatGateway.VpcId));
            sut.Addresses.Should().HaveCount(1);
        }

        private AwsAccount GetAccount() => new AwsAccount(new AwsAccount.AwsAccountIdentifier("000000000000", "Test"));

        private IResourceExplorer<AwsAccount, AwsAccount.AwsAccountIdentifier> GetTarget(out Mock<IAmazonEC2> client)
        {
            client = new Mock<IAmazonEC2>();
            var ec2Client = client.Object;
            return new AwsNatGatewayResourceExplorer(_ => ec2Client);
        }
    }
}
