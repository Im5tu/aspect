+++
title = "AwsRouteTable"
description = "A route table contains a set of rules, called routes, that are used to determine where network traffic from your subnet or gateway is directed."
weight = 13
+++

A route table contains a set of rules, called routes, that are used to determine where network traffic from your subnet or gateway is directed.

## Properties
{{< table style="table-striped" >}}
|Name|Description|Type|
|----------|----------|----------|
|Account|The account that is associated with the resource.|[AwsAccount](/docs/aws/resources/awsaccount/)|
|Arn|The Amazon Resource Names (ARN) uniquely identifying an AWS resource.|String|
|Associations|The associations between the route table and one or more subnets or a gateway. There may be 0 or more entries in this collection.|Collection\<[Association](#association)>|
|Id|The ID of the route table.|String|
|Name|The name of the resource.|String|
|OwnerId|The ID of the AWS account that owns the route table.|String|
|PropagatingVirtualGateways|A list of gateway ids for any virtual private gateway (VGW) propagating routes. There may be 0 or more entries in this collection.|Collection\<String>|
|Region|The region in which this resource is located.|String|
|Routes|The routes in the route table. There may be 0 or more entries in this collection.|Collection\<[Route](#route)>|
|Tags|The tags that are associated with the resource. Each entry has two properties: Key/Value. There may be 0 or more entries in this collection.|Collection\<KeyValuePair<String, String>>|
|Type|The type of the resource. This is what is used in policy evaluation.|String|
|VpcId|The ID of the VPC.|String|
{{< /table >}}

## Policy Template
This template will give you a quick head start on generating a policy for an AwsRouteTable:

{{< code lang="tf" >}}
resource "AwsRouteTable"

validate {

}
{{< /code >}}
## Nested Types
### Association
Describes an association between a route table and a subnet or gateway.

#### Properties
{{< table style="table-striped" >}}
|Name|Description|Type|
|----------|----------|----------|
|GatewayId|The ID of the internet gateway or virtual private gateway.|String|
|Id|The ID of the association.|String|
|IsPrimaryRouteTable|Indicates whether this is the main route table.|Boolean|
|RouteTableId|The ID of the route table.|String|
|State|The state of the association.|String|
|SubnetId|The ID of the subnet. A subnet ID is not returned for an implicit association.|String|
{{< /table >}}

### Route
Describes a route in a route table.

#### Properties
{{< table style="table-striped" >}}
|Name|Description|Type|
|----------|----------|----------|
|CarrierGatewayId|The ID of the carrier gateway.|String|
|DestinationCidrBlock|The IPv4 CIDR block used for the destination match.|String|
|DestinationIpv6CidrBlock|The IPv6 CIDR block used for the destination match.|String|
|DestinationPrefixListId|The prefix of the AWS service.|String|
|EgressOnlyInternetGatewayId|The ID of the egress-only internet gateway.|String|
|GatewayId|The ID of a gateway attached to your VPC.|String|
|LocalGatewayId|The ID of the local gateway.|String|
|NatGatewayId|The ID of a NAT gateway.|String|
|NatInstanceId|The ID of a NAT instance in your VPC.|String|
|NatInstanceOwnerId|The AWS account ID of the owner of the NAT instance.|String|
|NetworkInterfaceId|The ID of the network interface.|String|
|Origin|Describes how the route was created. One of: CreateRouteTable/CreateRoute/EnableVgwRoutePropagation.|String|
|State|The state of the route. One of: Active/Blackhole.|String|
|TransitGatewayId|The ID of a transit gateway.|String|
|VpcPeeringConnectionId|The ID of a VPC peering connection.|String|
{{< /table >}}

