using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon;
using Amazon.EC2;
using Amazon.EC2.Model;
using Aspect.Abstractions;
using Aspect.Providers.AWS.Models;
using Aspect.Providers.AWS.Models.EC2;

namespace Aspect.Providers.AWS.Resources.EC2
{
    internal sealed class AwsEc2VolumeResourceExplorer : AWSResourceExplorer
    {
        private readonly Func<AmazonEC2Config, IAmazonEC2> _creator;

        public AwsEc2VolumeResourceExplorer(Func<AmazonEC2Config, IAmazonEC2> creator)
            : base(typeof(AwsEc2Volume))
        {
            _creator = creator;
        }

        public AwsEc2VolumeResourceExplorer()
            : this(config => new AmazonEC2Client(config))
        {
        }

        protected override async Task<IEnumerable<IResource>> DiscoverResourcesAsync(AwsAccount account, RegionEndpoint region, CancellationToken cancellationToken)
        {
            using var ec2Client = _creator(new AmazonEC2Config { RegionEndpoint = region });
            var result = new List<IResource>();

            string? nextToken = null;
            do
            {
                var response = await ec2Client.DescribeVolumesAsync(new DescribeVolumesRequest { NextToken = nextToken }, cancellationToken);

                foreach (var vol in response.Volumes)
                {
                    var arn = GenerateArn(account, region, "ec2", $"volume/{vol.VolumeId}");
                    result.Add(new AwsEc2Volume(account, arn, vol.Tags.GetName(), vol.Tags.Convert(), region.SystemName)
                    {
                        Attachments = Map(vol.Attachments),
                        AvailabilityZone = vol.AvailabilityZone,
                        Encrypted = vol.Encrypted,
                        FastRestored = vol.FastRestored,
                        Id = vol.VolumeId,
                        Iops = vol.Iops,
                        KmsKeyId = vol.KmsKeyId,
                        MultiAttachEnabled = vol.MultiAttachEnabled,
                        OutpostArn = vol.OutpostArn,
                        Size = vol.Size,
                        SnapshotId = vol.SnapshotId,
                        State = vol.State?.Value,
                        Throughput = vol.Throughput,
                        VolumeType = vol.VolumeType?.Value,
                    });
                }

            } while (!string.IsNullOrWhiteSpace(nextToken));

            return result;
        }

        private IEnumerable<AwsEc2Volume.VolumeAttachment> Map(List<VolumeAttachment>? volAttachments)
        {
            var result = new List<AwsEc2Volume.VolumeAttachment>();

            foreach (var volAttachment in volAttachments ?? Enumerable.Empty<VolumeAttachment>())
            {
                result.Add(new AwsEc2Volume.VolumeAttachment
                {
                    DeleteOnTermination = volAttachment.DeleteOnTermination,
                    Device = volAttachment.Device,
                    InstanceId = volAttachment.InstanceId,
                    State = volAttachment.State?.Value,
                    VolumeId = volAttachment.VolumeId
                });
            }

            return result;
        }
    }
}
