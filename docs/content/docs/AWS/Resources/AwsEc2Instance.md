+++
title = "AwsEc2Instance"
description = "Amazon Elastic Compute Cloud (Amazon EC2) is a web service that provides secure, resizable compute capacity in the cloud. It is designed to make web-scale cloud computing easier for developers. Amazon EC2’s simple web service interface allows you to obtain and configure capacity with minimal friction."
weight = 3
+++

Amazon Elastic Compute Cloud (Amazon EC2) is a web service that provides secure, resizable compute capacity in the cloud. It is designed to make web-scale cloud computing easier for developers. Amazon EC2’s simple web service interface allows you to obtain and configure capacity with minimal friction.

## Properties
{{< table style="table-striped" >}}
|Name|Description|Type|
|----------|----------|----------|
|Account|The account that is associated with the resource.|[AwsAccount](/docs/aws/resources/awsaccount/)|
|Architecture|The architecture of the image.|String|
|BootMode|The boot mode of the image. For more information, see "https://docs.aws.amazon.com/AWSEC2/latest/UserGuide/ami-boot.html" (Boot modes) in the Amazon Elastic Compute Cloud User Guide.|String|
|CloudId|The cloud specific unique identifier for this resource, eg: AWS Arn.|String|
|CpuOptions|The CPU options for the instance.|[Cpu](#cpu)|
|EbsOptimized|Indicates whether the instance is optimized for Amazon EBS I/O. This optimization provides dedicated throughput to Amazon EBS and an optimized configuration stack to provide optimal I/O performance. This optimization isn't available with all instance types.|Boolean|
|EnaSupport|Specifies whether enhanced networking with ENA is enabled.|Boolean|
|EnclaveEnabled|Indicates whether the instance is enabled for AWS Nitro Enclaves.|Boolean|
|HibernationEnabled|Indicates whether the instance is enabled for hibernation.|Boolean|
|Hypervisor|The hypervisor type of the instance. The value 'xen' is used for both Xen and Nitro hypervisors.|String|
|IamInstanceProfile|The IAM instance profile associated with the instance, if applicable.|String|
|ImageId|The ID of the AMI used to launch the instance.|String|
|InstanceId|The ID of the instance.|String|
|InstanceLifecycle|Indicates whether this is a Spot Instance or a Scheduled Instance.|String|
|InstanceType|The instance type.|String|
|KernelId|The kernel associated with this instance, if applicable.|String|
|KeyName|The name of the key pair, if this instance was launched with an associated key pair.|String|
|Licenses|The license configurations. There may be 0 or more entries in this collection.|Collection\<String>|
|Monitoring|Indicates whether detailed monitoring is enabled. Otherwise, basic monitoring is enabled.|String|
|Name|The name of the resource.|String|
|NetworkInterfaces|The network interfaces for the instance. There may be 0 or more entries in this collection.|Collection\<[NetworkInterface](#networkinterface)>|
|Placement|The location where the instance launched, if applicable.|[InstancePlacement](#instanceplacement)|
|Platform|The value is 'Windows' for Windows instances; otherwise blank.|String|
|PrivateDnsName|The private DNS hostname name assigned to the instance. This DNS hostname can only be used inside the Amazon EC2 network.|String|
|PrivateIpAddress|The private IPv4 address assigned to the instance.|String|
|PublicDnsName|The public DNS name assigned to the instance.|String|
|PublicIpAddress|The public IPv4 address, or the Carrier IP address assigned to the instance.|String|
|RamdiskId|The RAM disk associated with this instance, if applicable.|String|
|Region|The region in which this resource is located.|String|
|RootDeviceName|The device name of the root device volume (for example, '/dev/sda1').|String|
|RootDeviceType|The root device type used by the AMI. The AMI can use an EBS volume or an instance store volume.|String|
|SourceDestCheck|Indicates whether source/destination checking is enabled.|Boolean|
|SriovNetSupport|Specifies whether enhanced networking with the Intel 82599 Virtual Function interface is enabled.|String|
|State|The current state of the instance.|String|
|SubnetId|The ID of the subnet in which the instance is running.|String|
|Tags|The tags that are associated with the resource. Each entry has two properties: Key/Value. There may be 0 or more entries in this collection.|Collection\<KeyValuePair<String, String>>|
|Type|The type of the resource. This is what is used in policy evaluation.|String|
|VirtualizationType|The virtualization type of the instance.|String|
|VpcId|The ID of the VPC in which the instance is running.|String|
{{< /table >}}

## Policy Template
This template will give you a quick head start on generating a policy for an AwsEc2Instance:

{{< code lang="tf" >}}
resource "AwsEc2Instance"

validate {

}
{{< /code >}}
## Nested Types
### InstancePlacement
Describes the placement of an EC2 instance.

#### Properties
{{< table style="table-striped" >}}
|Name|Description|Type|
|----------|----------|----------|
|Affinity|The affinity setting for the instance on the Dedicated Host.|String|
|AvailabilityZone|The Availability Zone of the instance.|String|
|GroupName|The name of the placement group the instance is in.|String|
|HostId|The ID of the Dedicated Host on which the instance resides.|String|
|PartitionNumber|The number of the partition the instance is in.|Number|
|SpreadDomain|.|String|
|Tenancy|The tenancy of the instance (if the instance is running in a VPC). An instance with a tenancy of 'dedicated' runs on single-tenant hardware.|String|
{{< /table >}}

### NetworkInterface
Describes a network interface that's attached to an EC2 instance.

#### Properties
{{< table style="table-striped" >}}
|Name|Description|Type|
|----------|----------|----------|
|Description|The description.|String|
|InterfaceType|Describes the type of network interface (interface/efa).|String|
|MacAddress|The MAC address of the network interface.|String|
|NetworkInterfaceId|The ID of the network interface.|String|
|OwnerId|The ID of the AWS account that created the network interface.|String|
|PrimaryPrivateIpAddress|The IPv4 address of the network interface within the subnet.|String|
|PrivateDnsName|The private DNS name.|String|
|PrivateIpAddresses|One or more private IPv4 addresses associated with the network interface. There may be 0 or more entries in this collection.|Collection\<String>|
|SourceDestCheck|Indicates whether source/destination checking is enabled.|Boolean|
|Status|The status of the network interface.|String|
|SubnetId|The ID of the subnet.|String|
|VpcId|The ID of the VPC.|String|
{{< /table >}}

### Cpu
The CPU options for the instance

#### Properties
{{< table style="table-striped" >}}
|Name|Description|Type|
|----------|----------|----------|
|Cores|The number of CPU cores for the instance.|Number|
|Threads|The number of threads per CPU core.|Number|
{{< /table >}}

