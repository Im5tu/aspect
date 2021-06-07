using System.Collections.Generic;
using System.ComponentModel;

namespace Aspect.Providers.AWS.Models.EC2
{
    [Description("Network address translation (NAT) gateways enable instances in a private subnet to connect to the internet or other AWS services, but prevent the internet from initiating a connection with those instances.")]
    public class AwsNatGateway : AwsResource
    {
        [Description("The ID of the NAT gateway.")]
        public string? Id { get; init; }

        [Description("The state of the NAT gateway.")]
        public string? State { get; init; }

        [Description("If the NAT gateway could not be created, specifies the error code for the failure.")]
        public string? FailureCode { get; init; }

        [Description("The provisioned bandwidth.")]
        public string? ProvisionedBandwidth { get; init; }

        [Description("The ID of the subnet in which the NAT gateway is located.")]
        public string? SubnetId { get; init; }

        [Description("The ID of the VPC in which the NAT gateway is located.")]
        public string? VpcId { get; init; }

        [Description("Information about the IP addresses and network interface associated with the NAT gateway.")]
        public IEnumerable<Address>? Addresses { get; init; }

        public AwsNatGateway(AwsAccount account, string arn, string name, IReadOnlyList<KeyValuePair<string, string>> tags, string region)
            : base(account, arn, name, tags, nameof(AwsNatGateway), region)
        {
        }

        [Description("Describes the IP addresses and network interface associated with a NAT gateway.")]
        public class Address
        {
            [Description("The allocation ID of the Elastic IP address that's associated with the NAT gateway.")]
            public string? AllocationId { get; init; }

            [Description("The ID of the network interface associated with the NAT gateway.")]
            public string? NetworkInterfaceId { get; init; }

            [Description("The private IP address associated with the Elastic IP address.")]
            public string? PrivateIp { get; init; }

            [Description("The Elastic IP address associated with the NAT gateway.")]
            public string? PublicIp { get; init; }
        }
    }
}
