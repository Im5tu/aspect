using System.Collections.Generic;
using System.ComponentModel;

namespace Aspect.Providers.AWS.Models.EC2
{
    [Description("Amazon Elastic Compute Cloud (Amazon EC2) is a web service that provides secure, resizable compute capacity in the cloud. It is designed to make web-scale cloud computing easier for developers. Amazon EC2’s simple web service interface allows you to obtain and configure capacity with minimal friction.")]
    public class AwsEc2Instance : AwsResource
    {
        [Description("The architecture of the image.")]
        public string? Architecture { get; init; }

        [Description("The boot mode of the image. For more information, see \"https://docs.aws.amazon.com/AWSEC2/latest/UserGuide/ami-boot.html\" (Boot modes) in the Amazon Elastic Compute Cloud User Guide.")]
        public string? BootMode { get; init; }

        [Description("The CPU options for the instance.")]
        public Cpu? CpuOptions { get; init; }

        [Description("Indicates whether the instance is optimized for Amazon EBS I/O. This optimization provides dedicated throughput to Amazon EBS and an optimized configuration stack to provide optimal I/O performance. This optimization isn't available with all instance types.")]
        public bool EbsOptimized { get; init; }

        [Description("Specifies whether enhanced networking with ENA is enabled.")]
        public bool EnaSupport { get; init; }

        [Description("Indicates whether the instance is enabled for AWS Nitro Enclaves.")]
        public bool EnclaveEnabled { get; init; }

        [Description("Indicates whether the instance is enabled for hibernation.")]
        public bool HibernationEnabled { get; init; }

        [Description("The hypervisor type of the instance. The value 'xen' is used for both Xen and Nitro hypervisors.")]
        public string? Hypervisor { get; init; }

        [Description("The IAM instance profile associated with the instance, if applicable")]
        public string? IamInstanceProfile { get; init; }

        [Description("The ID of the AMI used to launch the instance.")]
        public string? ImageId { get; init; }

        [Description("The ID of the instance.")]
        public string? InstanceId { get; init; }

        [Description("Indicates whether this is a Spot Instance or a Scheduled Instance.")]
        public string? InstanceLifecycle { get; init; }

        [Description("The instance type.")]
        public string? InstanceType { get; init; }

        [Description("The kernel associated with this instance, if applicable.")]
        public string? KernelId { get; init; }

        [Description("The name of the key pair, if this instance was launched with an associated key pair.")]
        public string? KeyName { get; init; }

        [Description("The license configurations.")]
        public IEnumerable<string>? Licenses { get; init; }

        [Description("Indicates whether detailed monitoring is enabled. Otherwise, basic monitoring is enabled.")]
        public string? Monitoring { get; init; }

        [Description("The network interfaces for the instance.")]
        public IEnumerable<NetworkInterface>? NetworkInterfaces { get; init; }

        [Description("The location where the instance launched, if applicable.")]
        public InstancePlacement? Placement { get; init; }

        [Description("The value is 'Windows' for Windows instances; otherwise blank.")]
        public string? Platform { get; init; }

        [Description("The private DNS hostname name assigned to the instance. This DNS hostname can only be used inside the Amazon EC2 network.")]
        public string? PrivateDnsName { get; init; }

        [Description("The private IPv4 address assigned to the instance.")]
        public string? PrivateIpAddress { get; init; }

        [Description("The public DNS name assigned to the instance.")]
        public string? PublicDnsName { get; init; }

        [Description("The public IPv4 address, or the Carrier IP address assigned to the instance.")]
        public string? PublicIpAddress { get; init; }

        [Description("The RAM disk associated with this instance, if applicable.")]
        public string? RamdiskId { get; init; }

        [Description("The device name of the root device volume (for example, '/dev/sda1').")]
        public string? RootDeviceName { get; init; }

        [Description("The root device type used by the AMI. The AMI can use an EBS volume or an instance store volume")]
        public string? RootDeviceType { get; init; }

        [Description("Indicates whether source/destination checking is enabled.")]
        public bool SourceDestCheck { get; init; }

        [Description("Specifies whether enhanced networking with the Intel 82599 Virtual Function interface is enabled")]
        public string? SriovNetSupport { get; init; }

        [Description("The current state of the instance.")]
        public string? State { get; init; }

        [Description("The ID of the subnet in which the instance is running.")]
        public string? SubnetId { get; init; }

        [Description("The virtualization type of the instance.")]
        public string? VirtualizationType { get; init; }

        [Description("The ID of the VPC in which the instance is running.")]
        public string? VpcId { get; init; }

        public AwsEc2Instance(AwsAccount account, string arn, string name, IReadOnlyList<KeyValuePair<string, string>> tags, string region)
            : base(account, arn, name, tags, nameof(AwsEc2Instance), region)
        {
        }

        [Description("Describes the placement of an EC2 instance.")]
        public class InstancePlacement
        {
            [Description("The affinity setting for the instance on the Dedicated Host")]
            public string? Affinity { get; init; }

            [Description("The Availability Zone of the instance.")]
            public string? AvailabilityZone { get; init; }

            [Description("The name of the placement group the instance is in.")]
            public string? GroupName { get; init; }

            [Description("The ID of the Dedicated Host on which the instance resides.")]
            public string? HostId { get; init; }

            [Description("The number of the partition the instance is in.")]
            public int? PartitionNumber { get; init; }

            [Description("")]
            public string? SpreadDomain { get; init; }

            [Description("The tenancy of the instance (if the instance is running in a VPC). An instance with a tenancy of 'dedicated' runs on single-tenant hardware.")]
            public string? Tenancy { get; init; }
        }

        [Description("Describes a network interface that's attached to an EC2 instance.")]
        public class NetworkInterface
        {
            [Description("The description.")]
            public string? Description { get; init; }

            [Description("Describes the type of network interface (interface/efa)")]
            public string? InterfaceType { get; init; }

            [Description("The MAC address of the network interface")]
            public string? MacAddress { get; init; }

            [Description("The ID of the network interface.")]
            public string? NetworkInterfaceId { get; init; }

            [Description("The ID of the AWS account that created the network interface.")]
            public string? OwnerId { get; init; }

            [Description("The IPv4 address of the network interface within the subnet.")]
            public string? PrimaryPrivateIpAddress { get; init; }

            [Description("The private DNS name.")]
            public string? PrivateDnsName { get; init; }

            [Description("One or more private IPv4 addresses associated with the network interface.")]
            public IEnumerable<string>? PrivateIpAddresses { get; init; }

            [Description("Indicates whether source/destination checking is enabled.")]
            public bool SourceDestCheck { get; init; }

            [Description("The status of the network interface.")]
            public string? Status { get; init; }

            [Description("The ID of the subnet.")]
            public string? SubnetId { get; init; }

            [Description("The ID of the VPC.")]
            public string? VpcId { get; init; }
        }

        [Description("The CPU options for the instance")]
        public class Cpu
        {
            [Description("The number of CPU cores for the instance.")]
            public int? Cores { get; init; }

            [Description("The number of threads per CPU core.")]
            public int? Threads { get; init; }
        }
    }
}
