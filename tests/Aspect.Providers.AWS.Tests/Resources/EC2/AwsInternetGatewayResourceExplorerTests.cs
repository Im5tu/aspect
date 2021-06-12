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
    public class AwsInternetGatewayResourceExplorerTests
    {
        [Fact]
        public void ShouldHaveCorrectType()
        {
            GetTarget(out _).ResourceType.Should().Be(typeof(AwsInternetGateway));
        }

        [Fact]
        public async Task ShouldGetAllValuesUsingNextTokens()
        {
            var account = GetAccount();
            var target = GetTarget(out var ec2Client);

            ec2Client.Setup(x => x.DescribeInternetGatewaysAsync(It.Is<DescribeInternetGatewaysRequest>(x => x.NextToken == null), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new DescribeInternetGatewaysResponse
                {
                    NextToken = nameof(DescribeInstancesResponse.NextToken),
                    InternetGateways = new List<InternetGateway>
                    {
                        new()
                    }
                });
            ec2Client.Setup(x => x.DescribeInternetGatewaysAsync(It.Is<DescribeInternetGatewaysRequest>(x => x.NextToken == nameof(DescribeInternetGatewaysRequest.NextToken)), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new DescribeInternetGatewaysResponse
                {
                    InternetGateways = new List<InternetGateway>
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
            ec2Client.Setup(x => x.DescribeInternetGatewaysAsync(It.Is<DescribeInternetGatewaysRequest>(y => y.NextToken == null), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new DescribeInternetGatewaysResponse
                {
                    InternetGateways = new List<InternetGateway>
                    {
                        new()
                        {
                            OwnerId = nameof(InternetGateway.OwnerId),
                            InternetGatewayId = nameof(InternetGateway.InternetGatewayId),
                            Attachments = new List<InternetGatewayAttachment>
                            {
                                new()
                                {
                                    State = AttachmentStatus.Attached,
                                    VpcId = nameof(InternetGatewayAttachment.VpcId)
                                }
                            },
                            Tags = new List<Tag>
                            {
                                new() { Key = "Name", Value = "Test" }
                            }
                        }
                    }
                });
            var resources = (await target.DiscoverResourcesAsync(account, "eu-west-1", default)).ToList();

            resources.Should().HaveCount(1).And.Subject.First().Should().BeOfType<AwsInternetGateway>();
            var sut = (AwsInternetGateway)resources[0];
            sut.Type.Should().Be(nameof(AwsInternetGateway));
            sut.Name.Should().Be("Test");
            sut.Arn.Should().Be("arn:aws:ec2:eu-west-1:000000000000:internet-gateway/InternetGatewayId");
            sut.Region.Should().Be("eu-west-1");
            sut.OwnerId.Should().Be(nameof(InternetGateway.OwnerId));
            sut.Id.Should().Be(nameof(InternetGateway.InternetGatewayId));
            sut.Attachments.Should().HaveCount(1);
        }

        private AwsAccount GetAccount() => new(new AwsAccount.AwsAccountIdentifier("000000000000", "Test"));

        private IResourceExplorer<AwsAccount, AwsAccount.AwsAccountIdentifier> GetTarget(out Mock<IAmazonEC2> client)
        {
            client = new Mock<IAmazonEC2>();
            var ec2Client = client.Object;
            return new AwsInternetGatewayResourceExplorer(_ => ec2Client);
        }
    }
}
