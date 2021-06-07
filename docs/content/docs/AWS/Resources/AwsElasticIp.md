+++
title = "AwsElasticIp"
description = "An Elastic IP address is a static IPv4 address designed for dynamic cloud computing. you can mask the failure of an instance or software by rapidly remapping the address to another instance in your account. Alternatively, you can specify the Elastic IP address in a DNS record for your domain, so that your domain points to your instance."
weight = 7
+++

An Elastic IP address is a static IPv4 address designed for dynamic cloud computing. you can mask the failure of an instance or software by rapidly remapping the address to another instance in your account. Alternatively, you can specify the Elastic IP address in a DNS record for your domain, so that your domain points to your instance.

## Properties
{{< table style="table-striped" >}}
|Name|Description|Type|
|----------|----------|----------|
|Account|The account that is associated with the resource.|[AwsAccount](/docs/aws/resources/awsaccount/)|
|AllocationId|The ID representing the allocation of the address for use with EC2-VPC.|String|
|Arn|The Amazon Resource Names (ARN) uniquely identifying an AWS resource.|String|
|AssociationId|The ID representing the association of the address with an instance in a VPC.|String|
|CarrierIp|The carrier IP address associated. This option is only available for network interfaces which reside in a subnet in a Wavelength Zone (for example an EC2 instance).|String|
|CustomerOwnedIp|The customer-owned IP address.|String|
|CustomerOwnedIpv4Pool|The ID of the customer-owned address pool.|String|
|InstanceId|The ID of the instance that the address is associated with (if any).|String|
|Name|The name of the resource.|String|
|NetworkBorderGroup|The name of the unique set of Availability Zones, Local Zones, or Wavelength Zones from which AWS advertises IP addresses.|String|
|NetworkInterfaceId|The ID of the network interface.|String|
|NetworkInterfaceOwnerId|The ID of the AWS account that owns the network interface.|String|
|PrivateIpAddress|The private IP address associated with the Elastic IP address.|String|
|PublicIp|The Elastic IP address.|String|
|PublicIpv4Pool|The ID of an address pool.|String|
|Region|The region in which this resource is located.|String|
|Tags|The tags that are associated with the resource. Each entry has two properties: Key/Value. There may be 0 or more entries in this collection.|Collection\<KeyValuePair<String, String>>|
|Type|The type of the resource. This is what is used in policy evaluation.|String|
{{< /table >}}

## Policy Template
This template will give you a quick head start on generating a policy for an AwsElasticIp:

{{< code lang="tf" >}}
resource "AwsElasticIp"

validate {

}
{{< /code >}}
