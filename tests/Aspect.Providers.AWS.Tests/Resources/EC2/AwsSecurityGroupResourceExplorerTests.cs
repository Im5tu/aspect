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
    public class AwsSecurityGroupResourceExplorerTests
    {
        [Fact]
        public void ShouldHaveCorrectType()
        {
            GetTarget(out _).ResourceType.Should().Be(typeof(AwsSecurityGroup));
        }

        [Fact]
        public async Task ShouldMapAllPropertiesWhenNotNull()
        {
            var account = GetAccount();
            var target = GetTarget(out var ec2Client);
            ec2Client.Setup(x => x.DescribeSecurityGroupsAsync(It.IsAny<DescribeSecurityGroupsRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new DescribeSecurityGroupsResponse
                {
                    SecurityGroups = new List<SecurityGroup>
                    {
                        new()
                        {
                            Description = nameof(SecurityGroup.Description),
                            GroupId = nameof(SecurityGroup.GroupId),
                            GroupName = "Test",
                            OwnerId = nameof(SecurityGroup.OwnerId),
                            VpcId = nameof(SecurityGroup.VpcId),
                            IpPermissions = new List<IpPermission>
                            {
                                new ()
                                {
                                    FromPort = 1,
                                    ToPort = 1,
                                    IpProtocol = "TCP"
                                }
                            },
                            IpPermissionsEgress = new List<IpPermission>
                            {
                                new ()
                                {
                                    FromPort = 1,
                                    ToPort = 1,
                                    IpProtocol = "TCP"
                                }
                            }
                        }
                    }
                });
            var resources = (await target.DiscoverResourcesAsync(account, "eu-west-1", default)).ToList();

            resources.Should().HaveCount(1).And.Subject.First().Should().BeOfType<AwsSecurityGroup>();
            var sut = (AwsSecurityGroup)resources[0];
            sut.Type.Should().Be(nameof(AwsSecurityGroup));
            sut.Name.Should().Be("Test");
            sut.Arn.Should().Be($"arn:aws:ec2:eu-west-1:000000000000:security-group/{nameof(SecurityGroup.GroupId)}");
            sut.Region.Should().Be("eu-west-1");

            sut.Description.Should().Be(nameof(SecurityGroup.Description));
            sut.Id.Should().Be(nameof(SecurityGroup.GroupId));
            sut.OwnerId.Should().Be(nameof(SecurityGroup.OwnerId));
            sut.VpcId.Should().Be(nameof(SecurityGroup.VpcId));
            sut.Description.Should().Be(nameof(SecurityGroup.Description));
            sut.IngressRules.Should().HaveCount(1);
            sut.EgressRules.Should().HaveCount(1);
        }

        private AwsAccount GetAccount() => new AwsAccount(new AwsAccount.AwsAccountIdentifier("000000000000", "Test"));

        private IResourceExplorer<AwsAccount, AwsAccount.AwsAccountIdentifier> GetTarget(out Mock<IAmazonEC2> client)
        {
            client = new Mock<IAmazonEC2>();
            var ec2Client = client.Object;
            return new AwsSecurityGroupResourceExplorer(_ => ec2Client);
        }
    }
}
