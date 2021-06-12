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
    public class AwsEc2VolumeResourceExplorerTests
    {
        [Fact]
        public void ShouldHaveCorrectType()
        {
            GetTarget(out _).ResourceType.Should().Be(typeof(AwsEc2Volume));
        }

        [Fact]
        public async Task ShouldMapAllPropertiesWhenNotNull()
        {
            var account = GetAccount();
            var target = GetTarget(out var ec2Client);
            ec2Client.Setup(x => x.DescribeVolumesAsync(It.Is<DescribeVolumesRequest>(y => y.NextToken == null), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new DescribeVolumesResponse
                {
                    Volumes = new List<Volume>
                    {
                        new()
                        {
                            Attachments = new List<VolumeAttachment>
                            {
                                new()
                                {
                                    Device = nameof(VolumeAttachment.Device),
                                    State = nameof(VolumeAttachment.State),
                                    InstanceId = nameof(VolumeAttachment.InstanceId),
                                    VolumeId = nameof(VolumeAttachment.VolumeId),
                                    DeleteOnTermination = true,
                                }
                            },
                            AvailabilityZone = nameof(Volume.AvailabilityZone),
                            Encrypted = true,
                            FastRestored = true,
                            VolumeId = nameof(Volume.VolumeId),
                            Iops = 100,
                            KmsKeyId = nameof(Volume.KmsKeyId),
                            MultiAttachEnabled = true,
                            OutpostArn = nameof(Volume.OutpostArn),
                            Size = 100,
                            SnapshotId = nameof(Volume.SnapshotId),
                            State = VolumeState.Available,
                            Throughput = 100,
                            VolumeType = VolumeType.Gp2,
                            Tags = new List<Tag>
                            {
                                new("Name", "Test")
                            }
                        }
                    }
                });
            var resources = (await target.DiscoverResourcesAsync(account, "eu-west-1", default)).ToList();

            resources.Should().HaveCount(1).And.Subject.First().Should().BeOfType<AwsEc2Volume>();
            var sut = (AwsEc2Volume)resources[0];
            sut.Type.Should().Be(nameof(AwsEc2Volume));
            sut.Name.Should().Be("Test");
            sut.Arn.Should().Be("arn:aws:ec2:eu-west-1:000000000000:volume/VolumeId");
            sut.Region.Should().Be("eu-west-1");
            sut.Attachments.Should().BeEquivalentTo(new List<AwsEc2Volume.VolumeAttachment>
            {
                new ()
                {
                    Device = nameof(VolumeAttachment.Device),
                    State = nameof(VolumeAttachment.State),
                    InstanceId = nameof(VolumeAttachment.InstanceId),
                    VolumeId = nameof(VolumeAttachment.VolumeId),
                    DeleteOnTermination = true
                }
            });
            sut.AvailabilityZone.Should().Be(nameof(Volume.AvailabilityZone));
            sut.Encrypted.Should().Be(true);
            sut.FastRestored.Should().Be(true);
            sut.Id.Should().Be(nameof(Volume.VolumeId));
            sut.Iops.Should().Be(100);
            sut.KmsKeyId.Should().Be(nameof(Volume.KmsKeyId));
            sut.MultiAttachEnabled.Should().Be(true);
            sut.OutpostArn.Should().Be(nameof(Volume.OutpostArn));
            sut.Size.Should().Be(100);
            sut.SnapshotId.Should().Be(nameof(Volume.SnapshotId));
            sut.State.Should().Be("available");
            sut.Throughput.Should().Be(100);
            sut.VolumeType.Should().Be("gp2");
        }

        [Fact]
        public async Task ShouldNotFailWhenPropertiesAreNull()
        {
            var account = GetAccount();
            var target = GetTarget(out var ec2Client);
            ec2Client.Setup(x => x.DescribeVolumesAsync(It.Is<DescribeVolumesRequest>(y => y.NextToken == null), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new DescribeVolumesResponse
                {
                    Volumes = new List<Volume>
                    {
                        new()
                        {
                            AvailabilityZone = nameof(Volume.AvailabilityZone),
                            Encrypted = true,
                            FastRestored = true,
                            VolumeId = nameof(Volume.VolumeId),
                            Iops = 100,
                            KmsKeyId = nameof(Volume.KmsKeyId),
                            MultiAttachEnabled = true,
                            OutpostArn = nameof(Volume.OutpostArn),
                            Size = 100,
                            SnapshotId = nameof(Volume.SnapshotId),
                            Throughput = 100,
                            Tags = new List<Tag>
                            {
                                new("Name", "Test")
                            }
                        }
                    }
                });
            var resources = (await target.DiscoverResourcesAsync(account, "eu-west-1", default)).ToList();

            resources.Should().HaveCount(1).And.Subject.First().Should().BeOfType<AwsEc2Volume>();
            var sut = (AwsEc2Volume)resources[0];
            sut.Type.Should().Be(nameof(AwsEc2Volume));
            sut.Name.Should().Be("Test");
            sut.Arn.Should().Be("arn:aws:ec2:eu-west-1:000000000000:volume/VolumeId");
            sut.Region.Should().Be("eu-west-1");
            sut.AvailabilityZone.Should().Be(nameof(Volume.AvailabilityZone));
            sut.Encrypted.Should().Be(true);
            sut.FastRestored.Should().Be(true);
            sut.Id.Should().Be(nameof(Volume.VolumeId));
            sut.Iops.Should().Be(100);
            sut.KmsKeyId.Should().Be(nameof(Volume.KmsKeyId));
            sut.MultiAttachEnabled.Should().Be(true);
            sut.OutpostArn.Should().Be(nameof(Volume.OutpostArn));
            sut.Size.Should().Be(100);
            sut.SnapshotId.Should().Be(nameof(Volume.SnapshotId));
            sut.Throughput.Should().Be(100);
        }

        private AwsAccount GetAccount() => new(new AwsAccount.AwsAccountIdentifier("000000000000", "Test"));

        private IResourceExplorer<AwsAccount, AwsAccount.AwsAccountIdentifier> GetTarget(out Mock<IAmazonEC2> client)
        {
            client = new Mock<IAmazonEC2>();
            var ec2Client = client.Object;
            return new AwsEc2VolumeResourceExplorer(_ => ec2Client);
        }
    }
}
