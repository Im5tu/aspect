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
    public class AwsPrefixListResourceExplorerTests
    {
        [Fact]
        public void ShouldHaveCorrectType()
        {
            GetTarget(out _).ResourceType.Should().Be(typeof(AwsPrefixList));
        }

        [Fact]
        public async Task ShouldMapAllPropertiesWhenNotNull()
        {
            var account = GetAccount();
            var target = GetTarget(out var ec2Client);
            ec2Client.Setup(x => x.DescribeManagedPrefixListsAsync(It.IsAny<DescribeManagedPrefixListsRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new DescribeManagedPrefixListsResponse
                {
                    PrefixLists = new List<ManagedPrefixList>
                    {
                        new()
                        {
                            State = PrefixListState.CreateComplete,
                            OwnerId = nameof(ManagedPrefixList.OwnerId),
                            PrefixListId = nameof(ManagedPrefixList.PrefixListId),
                            PrefixListArn = nameof(ManagedPrefixList.PrefixListArn),
                            PrefixListName = nameof(ManagedPrefixList.PrefixListName),
                            AddressFamily = nameof(ManagedPrefixList.AddressFamily),
                            Tags = new List<Tag>
                            {
                                new() { Key = "Name", Value = nameof(ManagedPrefixList.PrefixListName) }
                            }
                        }
                    }
                });
            var resources = (await target.DiscoverResourcesAsync(account, "eu-west-1", default)).ToList();

            resources.Should().HaveCount(1).And.Subject.First().Should().BeOfType<AwsPrefixList>();
            var sut = (AwsPrefixList)resources[0];
            sut.Type.Should().Be(nameof(AwsPrefixList));
            sut.Name.Should().Be(nameof(ManagedPrefixList.PrefixListName));
            sut.Arn.Should().Be(nameof(ManagedPrefixList.PrefixListArn));
            sut.Region.Should().Be("eu-west-1");
            sut.AddressFamily.Should().Be(nameof(ManagedPrefixList.AddressFamily));
            sut.Id.Should().Be(nameof(ManagedPrefixList.PrefixListId));
            sut.OwnerId.Should().Be(nameof(ManagedPrefixList.OwnerId));
            sut.State.Should().Be("create-complete");
        }

        private AwsAccount GetAccount() => new(new AwsAccount.AwsAccountIdentifier("000000000000", "Test"));

        private IResourceExplorer<AwsAccount, AwsAccount.AwsAccountIdentifier> GetTarget(out Mock<IAmazonEC2> client)
        {
            client = new Mock<IAmazonEC2>();
            var ec2Client = client.Object;
            return new AwsPrefixListResourceExplorer(_ => ec2Client);
        }
    }
}
