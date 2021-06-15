+++
title = "AwsNetworkInterface"
description = "An elastic network interface is a logical networking component in a VPC that represents a virtual network card."
weight = 10
+++

An elastic network interface is a logical networking component in a VPC that represents a virtual network card.

## Properties
{{< table style="table-striped" >}}
|Name|Description|Type|
|----------|----------|----------|
|Account|The account that is associated with the resource.|[AwsAccount](/docs/aws/resources/awsaccount/)|
|Association|The association information for an Elastic IP address (IPv4) associated with the network interface.|[NetworkAssociation](#networkassociation)|
|Attachment|The network interface attachment.|[NetworkAttachment](#networkattachment)|
|AvailabilityZone|The Availability Zone.|String|
|CloudId|The cloud specific unique identifier for this resource, eg: AWS Arn.|String|
|Description|A description.|String|
|Groups|Any security groups for the network interface. There may be 0 or more entries in this collection.|Collection\<[Group](#group)>|
|Id|The ID of the network interface.|String|
|InterfaceType|The type of network interface.|String|
|IpAddresses|The private IPv4 addresses associated with the network interface. There may be 0 or more entries in this collection.|Collection\<String>|
|Ipv6Addresses|The IPv6 addresses associated with the network interface. There may be 0 or more entries in this collection.|Collection\<String>|
|MacAddress|The MAC address.|String|
|Name|The name of the resource.|String|
|OutpostArn|The Amazon Resource Name (ARN) of the Outpost.|String|
|OwnerId|The AWS account ID of the owner of the network interface.|String|
|PrivateDnsName|The private DNS name.|String|
|Region|The region in which this resource is located.|String|
|RequesterId|The alias or AWS account ID of the principal or service that created the network interface.|String|
|RequesterManaged|Indicates whether the network interface is being managed by AWS.|Boolean|
|SourceDestCheck|Indicates whether source/destination checking is enabled.|Boolean|
|Status|The status of the network interface.|String|
|SubnetId|The ID of the subnet.|String|
|Tags|The tags that are associated with the resource. Each entry has two properties: Key/Value. There may be 0 or more entries in this collection.|Collection\<KeyValuePair<String, String>>|
|Type|The type of the resource. This is what is used in policy evaluation.|String|
|VpcId|The ID of the VPC.|String|
{{< /table >}}

## Policy Template
This template will give you a quick head start on generating a policy for an AwsNetworkInterface:

{{< code lang="tf" >}}
resource "AwsNetworkInterface"

validate {

}
{{< /code >}}
## Nested Types
### NetworkAssociation
Describes association information for an Elastic IP address (IPv4 only), or a Carrier IP address (for a network interface which resides in a subnet in a Wavelength Zone).

#### Properties
{{< table style="table-striped" >}}
|Name|Description|Type|
|----------|----------|----------|
|AllocationId|The allocation ID.|String|
|AssociationId|The association ID.|String|
|CarrierIp|The carrier IP address associated with the network interface.|String|
|CustomerOwnedIp|The customer-owned IP address associated with the network interface.|String|
|IpOwnerId|The ID of the Elastic IP address owner.|String|
|PublicDnsName|The public DNS name.|String|
|PublicIp|The address of the Elastic IP address bound to the network interface.|String|
{{< /table >}}

### NetworkAttachment
Describes a network interface attachment.

#### Properties
{{< table style="table-striped" >}}
|Name|Description|Type|
|----------|----------|----------|
|DeleteOnTermination|Indicates whether the network interface is deleted when the instance is terminated.|Boolean|
|DeviceIndex|The device index of the network interface attachment on the instance.|Number|
|Id|The ID of the network interface attachment.|String|
|InstanceId|The ID of the instance.|String|
|InstanceOwnerId|The AWS account ID of the owner of the instance.|String|
|NetworkCardIndex|The index of the network card.|Number|
|Status|The attachment state.|String|
{{< /table >}}

### Group
Describes a security group.

#### Properties
{{< table style="table-striped" >}}
|Name|Description|Type|
|----------|----------|----------|
|Id|The ID of the security group.|String|
|Name|The name of the security group.|String|
{{< /table >}}

