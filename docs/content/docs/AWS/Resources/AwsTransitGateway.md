+++
title = "AwsTransitGateway"
description = "AWS Transit Gateway connects VPCs and on-premises networks through a central hub. This simplifies your network and puts an end to complex peering relationships. It acts as a cloud router – each new connection is only made once."
weight = 15
+++

AWS Transit Gateway connects VPCs and on-premises networks through a central hub. This simplifies your network and puts an end to complex peering relationships. It acts as a cloud router – each new connection is only made once.

## Properties
{{< table style="table-striped" >}}
|Name|Description|Type|
|----------|----------|----------|
|Account|The account that is associated with the resource.|[AwsAccount](/docs/aws/resources/awsaccount/)|
|Arn|The Amazon Resource Names (ARN) uniquely identifying an AWS resource.|String|
|Configuration|The current configuration of the transit gateway.|[Config](#config)|
|Description|The description of the transit gateway.|String|
|Id|The ID of the transit gateway.|String|
|Name|The name of the resource.|String|
|OwnerId|The ID of the AWS account ID that owns the transit gateway.|String|
|Region|The region in which this resource is located.|String|
|State|The state of the transit gateway: available / deleted / deleting / modifying / pending.|String|
|Tags|The tags that are associated with the resource. Each entry has two properties: Key/Value. There may be 0 or more entries in this collection.|Collection\<KeyValuePair<String, String>>|
|Type|The type of the resource. This is what is used in policy evaluation.|String|
{{< /table >}}

## Policy Template
This template will give you a quick head start on generating a policy for an AwsTransitGateway:

{{< code lang="tf" >}}
resource "AwsTransitGateway"

validate {

}
{{< /code >}}
## Nested Types
### Config
Describes the current configuration for a transit gateway.

#### Properties
{{< table style="table-striped" >}}
|Name|Description|Type|
|----------|----------|----------|
|AmazonAsn|A private Autonomous System Number (ASN) for the Amazon side of a BGP session.|Number|
|AssociationDefaultRouteTableId|The ID of the default association route table.|String|
|AutoAcceptsSharedAttachments|Indicates whether attachment requests are automatically accepted.|Boolean|
|DefaultRouteTableAssociation|Indicates whether resource attachments are automatically associated with the default association route table.|Boolean|
|DefaultRouteTablePropagation|Indicates whether resource attachments automatically propagate routes to the default propagation route table.|Boolean|
|PropagationDefaultRouteTableId|The ID of the default propagation route table.|String|
|SupportsDNS|Indicates whether or not the transit gateway has DNS support enabled.|Boolean|
|SupportsECMP|Indicates whether or not the transit gateway has equal-cost multi-path routing (ECMP) enabled.|Boolean|
|SupportsMulticast|Indicates whether or not the transit gateway has multicasting enabled.|Boolean|
|TransitGatewayCidrBlocks|The transit gateway CIDR blocks. There may be 0 or more entries in this collection.|Collection\<String>|
{{< /table >}}

