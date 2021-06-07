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
    public class AwsEc2ImageResourceExplorerTests
    {
        [Fact]
        public void ShouldHaveCorrectType()
        {
            GetTarget(out _).ResourceType.Should().Be(typeof(AwsEc2Image));
        }

        [Fact]
        public async Task ShouldMapAllPropertiesWhenNotNull()
        {
            var account = GetAccount();
            var target = GetTarget(out var ec2Client);

            ec2Client.Setup(x => x.DescribeImagesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new DescribeImagesResponse
                {
                    Images = new List<Image>
                    {
                        new Image
                        {
                            Architecture = nameof(Image.Architecture),
                            BootMode = BootModeValues.Uefi,
                            Description = nameof(Image.Description),
                            EnaSupport = true,
                            Hypervisor = nameof(Image.Hypervisor),
                            ImageId = nameof(Image.ImageId),
                            ImageLocation = nameof(Image.ImageLocation),
                            ImageType = ImageTypeValues.Machine,
                            Public = true,
                            KernelId = nameof(Image.KernelId),
                            Name = "Test",
                            OwnerId = nameof(Image.OwnerId),
                            Platform = PlatformValues.Windows,
                            PlatformDetails = nameof(Image.PlatformDetails),
                            RamdiskId = nameof(Image.RamdiskId),
                            RootDeviceName = nameof(Image.RootDeviceName),
                            RootDeviceType = RootDeviceType.InstanceStore.Value,
                            SriovNetSupport = nameof(Image.SriovNetSupport),
                            State = State.Available.Value,
                            VirtualizationType = VirtualizationType.Hvm,
                            Tags = new List<Tag>
                            {
                                new("Name", "Test")
                            }
                        }
                    }
                });

            var resources = (await target.DiscoverResourcesAsync(account, "eu-west-1", default)).ToList();

            resources.Should().HaveCount(1).And.Subject.First().Should().BeOfType<AwsEc2Image>();
            var sut = (AwsEc2Image)resources[0];
            sut.Type.Should().Be(nameof(AwsEc2Image));
            sut.Name.Should().Be("Test");
            sut.Arn.Should().Be("arn:aws:ec2:eu-west-1:000000000000:image/ImageId");
            sut.Region.Should().Be("eu-west-1");
            sut.Architecture.Should().Be(nameof(Image.Architecture));
            sut.BootMode.Should().Be("uefi");
            sut.Description.Should().Be(nameof(Image.Description));
            sut.EnaSupport.Should().Be(true);
            sut.Hypervisor.Should().Be(nameof(Image.Hypervisor));
            sut.ImageId.Should().Be(nameof(Image.ImageId));
            sut.ImageLocation.Should().Be(nameof(Image.ImageLocation));
            sut.ImageType.Should().Be("machine");
            sut.IsPublic.Should().Be(true);
            sut.KernelId.Should().Be(nameof(Image.KernelId));
            sut.OwnerId.Should().Be(nameof(Image.OwnerId));
            sut.Platform.Should().Be("Windows");
            sut.PlatformDetails.Should().Be(nameof(Image.PlatformDetails));
            sut.RamdiskId.Should().Be(nameof(Image.RamdiskId));
            sut.RootDeviceName.Should().Be(nameof(Image.RootDeviceName));
            sut.RootDeviceType.Should().Be("instance-store");
            sut.State.Should().Be("available");
            sut.VirtualizationType.Should().Be("hvm");
            sut.Tags.Should().HaveCount(1).And.Subject.First().Key.Should().Be("Name");
        }

        [Fact]
        public async Task ShouldNotFailWhenPropertiesAreNull()
        {
            var account = GetAccount();
            var target = GetTarget(out var ec2Client);

            ec2Client.Setup(x => x.DescribeImagesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new DescribeImagesResponse
                {
                    Images = new List<Image>
                    {
                        new Image
                        {
                            Architecture = nameof(Image.Architecture),
                            Description = nameof(Image.Description),
                            EnaSupport = true,
                            Hypervisor = nameof(Image.Hypervisor),
                            ImageId = nameof(Image.ImageId),
                            ImageLocation = nameof(Image.ImageLocation),
                            Public = true,
                            KernelId = nameof(Image.KernelId),
                            Name = "Test",
                            OwnerId = nameof(Image.OwnerId),
                            PlatformDetails = nameof(Image.PlatformDetails),
                            RamdiskId = nameof(Image.RamdiskId),
                            RootDeviceName = nameof(Image.RootDeviceName),
                            SriovNetSupport = nameof(Image.SriovNetSupport),
                            Tags = new List<Tag>
                            {
                                new("Name", "Test")
                            }
                        }
                    }
                });

            var resources = (await target.DiscoverResourcesAsync(account, "eu-west-1", default)).ToList();

            resources.Should().HaveCount(1).And.Subject.First().Should().BeOfType<AwsEc2Image>();
            var sut = (AwsEc2Image)resources[0];
            sut.Type.Should().Be(nameof(AwsEc2Image));
            sut.Name.Should().Be("Test");
            sut.Arn.Should().Be("arn:aws:ec2:eu-west-1:000000000000:image/ImageId");
            sut.Region.Should().Be("eu-west-1");
            sut.Architecture.Should().Be(nameof(Image.Architecture));
            sut.BootMode.Should().BeNull();
            sut.Description.Should().Be(nameof(Image.Description));
            sut.EnaSupport.Should().Be(true);
            sut.Hypervisor.Should().Be(nameof(Image.Hypervisor));
            sut.ImageId.Should().Be(nameof(Image.ImageId));
            sut.ImageLocation.Should().Be(nameof(Image.ImageLocation));
            sut.ImageType.Should().BeNull();
            sut.IsPublic.Should().Be(true);
            sut.KernelId.Should().Be(nameof(Image.KernelId));
            sut.OwnerId.Should().Be(nameof(Image.OwnerId));
            sut.Platform.Should().BeNull();
            sut.PlatformDetails.Should().Be(nameof(Image.PlatformDetails));
            sut.RamdiskId.Should().Be(nameof(Image.RamdiskId));
            sut.RootDeviceName.Should().Be(nameof(Image.RootDeviceName));
            sut.RootDeviceType.Should().BeNull();
            sut.State.Should().BeNull();
            sut.VirtualizationType.Should().BeNull();
            sut.Tags.Should().HaveCount(1).And.Subject.First().Key.Should().Be("Name");
        }

        private AwsAccount GetAccount() => new AwsAccount(new AwsAccount.AwsAccountIdentifier("000000000000", "Test"));

        private IResourceExplorer<AwsAccount, AwsAccount.AwsAccountIdentifier> GetTarget(out Mock<IAmazonEC2> client)
        {
            client = new Mock<IAmazonEC2>();
            var ec2Client = client.Object;
            return new AwsEc2ImageResourceExplorer(_ => ec2Client);
        }
    }
}
