using System;
using System.Collections.Generic;
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
    internal sealed class AwsEc2SnapshotResourceExplorer : AWSResourceExplorer
    {
        private readonly Func<AmazonEC2Config, IAmazonEC2> _creator;

        public AwsEc2SnapshotResourceExplorer(Func<AmazonEC2Config, IAmazonEC2> creator)
            : base(typeof(AwsEc2Snapshot))
        {
            _creator = creator;
        }

        public AwsEc2SnapshotResourceExplorer()
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
                var response = await ec2Client.DescribeSnapshotsAsync(new DescribeSnapshotsRequest { NextToken = nextToken, Filters = new List<Filter>
                {
                    new() { Name = "owner-id", Values = new() { account.Id.Id } }
                }}, cancellationToken);
                nextToken = response.NextToken;

                foreach (var ss in response.Snapshots)
                {
                    var arn = GenerateArn(account, region, "ec2", $"snapshot/{ss.SnapshotId}");
                    result.Add(new AwsEc2Snapshot(account, arn, ss.Tags.GetName(), ss.Tags.Convert(), region.SystemName)
                    {
                        DataEncryptionKeyId = ss.DataEncryptionKeyId.ValueOrEmpty(),
                        Description = ss.Description.ValueOrEmpty(),
                        Encrypted = ss.Encrypted,
                        Id = ss.SnapshotId.ValueOrEmpty(),
                        KmsKeyId = ss.KmsKeyId.ValueOrEmpty(),
                        OutpostArn = ss.OutpostArn.ValueOrEmpty(),
                        OwnerId = ss.OwnerId.ValueOrEmpty(),
                        Progress = ss.Progress.ValueOrEmpty(),
                        State = ss.State?.Value.ValueOrEmpty(),
                        VolumeId = ss.VolumeId.ValueOrEmpty(),
                        VolumeSize = ss.VolumeSize,
                    });

                }
            } while (!string.IsNullOrWhiteSpace(nextToken));

            return result;
        }
    }
}
