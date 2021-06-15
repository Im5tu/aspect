+++
title = "AwsNatGateway"
description = "Network address translation (NAT) gateways enable instances in a private subnet to connect to the internet or other AWS services, but prevent the internet from initiating a connection with those instances."
weight = 9
+++

Network address translation (NAT) gateways enable instances in a private subnet to connect to the internet or other AWS services, but prevent the internet from initiating a connection with those instances.

## Properties
{{< table style="table-striped" >}}
|Name|Description|Type|
|----------|----------|----------|
|Account|The account that is associated with the resource.|[AwsAccount](/docs/aws/resources/awsaccount/)|
|Addresses|Information about the IP addresses and network interface associated with the NAT gateway. There may be 0 or more entries in this collection.|Collection\<[Address](#address)>|
|CloudId|The cloud specific unique identifier for this resource, eg: AWS Arn.|String|
|FailureCode|If the NAT gateway could not be created, specifies the error code for the failure.|String|
|Id|The ID of the NAT gateway.|String|
|Name|The name of the resource.|String|
|ProvisionedBandwidth|The provisioned bandwidth.|String|
|Region|The region in which this resource is located.|String|
|State|The state of the NAT gateway.|String|
|SubnetId|The ID of the subnet in which the NAT gateway is located.|String|
|Tags|The tags that are associated with the resource. Each entry has two properties: Key/Value. There may be 0 or more entries in this collection.|Collection\<KeyValuePair<String, String>>|
|Type|The type of the resource. This is what is used in policy evaluation.|String|
|VpcId|The ID of the VPC in which the NAT gateway is located.|String|
{{< /table >}}

## Policy Template
This template will give you a quick head start on generating a policy for an AwsNatGateway:

{{< code lang="tf" >}}
resource "AwsNatGateway"

validate {

}
{{< /code >}}
## Nested Types
### Address
Describes the IP addresses and network interface associated with a NAT gateway.

#### Properties
{{< table style="table-striped" >}}
|Name|Description|Type|
|----------|----------|----------|
|AllocationId|The allocation ID of the Elastic IP address that's associated with the NAT gateway.|String|
|NetworkInterfaceId|The ID of the network interface associated with the NAT gateway.|String|
|PrivateIp|The private IP address associated with the Elastic IP address.|String|
|PublicIp|The Elastic IP address associated with the NAT gateway.|String|
{{< /table >}}

