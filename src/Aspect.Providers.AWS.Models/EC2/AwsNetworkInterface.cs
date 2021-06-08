using System.Collections.Generic;
using System.ComponentModel;

namespace Aspect.Providers.AWS.Models.EC2
{
    [Description("An elastic network interface is a logical networking component in a VPC that represents a virtual network card.")]
    public class AwsNetworkInterface : AwsResource
    {
        [Description("The Availability Zone.")]
        public string? AvailabilityZone { get; init; }

        [Description("A description.")]
        public string? Description { get; init; }

        [Description("The type of network interface.")]
        public string? InterfaceType { get; init; }

        [Description("The private IPv4 addresses associated with the network interface.")]
        public IEnumerable<string>? IpAddresses { get; init; }

        [Description("The IPv6 addresses associated with the network interface.")]
        public IEnumerable<string>? Ipv6Addresses { get; init; }

        [Description("The MAC address.")]
        public string? MacAddress { get; init; }

        [Description("The ID of the network interface.")]
        public string? Id { get; init; }

        [Description("The Amazon Resource Name (ARN) of the Outpost.")]
        public string? OutpostArn { get; init; }

        [Description("The AWS account ID of the owner of the network interface.")]
        public string? OwnerId { get; init; }

        [Description("The private DNS name.")]
        public string? PrivateDnsName { get; init; }

        [Description("The alias or AWS account ID of the principal or service that created the network interface.")]
        public string? RequesterId { get; init; }

        [Description("Indicates whether the network interface is being managed by AWS.")]
        public bool RequesterManaged { get; init; }

        [Description("Indicates whether source/destination checking is enabled.")]
        public bool SourceDestCheck { get; init; }

        [Description("The status of the network interface.")]
        public string? Status { get; init; }

        [Description("The ID of the subnet.")]
        public string? SubnetId { get; init; }

        [Description("The ID of the VPC.")]
        public string? VpcId { get; init; }

        [Description("The association information for an Elastic IP address (IPv4) associated with the network interface.")]
        public NetworkAssociation? Association { get; init; }

        [Description("The network interface attachment.")]
        public NetworkAttachment? Attachment { get; init; }

        [Description("Any security groups for the network interface.")]
        public IEnumerable<Group>? Groups { get; init; }

        public AwsNetworkInterface(AwsAccount account, string arn, string name, IReadOnlyList<KeyValuePair<string, string>> tags, string region)
            : base(account, arn, name, tags, nameof(AwsNetworkInterface), region)
        {
        }

        [Description("Describes association information for an Elastic IP address (IPv4 only), or a Carrier IP address (for a network interface which resides in a subnet in a Wavelength Zone).")]
        public class NetworkAssociation
        {
            [Description("The allocation ID.")]
            public string? AllocationId { get; init; }

            [Description("The association ID.")]
            public string? AssociationId { get; init; }

            [Description("The carrier IP address associated with the network interface.")]
            public string? CarrierIp { get; init; }

            [Description("The address of the Elastic IP address bound to the network interface.")]
            public string? PublicIp { get; init; }

            [Description("The customer-owned IP address associated with the network interface.")]
            public string? CustomerOwnedIp { get; init; }

            [Description("The ID of the Elastic IP address owner.")]
            public string? IpOwnerId { get; init; }

            [Description("The public DNS name.")]
            public string? PublicDnsName { get; init; }
        }

        [Description("Describes a network interface attachment.")]
        public class NetworkAttachment
        {
            [Description("Indicates whether the network interface is deleted when the instance is terminated.")]
            public bool DeleteOnTermination { get; init; }

            [Description("The device index of the network interface attachment on the instance.")]
            public int DeviceIndex { get; init; }

            [Description("The ID of the network interface attachment.")]
            public string? Id { get; init; }

            [Description("The ID of the instance.")]
            public string? InstanceId { get; init; }

            [Description("The AWS account ID of the owner of the instance.")]
            public string? InstanceOwnerId { get; init; }

            [Description("The index of the network card.")]
            public int NetworkCardIndex { get; init; }

            [Description("The attachment state.")]
            public string? Status { get; init; }
        }

        [Description("Describes a security group.")]
        public class Group
        {
            [Description("The ID of the security group.")]
            public string? Id { get; init; }

            [Description("The name of the security group.")]
            public string? Name { get; init; }
        }
    }
}
