using System.Collections.Generic;
using System.ComponentModel;

namespace Aspect.Providers.AWS.Models.EC2
{
    [Description("A route table contains a set of rules, called routes, that are used to determine where network traffic from your subnet or gateway is directed.")]
    public class AwsRouteTable : AwsResource
    {
        [Description("The associations between the route table and one or more subnets or a gateway.")]
        public IEnumerable<Association>? Associations { get; init; }

        [Description("The routes in the route table.")]
        public IEnumerable<Route>? Routes { get; init; }

        [Description("A list of gateway ids for any virtual private gateway (VGW) propagating routes.")]
        public IEnumerable<string>? PropagatingVirtualGateways { get; init; }

        [Description("The ID of the AWS account that owns the route table.")]
        public string? OwnerId { get; init; }

        [Description("The ID of the VPC.")]
        public string? VpcId { get; init; }

        [Description("The ID of the route table.")]
        public string? Id { get; init; }

        public AwsRouteTable(AwsAccount account, string arn, string name, IReadOnlyList<KeyValuePair<string, string>> tags, string region)
            : base(account, arn, name, tags, nameof(AwsRouteTable), region)
        {
        }

        [Description("Describes an association between a route table and a subnet or gateway.")]
        public class Association
        {
            [Description("Indicates whether this is the main route table.")]
            public bool? IsPrimaryRouteTable { get; init; }

            [Description("The state of the association.")]
            public string? State { get; init; }

            [Description("The ID of the internet gateway or virtual private gateway.")]
            public string? GatewayId { get; init; }

            [Description("The ID of the subnet. A subnet ID is not returned for an implicit association.")]
            public string? SubnetId { get; init; }

            [Description("The ID of the route table.")]
            public string? RouteTableId { get; init; }

            [Description("The ID of the association.")]
            public string? Id { get; init; }
        }

        [Description("Describes a route in a route table.")]
        public class Route
        {
            [Description("Describes how the route was created. One of: CreateRouteTable/CreateRoute/EnableVgwRoutePropagation")]
            public string? Origin { get; init; }

            [Description("The state of the route. One of: Active/Blackhole")]
            public string? State { get; init; }

            [Description("The ID of a gateway attached to your VPC.")]
            public string? GatewayId { get; init; }

            [Description("The ID of a NAT instance in your VPC.")]
            public string? NatInstanceId { get; init; }

            [Description("The ID of the carrier gateway.")]
            public string? CarrierGatewayId { get; init; }

            [Description("The IPv4 CIDR block used for the destination match.")]
            public string? DestinationCidrBlock { get; init; }

            [Description("The AWS account ID of the owner of the NAT instance.")]
            public string? NatInstanceOwnerId { get; init; }

            [Description("The ID of the local gateway.")]
            public string? LocalGatewayId { get; init; }

            [Description("The ID of a NAT gateway.")]
            public string? NatGatewayId { get; init; }

            [Description("The ID of the network interface.")]
            public string? NetworkInterfaceId { get; init; }

            [Description("The ID of a transit gateway.")]
            public string? TransitGatewayId { get; init; }

            [Description("The IPv6 CIDR block used for the destination match.")]
            public string? DestinationIpv6CidrBlock { get; init; }

            [Description("The prefix of the AWS service.")]
            public string? DestinationPrefixListId { get; init; }

            [Description("The ID of a VPC peering connection.")]
            public string? VpcPeeringConnectionId { get; init; }

            [Description("The ID of the egress-only internet gateway.")]
            public string? EgressOnlyInternetGatewayId { get; init; }
        }
    }
}
