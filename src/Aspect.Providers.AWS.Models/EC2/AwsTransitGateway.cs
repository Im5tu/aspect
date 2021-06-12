using System.Collections.Generic;
using System.ComponentModel;

namespace Aspect.Providers.AWS.Models.EC2
{
    [Description("AWS Transit Gateway connects VPCs and on-premises networks through a central hub. This simplifies your network and puts an end to complex peering relationships. It acts as a cloud router – each new connection is only made once.")]
    public class AwsTransitGateway : AwsResource
    {
        [Description("The current configuration of the transit gateway")]
        public Config? Configuration { get; init; }

        [Description("The state of the transit gateway: available / deleted / deleting / modifying / pending")]
        public string? State { get; init; }

        [Description("The ID of the transit gateway.")]
        public string? Id { get; init; }

        [Description("The description of the transit gateway.")]
        public string? Description { get; init; }

        [Description("The ID of the AWS account ID that owns the transit gateway.")]
        public string? OwnerId { get; init; }


        public AwsTransitGateway(AwsAccount account, string arn, string name, IReadOnlyList<KeyValuePair<string, string>> tags, string region)
            : base(account, arn, name, tags, nameof(AwsTransitGateway), region)
        {
        }

        [Description("Describes the current configuration for a transit gateway.")]
        public class Config
        {
            [Description("Indicates whether or not the transit gateway has DNS support enabled")]
            public bool SupportsDNS { get; init; }

            [Description("Indicates whether or not the transit gateway has multicasting enabled")]
            public bool SupportsMulticast { get; init; }

            [Description("A private Autonomous System Number (ASN) for the Amazon side of a BGP session")]
            public long AmazonAsn { get; init; }

            [Description("Indicates whether or not the transit gateway has equal-cost multi-path routing (ECMP) enabled")]
            public bool SupportsECMP { get; init; }

            [Description("Indicates whether attachment requests are automatically accepted.")]
            public bool AutoAcceptsSharedAttachments { get; init; }

            [Description("Indicates whether resource attachments are automatically associated with the default association route table.")]
            public bool DefaultRouteTableAssociation { get; init; }

            [Description("Indicates whether resource attachments automatically propagate routes to the default propagation route table.")]
            public bool DefaultRouteTablePropagation { get; init; }

            [Description("The transit gateway CIDR blocks.")]
            public IEnumerable<string>? TransitGatewayCidrBlocks { get; init; }

            [Description("The ID of the default association route table.")]
            public string? AssociationDefaultRouteTableId { get; init; }

            [Description("The ID of the default propagation route table.")]
            public string? PropagationDefaultRouteTableId { get; init; }
        }
    }
}
