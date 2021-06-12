using System.Linq;
using System.Threading.Tasks;
using Amazon.EC2;
using Aspect.Abstractions;
using Aspect.Providers.AWS.Models;
using Aspect.Providers.AWS.Models.EC2;
using Aspect.Providers.AWS.Resources.EC2;
using FluentAssertions;
using Moq;
using Xunit;

namespace Aspect.Providers.AWS.Tests.Resources.EC2
{
    public class AwsTransitGatewayResourceExplorerTests
    {
        [Fact]
        public void ShouldHaveCorrectType()
        {
            GetTarget(out _).ResourceType.Should().Be(typeof(AwsTransitGateway));
        }

        [Fact]
        public async Task ShouldMapAllPropertiesWhenNotNull()
        {
            var account = GetAccount();
            var target = GetTarget(out var ec2Client);

            var resources = (await target.DiscoverResourcesAsync(account, "eu-west-1", default)).ToList();

            resources.Should().HaveCount(1).And.Subject.First().Should().BeOfType<AwsTransitGateway>();
            var sut = (AwsTransitGateway)resources[0];
            sut.Type.Should().Be(nameof(AwsTransitGateway));
            sut.Name.Should().Be("Test");
            sut.Arn.Should().Be("arn:aws:ec2:eu-west-1:000000000000:image/ImageId");
            sut.Region.Should().Be("eu-west-1");
        }

        [Fact]
        public async Task ShouldNotFailWhenPropertiesAreNull()
        {
            var account = GetAccount();
            var target = GetTarget(out var ec2Client);

            var resources = (await target.DiscoverResourcesAsync(account, "eu-west-1", default)).ToList();

            resources.Should().HaveCount(1).And.Subject.First().Should().BeOfType<AwsTransitGateway>();
            var sut = (AwsTransitGateway)resources[0];
            sut.Type.Should().Be(nameof(AwsTransitGateway));
            sut.Name.Should().Be("Test");
            sut.Arn.Should().Be("arn:aws:ec2:eu-west-1:000000000000:image/ImageId");
            sut.Region.Should().Be("eu-west-1");
        }

        private AwsAccount GetAccount() => new(new AwsAccount.AwsAccountIdentifier("000000000000", "Test"));

        private IResourceExplorer<AwsAccount, AwsAccount.AwsAccountIdentifier> GetTarget(out Mock<IAmazonEC2> client)
        {
            client = new Mock<IAmazonEC2>();
            var ec2Client = client.Object;
            return new AwsTransitGatewayResourceExplorer(_ => ec2Client);
        }
    }
}
