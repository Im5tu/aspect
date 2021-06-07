using System;
using System.Collections.Generic;
using System.Linq;
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
                    Architecture = image.Architecture?.Value,
                    BootMode = image.BootMode?.Value,
                    Description = image.Description,
                    EnaSupport = image.EnaSupport,
                    Hypervisor = image.Hypervisor?.Value,
                    ImageId = image.ImageId,
                    ImageLocation = image.ImageLocation,
                    ImageType = image.ImageType?.Value,
                    IsPublic = image.Public,
                    KernelId = image.KernelId,
                    OwnerId = image.OwnerId,
                    Platform = image.Platform?.Value,
                    PlatformDetails = image.PlatformDetails,
                    RamdiskId = image.RamdiskId,
                    RootDeviceName = image.RootDeviceName,
                    RootDeviceType = image.RootDeviceType?.Value,
                    SriovNetSupport = image.SriovNetSupport,
                    State = image.State?.Value,
                    VirtualizationType = image.VirtualizationType?.Value,
                });
            }

            return result;
        }
    }
}
