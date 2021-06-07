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
    public class AwsEc2KeyPairResourceExplorerTests
    {
        [Fact]
        public void ShouldHaveCorrectType()
        {
            GetTarget(out _).ResourceType.Should().Be(typeof(AwsEc2KeyPair));
        }

        [Fact]
        public async Task ShouldMapAllProperties()
        {
            var account = GetAccount();
            var target = GetTarget(out var ec2Client);
            ec2Client.Setup(x => x.DescribeKeyPairsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new DescribeKeyPairsResponse
                {
                    KeyPairs = new List<KeyPairInfo>
                    {
                        new()
                        {
                            KeyFingerprint = nameof(KeyPairInfo.KeyFingerprint),
                            KeyName = "Test",
                            KeyPairId = nameof(KeyPairInfo.KeyPairId),
                            Tags = new List<Tag>
                            {
                                new("Name", "Test")
                            },
                        }
                    }
                });
            var resources = (await target.DiscoverResourcesAsync(account, "eu-west-1", default)).ToList();

            resources.Should().HaveCount(1).And.Subject.First().Should().BeOfType<AwsEc2KeyPair>();
            var sut = (AwsEc2KeyPair)resources[0];
            sut.Type.Should().Be(nameof(AwsEc2KeyPair));
            sut.Name.Should().Be("Test");
            sut.Arn.Should().Be("arn:aws:ec2:eu-west-1:000000000000:key-pair/KeyPairId");
            sut.Region.Should().Be("eu-west-1");
            sut.Fingerprint.Should().Be(nameof(KeyPairInfo.KeyFingerprint));
            sut.Id.Should().Be(nameof(KeyPairInfo.KeyPairId));
        }

        private AwsAccount GetAccount() => new AwsAccount(new AwsAccount.AwsAccountIdentifier("000000000000", "Test"));

        private IResourceExplorer<AwsAccount, AwsAccount.AwsAccountIdentifier> GetTarget(out Mock<IAmazonEC2> client)
        {
            client = new Mock<IAmazonEC2>();
            var ec2Client = client.Object;
            return new AwsEc2KeyPairResourceExplorer(_ => ec2Client);
        }
    }
}
