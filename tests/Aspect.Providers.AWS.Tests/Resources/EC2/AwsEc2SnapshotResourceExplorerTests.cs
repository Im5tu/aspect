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
    public class AwsEc2SnapshotResourceExplorerTests
    {
        [Fact]
        public void ShouldHaveCorrectType()
        {
            GetTarget(out _).ResourceType.Should().Be(typeof(AwsEc2Snapshot));
        }

        [Fact]
        public async Task ShouldGetAllValuesUsingNextTokens()
        {
            var account = GetAccount();
            var target = GetTarget(out var ec2Client);

            ec2Client.Setup(x => x.DescribeSnapshotsAsync(It.Is<DescribeSnapshotsRequest>(y => y.NextToken == null), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new DescribeSnapshotsResponse
                {
                    NextToken = "next",
                    Snapshots = new List<Snapshot>
                    {
                        new()
                    }
                });
            ec2Client.Setup(x => x.DescribeSnapshotsAsync(It.Is<DescribeSnapshotsRequest>(y => y.NextToken == "next"), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new DescribeSnapshotsResponse
                {
                    Snapshots = new List<Snapshot>
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
            ec2Client.Setup(x => x.DescribeSnapshotsAsync(It.Is<DescribeSnapshotsRequest>(y => y.NextToken == null),It.IsAny<CancellationToken>()))
                .ReturnsAsync(new DescribeSnapshotsResponse
                {
                    Snapshots = new List<Snapshot>
                    {
                        new()
                        {
                            DataEncryptionKeyId = nameof(Snapshot.DataEncryptionKeyId),
                            Description = nameof(Snapshot.Description),
                            Encrypted = true,
                            SnapshotId = nameof(Snapshot.SnapshotId),
                            KmsKeyId = nameof(Snapshot.KmsKeyId),
                            OutpostArn = nameof(Snapshot.OutpostArn),
                            OwnerId = nameof(Snapshot.OwnerId),
                            Progress = nameof(Snapshot.Progress),
                            State = SnapshotState.Completed,
                            Tags = new List<Tag>
                            {
                                new()
                                {
                                    Key = "Name",
                                    Value = "Test"
                                }
                            },
                            VolumeId = nameof(Snapshot.VolumeId),
                            VolumeSize = 10,
                        }
                    }
                });
            var resources = (await target.DiscoverResourcesAsync(account, "eu-west-1", default)).ToList();

            resources.Should().HaveCount(1).And.Subject.First().Should().BeOfType<AwsEc2Snapshot>();
            var sut = (AwsEc2Snapshot)resources[0];
            sut.Type.Should().Be(nameof(AwsEc2Snapshot));
            sut.Name.Should().Be("Test");
            sut.CloudId.Should().Be("arn:aws:ec2:eu-west-1:000000000000:snapshot/SnapshotId");
            sut.Region.Should().Be("eu-west-1");
            sut.DataEncryptionKeyId.Should().Be(nameof(Snapshot.DataEncryptionKeyId));
            sut.Description.Should().Be(nameof(Snapshot.Description));
            sut.Encrypted.Should().Be(true);
            sut.Id.Should().Be(nameof(Snapshot.SnapshotId));
            sut.KmsKeyId.Should().Be(nameof(Snapshot.KmsKeyId));
            sut.OutpostArn.Should().Be(nameof(Snapshot.OutpostArn));
            sut.OwnerId.Should().Be(nameof(Snapshot.OwnerId));
            sut.Progress.Should().Be(nameof(Snapshot.Progress));
            sut.State.Should().Be(SnapshotState.Completed);
            sut.VolumeId.Should().Be(nameof(Snapshot.VolumeId));
            sut.VolumeSize.Should().Be(10);
        }

        [Fact]
        public async Task ShouldNotFailWhenPropertiesAreNull()
        {
            var account = GetAccount();
            var target = GetTarget(out var ec2Client);
            ec2Client.Setup(x => x.DescribeSnapshotsAsync(It.Is<DescribeSnapshotsRequest>(y => y.NextToken == null),It.IsAny<CancellationToken>()))
                .ReturnsAsync(new DescribeSnapshotsResponse
                {
                    Snapshots = new List<Snapshot>
                    {
                        new()
                        {
                            DataEncryptionKeyId = nameof(Snapshot.DataEncryptionKeyId),
                            Description = nameof(Snapshot.Description),
                            Encrypted = true,
                            SnapshotId = nameof(Snapshot.SnapshotId),
                            KmsKeyId = nameof(Snapshot.KmsKeyId),
                            OutpostArn = nameof(Snapshot.OutpostArn),
                            OwnerId = nameof(Snapshot.OwnerId),
                            Progress = nameof(Snapshot.Progress),
                            Tags = new List<Tag>
                            {
                                new()
                                {
                                    Key = "Name",
                                    Value = "Test"
                                }
                            },
                            VolumeId = nameof(Snapshot.VolumeId),
                            VolumeSize = 10,
                        }
                    }
                });
            var resources = (await target.DiscoverResourcesAsync(account, "eu-west-1", default)).ToList();

            resources.Should().HaveCount(1).And.Subject.First().Should().BeOfType<AwsEc2Snapshot>();
            var sut = (AwsEc2Snapshot)resources[0];
            sut.Type.Should().Be(nameof(AwsEc2Snapshot));
            sut.Name.Should().Be("Test");
            sut.CloudId.Should().Be("arn:aws:ec2:eu-west-1:000000000000:snapshot/SnapshotId");
            sut.Region.Should().Be("eu-west-1");
            sut.DataEncryptionKeyId.Should().Be(nameof(Snapshot.DataEncryptionKeyId));
            sut.Description.Should().Be(nameof(Snapshot.Description));
            sut.Encrypted.Should().Be(true);
            sut.Id.Should().Be(nameof(Snapshot.SnapshotId));
            sut.KmsKeyId.Should().Be(nameof(Snapshot.KmsKeyId));
            sut.OutpostArn.Should().Be(nameof(Snapshot.OutpostArn));
            sut.OwnerId.Should().Be(nameof(Snapshot.OwnerId));
            sut.Progress.Should().Be(nameof(Snapshot.Progress));
            sut.State.Should().BeNull();
            sut.VolumeId.Should().Be(nameof(Snapshot.VolumeId));
            sut.VolumeSize.Should().Be(10);
        }

        private AwsAccount GetAccount() => new(new AwsAccount.AwsAccountIdentifier("000000000000", "Test"));

        private IResourceExplorer<AwsAccount, AwsAccount.AwsAccountIdentifier> GetTarget(out Mock<IAmazonEC2> client)
        {
            client = new Mock<IAmazonEC2>();
            var ec2Client = client.Object;
            return new AwsEc2SnapshotResourceExplorer(_ => ec2Client);
        }
    }
}
