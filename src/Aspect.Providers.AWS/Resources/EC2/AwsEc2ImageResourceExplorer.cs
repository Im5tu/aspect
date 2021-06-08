using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amazon;
using Amazon.EC2;
using Aspect.Abstractions;
using Aspect.Providers.AWS.Models;
using Aspect.Providers.AWS.Models.EC2;

namespace Aspect.Providers.AWS.Resources.EC2
{
    internal sealed class AwsEc2ImageResourceExplorer : AWSResourceExplorer
    {
        private readonly Func<AmazonEC2Config, IAmazonEC2> _creator;

        public AwsEc2ImageResourceExplorer(Func<AmazonEC2Config, IAmazonEC2> creator)
            : base(typeof(AwsEc2Image))
        {
            _creator = creator;
        }

        public AwsEc2ImageResourceExplorer()
            : this(config => new AmazonEC2Client(config))
        {
        }

        protected override async Task<IEnumerable<IResource>> DiscoverResourcesAsync(AwsAccount account, RegionEndpoint region, CancellationToken cancellationToken)
        {
            using var ec2Client = _creator(new AmazonEC2Config { RegionEndpoint = region });
            var result = new List<IResource>();
            var response = await ec2Client.DescribeImagesAsync(cancellationToken);

            foreach (var image in response.Images)
            {
                var arn = GenerateArn(account, region, "ec2", $"image/{image.ImageId}");
                result.Add(new AwsEc2Image(account, arn, image.Name ?? string.Empty, image.Tags.Convert(), region.SystemName)
                {
                    Architecture = image.Architecture?.Value.ValueOrEmpty(),
                    BootMode = image.BootMode?.Value.ValueOrEmpty(),
                    Description = image.Description.ValueOrEmpty(),
                    EnaSupport = image.EnaSupport,
                    Hypervisor = image.Hypervisor?.Value.ValueOrEmpty(),
                    ImageId = image.ImageId.ValueOrEmpty(),
                    ImageLocation = image.ImageLocation.ValueOrEmpty(),
                    ImageType = image.ImageType?.Value.ValueOrEmpty(),
                    IsPublic = image.Public,
                    KernelId = image.KernelId.ValueOrEmpty(),
                    OwnerId = image.OwnerId.ValueOrEmpty(),
                    Platform = image.Platform?.Value.ValueOrEmpty(),
                    PlatformDetails = image.PlatformDetails.ValueOrEmpty(),
                    RamdiskId = image.RamdiskId.ValueOrEmpty(),
                    RootDeviceName = image.RootDeviceName.ValueOrEmpty(),
                    RootDeviceType = image.RootDeviceType?.Value.ValueOrEmpty(),
                    SriovNetSupport = image.SriovNetSupport.ValueOrEmpty(),
                    State = image.State?.Value.ValueOrEmpty(),
                    VirtualizationType = image.VirtualizationType?.Value.ValueOrEmpty(),
                });
            }

            return result;
        }
    }
}
