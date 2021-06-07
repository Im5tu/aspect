using System.Collections.Generic;
using System.ComponentModel;

namespace Aspect.Providers.AWS.Models.EC2
{
    [Description("")]
    public class AwsEc2Volume : AwsResource
    {
        [Description("Describes the attachments to an EC2 instances.")]
        public IEnumerable<AwsEc2Volume.VolumeAttachment>? Attachments { get; init; }

        [Description("Indicates whether the volume is encrypted.")]
        public bool? Encrypted { get; init; }

        [Description("The number of I/O operations per second (IOPS). For gp3, io1 and io2 volumes, this represents the number of IOPS that are provisioned for the volume. For gp2 volumes, this represents the baseline performance of the volume and the rate at which the volume accumulates I/O credits for bursting.")]
        public int? Iops { get; init; }

        [Description("The size of the volume, in GiBs.")]
        public int? Size { get; init; }

        [Description("The volume state.")]
        public string? State { get; init; }

        [Description("The throughput that the volume supports, in MiB/s.")]
        public int? Throughput { get; init; }

        [Description("The Availability Zone for the volume.")]
        public string? AvailabilityZone { get; init; }

        [Description("Indicates whether the volume was created using fast snapshot restore.")]
        public bool? FastRestored { get; init; }

        [Description("The Amazon Resource Name (ARN) of the Outpost.")]
        public string? OutpostArn { get; init; }

        [Description("The snapshot from which the volume was created, if applicable.")]
        public string? SnapshotId { get; init; }

        [Description("The ID of the volume.")]
        public string? Id { get; init; }

        [Description("The volume type")]
        public string? VolumeType { get; init; }

        [Description("The Amazon Resource Name (ARN) of the AWS Key Management Service (AWS KMS) customer master key (CMK) that was used to protect the volume encryption key for the volume.")]
        public string? KmsKeyId { get; init; }

        [Description("Indicates whether Amazon EBS Multi-Attach is enabled.")]
        public bool? MultiAttachEnabled { get; init; }

        public AwsEc2Volume(AwsAccount account, string arn, string name, IReadOnlyList<KeyValuePair<string, string>> tags, string region)
            : base(account, arn, name, tags, nameof(AwsEc2Volume), region)
        {
        }

        [Description("Describes the attachment to an EC2 instance.")]
        public class VolumeAttachment
        {
            [Description("Indicates whether the EBS volume is deleted on instance termination.")]
            public bool? DeleteOnTermination { get; init; }

            [Description("The device name.")]
            public string? Device { get; init; }

            [Description("The ID of the instance.")]
            public string? InstanceId { get; init; }

            [Description("The attachment state of the volume.")]
            public string? State { get; init; }

            [Description("The ID of the volume.")]
            public string? VolumeId { get; init; }
        }
    }
}
