+++
title = "AwsVpc"
description = "Amazon Virtual Private Cloud (Amazon VPC) is a service that lets you launch AWS resources in a logically isolated virtual network that you define. You have complete control over your virtual networking environment, including selection of your own IP address range, creation of subnets, and configuration of route tables and network gateways. You can use both IPv4 and IPv6 for most resources in your virtual private cloud, helping to ensure secure and easy access to resources and applications."
weight = 16
+++

Amazon Virtual Private Cloud (Amazon VPC) is a service that lets you launch AWS resources in a logically isolated virtual network that you define. You have complete control over your virtual networking environment, including selection of your own IP address range, creation of subnets, and configuration of route tables and network gateways. You can use both IPv4 and IPv6 for most resources in your virtual private cloud, helping to ensure secure and easy access to resources and applications.

## Properties
{{< table style="table-striped" >}}
|Name|Description|Type|
|----------|----------|----------|
|Account|The account that is associated with the resource.|[AwsAccount](/docs/aws/resources/awsaccount/)|
|CidrBlock|The primary IPv4 CIDR block for the VPC.|String|
|CloudId|The cloud specific unique identifier for this resource, eg: AWS Arn.|String|
|Endpoints|The endpoints that are associated with the VPC There may be 0 or more entries in this collection.|Collection\<[VpcEndpoint](#vpcendpoint)>|
|Id|The ID of the VPC.|String|
|InstanceTenancy|The allowed tenancy of instances launched into the VPC.|String|
|IsDefault|Indicates whether the VPC is the default VPC.|Boolean|
|Name|The name of the resource.|String|
|OwnerId|The ID of the AWS account that owns the VPC.|String|
|PeeringConnections|The VPC's that this VPC is peered with using a peering connection There may be 0 or more entries in this collection.|Collection\<[VpcPeeringConnection](#vpcpeeringconnection)>|
|Region|The region in which this resource is located.|String|
|State|The current state of the VPC: available / pending.|String|
|Subnets|The subnets that belong to the VPC There may be 0 or more entries in this collection.|Collection\<[VpcSubnet](#vpcsubnet)>|
|Tags|The tags that are associated with the resource. Each entry has two properties: Key/Value. There may be 0 or more entries in this collection.|Collection\<KeyValuePair<String, String>>|
|Type|The type of the resource. This is what is used in policy evaluation.|String|
{{< /table >}}

## Policy Template
This template will give you a quick head start on generating a policy for an AwsVpc:

{{< code lang="tf" >}}
resource "AwsVpc"

validate {

}
{{< /code >}}
## Nested Types
### VpcSubnet
A subnetwork or subnet is a logical subdivision of an IP network.

#### Properties
{{< table style="table-striped" >}}
|Name|Description|Type|
|----------|----------|----------|
|Arn|The Amazon Resource Name (ARN) of the subnet.|String|
|AvailabilityZone|The Availability Zone of the subnet.|String|
|AvailableIpAddressCount|The number of unused private IPv4 addresses in the subnet.|Number|
|CidrBlock|The IPv4 CIDR block assigned to the subnet.|String|
|Id|The ID of the subnet.|String|
|IsDefaultForAvailabilityZone|Indicates whether this is the default subnet for the Availability Zone.|Boolean|
|MapPublicIpOnLaunch|Indicates whether instances launched in this subnet receive a public IPv4 address.|Boolean|
|OutpostArn|The Amazon Resource Name (ARN) of the Outpost.|String|
|OwnerId|The ID of the AWS account that owns the subnet.|String|
{{< /table >}}

### VpcEndpoint
A VPC endpoint enables private connections between your VPC and supported AWS services and VPC endpoint services powered by AWS PrivateLink.  

#### Properties
{{< table style="table-striped" >}}
|Name|Description|Type|
|----------|----------|----------|
|DnsEntries|The DNS entries for the endpoint. There may be 0 or more entries in this collection.|Collection\<String>|
|Id|The ID of the VPC endpoint.|String|
|NetworkInterfaces|One or more network interfaces for the endpoint. There may be 0 or more entries in this collection.|Collection\<String>|
|OwnerId|The ID of the AWS account that owns the VPC endpoint.|String|
|PrivateDnsEnabled|Indicates whether the VPC is associated with a private hosted zone.|Boolean|
|RequesterManaged|Indicates whether the VPC endpoint is being managed by its service.|Boolean|
|RouteTables|(Gateway endpoint) One or more route tables associated with the endpoint. There may be 0 or more entries in this collection.|Collection\<String>|
|SecurityGroups|(Interface endpoint) The security groups that are associated with the network interface. There may be 0 or more entries in this collection.|Collection\<String>|
|ServiceName|The name of the service to which the endpoint is associated.|String|
|State|The state of the VPC endpoint.|String|
|SubnetIds|(Interface endpoint) One or more subnets in which the endpoint is located. There may be 0 or more entries in this collection.|Collection\<String>|
|Type|The type of endpoint.|String|
{{< /table >}}

### VpcPeeringConnection
A VPC peering connection is a networking connection between two VPCs that enables you to route traffic between them using private IPv4 addresses or IPv6 addresses.

#### Properties
{{< table style="table-striped" >}}
|Name|Description|Type|
|----------|----------|----------|
|Accepter|The VPC ID of the accepter.|String|
|Id|The ID of the VPC peering connection.|String|
|Requester|The VPC ID of the requester.|String|
|Status|Describes the status of a VPC peering connection.|String|
{{< /table >}}

