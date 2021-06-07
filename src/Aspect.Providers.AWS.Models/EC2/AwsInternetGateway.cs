using System.Collections.Generic;
using System.ComponentModel;

namespace Aspect.Providers.AWS.Models.EC2
{
    [Description("Internet gateway are horizontally scaled, redundant, and highly available VPC components that allows communication between your VPC and the internet.")]
    public class AwsInternetGateway : AwsResource
    {
        [Description("The ID of the internet gateway.")]
        public string? Id { get; init; }

        [Description("The ID of the AWS account that owns the internet gateway.")]
        public string? OwnerId { get; init; }

        [Description("Any VPCs attached to the internet gateway.")]
        public IEnumerable<Attachment>? Attachments { get; init; }

        public AwsInternetGateway(AwsAccount account, string arn, string name, IReadOnlyList<KeyValuePair<string, string>> tags, string region)
            : base(account, arn, name, tags, nameof(AwsInternetGateway), region)
        {
        }

        [Description("Describes the attachment of a VPC to an internet gateway or an egress-only internet gateway.")]
        public class Attachment
        {
            [Description("The current state of the attachment.")]
            public string? State { get; init; }

            [Description("The ID of the VPC.")]
            public string? VpcId { get; init; }
        }
    }
}
