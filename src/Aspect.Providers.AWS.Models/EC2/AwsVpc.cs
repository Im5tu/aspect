using System.Collections.Generic;
using System.ComponentModel;

namespace Aspect.Providers.AWS.Models.EC2
{
    [Description("Amazon Virtual Private Cloud (Amazon VPC) is a service that lets you launch AWS resources in a logically isolated virtual network that you define. You have complete control over your virtual networking environment, including selection of your own IP address range, creation of subnets, and configuration of route tables and network gateways. You can use both IPv4 and IPv6 for most resources in your virtual private cloud, helping to ensure secure and easy access to resources and applications.")]
    public class AwsVpc : AwsResource
    {
        [Description("The current state of the VPC: available / pending")]
        public string? State { get; set; }

        [Description("The primary IPv4 CIDR block for the VPC.")]
        public string? CidrBlock { get; set; }

        [Description("The allowed tenancy of instances launched into the VPC.")]
        public string? InstanceTenancy { get; set; }

        [Description("Indicates whether the VPC is the default VPC.")]
        public bool IsDefault { get; set; }

        [Description("The ID of the AWS account that owns the VPC.")]
        public string? OwnerId { get; set; }

        [Description("The ID of the VPC.")]
        public string? Id { get; set; }

        [Description("The subnets that belong to the VPC")]
        public IEnumerable<VpcSubnet>? Subnets { get; set; }

        [Description("The endpoints that are associated with the VPC")]
        public IEnumerable<VpcEndpoint>? Endpoints { get; set; }

        [Description("The VPC's that this VPC is peered with using a peering connection")]
        public IEnumerable<VpcPeeringConnection>? PeeringConnections { get; set; }

        public AwsVpc(AwsAccount account, string arn, string name, IReadOnlyList<KeyValuePair<string, string>> tags, string region)
            : base(account, arn, name, tags, nameof(AwsVpc), region)
        {
        }

        [Description("A subnetwork or subnet is a logical subdivision of an IP network.")]
        public class VpcSubnet
        {
            [Description("The Availability Zone of the subnet.")]
            public string? AvailabilityZone { get; set; }

            [Description("The IPv4 CIDR block assigned to the subnet.")]
            public string? CidrBlock { get; set; }

            [Description("The Amazon Resource Name (ARN) of the Outpost.")]
            public string? OutpostArn { get; set; }

            [Description("The ID of the AWS account that owns the subnet.")]
            public string? OwnerId { get; set; }

            [Description("The ID of the subnet.")]
            public string? Id { get; set; }

            [Description("The Amazon Resource Name (ARN) of the subnet.")]
            public string? Arn { get; set; }

            [Description("Indicates whether this is the default subnet for the Availability Zone.")]
            public bool IsDefaultForAvailabilityZone { get; set; }

            [Description("The number of unused private IPv4 addresses in the subnet")]
            public int AvailableIpAddressCount { get; set; }

            [Description("Indicates whether instances launched in this subnet receive a public IPv4 address.")]
            public bool MapPublicIpOnLaunch { get; set; }
        }


        [Description("A VPC endpoint enables private connections between your VPC and supported AWS services and VPC endpoint services powered by AWS PrivateLink.  ")]
        public class VpcEndpoint
        {
            [Description("The ID of the VPC endpoint.")]
            public string? Id { get; set; }

            [Description("The ID of the AWS account that owns the VPC endpoint.")]
            public string? OwnerId { get; set; }

            [Description("The name of the service to which the endpoint is associated.")]
            public string? ServiceName { get; set; }

            [Description("Indicates whether the VPC endpoint is being managed by its service.")]
            public bool RequesterManaged { get; set; }

            [Description("(Interface endpoint) One or more subnets in which the endpoint is located.")]
            public IEnumerable<string>? SubnetIds { get; set; }

            [Description("One or more network interfaces for the endpoint.")]
            public IEnumerable<string>? NetworkInterfaces { get; set; }

            [Description("Indicates whether the VPC is associated with a private hosted zone")]
            public bool PrivateDnsEnabled { get; set; }

            [Description("(Gateway endpoint) One or more route tables associated with the endpoint.")]
            public IEnumerable<string>? RouteTables { get; set; }

            [Description("The state of the VPC endpoint.")]
            public string? State { get; set; }

            [Description("The type of endpoint.")]
            public string? Type { get; set; }

            [Description("(Interface endpoint) The security groups that are associated with the network interface.")]
            public IEnumerable<string>? SecurityGroups { get; set; }

            [Description("The DNS entries for the endpoint.")]
            public IEnumerable<string>? DnsEntries { get; set; }
        }

        [Description("A VPC peering connection is a networking connection between two VPCs that enables you to route traffic between them using private IPv4 addresses or IPv6 addresses.")]
        public class VpcPeeringConnection
        {
            [Description("The VPC ID of the accepter")]
            public string? Accepter { get; set; }

            [Description("The VPC ID of the requester")]
            public string? Requester { get; set; }

            [Description("Describes the status of a VPC peering connection.")]
            public string? Status { get; set; }

            [Description("The ID of the VPC peering connection.")]
            public string? Id { get; set; }
        }
    }
}
