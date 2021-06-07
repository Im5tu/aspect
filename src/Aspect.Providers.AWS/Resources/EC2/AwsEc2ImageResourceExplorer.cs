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
        public AwsEc2ImageResourceExplorer()
            : base(typeof(AwsEc2Image))
        {
        }

        protected override async Task<IEnumerable<IResource>> DiscoverResourcesAsync(AwsAccount account, RegionEndpoint region, CancellationToken cancellationToken)
        {
            using var ec2Client = new AmazonEC2Client(new AmazonEC2Config { RegionEndpoint = region });
            var result = new List<IResource>();
            var response = await ec2Client.DescribeImagesAsync(cancellationToken);

            foreach (var image in response.Images)
            {
                var arn = $"arn:aws:ec2:{region.SystemName}:{account.Id.Id}:image/{image.ImageId}";
                result.Add(new AwsEc2Image(account, arn, image.Name ?? string.Empty, image.Tags.ConvertTags(), region.SystemName)
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
