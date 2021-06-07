using System.Collections.Generic;
using System.ComponentModel;

namespace Aspect.Providers.AWS.Models.EC2
{
    [Description("Amazon EBS provides the ability to create snapshots (backups) of any EBS volume. A snapshot takes a copy of the EBS volume and places it in Amazon S3, where it is stored redundantly in multiple Availability Zones.")]
    public class AwsEc2Snapshot : AwsResource
    {
        [Description("The data encryption key identifier for the snapshot. This value is a unique identifier that corresponds to the data encryption key that was used to encrypt the original volume or snapshot copy. Because data encryption keys are inherited by volumes created from snapshots, and vice versa, if snapshots share the same data encryption key identifier, then they belong to the same volume/snapshot lineage.")]
        public string? DataEncryptionKeyId { get; init; }

        [Description("The description for the snapshot.")]
        public string? Description { get; init; }

        [Description("Indicates whether the snapshot is encrypted.")]
        public bool Encrypted { get; init; }

        [Description("The ID of the snapshot.")]
        public string? Id { get; init; }

        [Description("The Amazon Resource Name (ARN) of the AWS Key Management Service (AWS KMS) customer master key (CMK) that was used to protect the volume encryption key for the parent volume.")]
        public string? KmsKeyId { get; init; }

        [Description("The ARN of the AWS Outpost on which the snapshot is stored.")]
        public string? OutpostArn { get; init; }

        [Description("The AWS account ID of the EBS snapshot owner.")]
        public string? OwnerId { get; init; }

        [Description("The progress of the snapshot, as a percentage.")]
        public string? Progress { get; init; }

        [Description("The snapshot state.")]
        public string? State { get; init; }

        [Description("The ID of the volume that was used to create the snapshot.")]
        public string? VolumeId { get; init; }

        [Description("The size of the volume, in GiB.")]
        public int VolumeSize { get; init; }

        public AwsEc2Snapshot(AwsAccount account, string arn, string name, IReadOnlyList<KeyValuePair<string, string>> tags, string region)
            : base(account, arn, name, tags, nameof(AwsEc2Snapshot), region)
        {
        }
    }
}
