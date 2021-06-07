using System.Collections.Generic;
using System.ComponentModel;

namespace Aspect.Providers.AWS.Models.EC2
{
    [Description("An Elastic IP address is a static IPv4 address designed for dynamic cloud computing. you can mask the failure of an instance or software by rapidly remapping the address to another instance in your account. Alternatively, you can specify the Elastic IP address in a DNS record for your domain, so that your domain points to your instance.")]
    public class AwsElasticIp : AwsResource
    {
        [Description("The ID representing the allocation of the address for use with EC2-VPC.")]
        public string? AllocationId { get; init; }

        [Description("The ID representing the association of the address with an instance in a VPC.")]
        public string? AssociationId { get; init; }

        [Description("The carrier IP address associated. This option is only available for network interfaces which reside in a subnet in a Wavelength Zone (for example an EC2 instance).")]
        public string? CarrierIp { get; init; }

        [Description("The customer-owned IP address.")]
        public string? CustomerOwnedIp { get; init; }

        [Description("The ID of the customer-owned address pool.")]
        public string? CustomerOwnedIpv4Pool { get; init; }

        [Description("The ID of the instance that the address is associated with (if any).")]
        public string? InstanceId { get; init; }

        [Description("The private IP address associated with the Elastic IP address.")]
        public string? PrivateIpAddress { get; init; }

        [Description("The Elastic IP address.")]
        public string? PublicIp { get; init; }

        [Description("The ID of an address pool.")]
        public string? PublicIpv4Pool { get; init; }

        [Description("The name of the unique set of Availability Zones, Local Zones, or Wavelength Zones from which AWS advertises IP addresses.")]
        public string? NetworkBorderGroup { get; init; }

        [Description("The ID of the network interface.")]
        public string? NetworkInterfaceId { get; init; }

        [Description("The ID of the AWS account that owns the network interface.")]
        public string? NetworkInterfaceOwnerId { get; init; }

        public AwsElasticIp(AwsAccount account, string arn, string name, IReadOnlyList<KeyValuePair<string, string>> tags, string region)
            : base(account, arn, name, tags, nameof(AwsElasticIp), region)
        {
        }
    }
}
