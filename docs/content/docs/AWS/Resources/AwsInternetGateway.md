+++
title = "AwsInternetGateway"
description = "Internet gateway are horizontally scaled, redundant, and highly available VPC components that allows communication between your VPC and the internet."
weight = 8
+++

Internet gateway are horizontally scaled, redundant, and highly available VPC components that allows communication between your VPC and the internet.

## Properties
{{< table style="table-striped" >}}
|Name|Description|Type|
|----------|----------|----------|
|Account|The account that is associated with the resource.|[AwsAccount](/docs/aws/resources/awsaccount/)|
|Arn|The Amazon Resource Names (ARN) uniquely identifying an AWS resource.|String|
|Attachments|Any VPCs attached to the internet gateway. There may be 0 or more entries in this collection.|Collection\<[Attachment](#attachment)>|
|Id|The ID of the internet gateway.|String|
|Name|The name of the resource.|String|
|OwnerId|The ID of the AWS account that owns the internet gateway.|String|
|Region|The region in which this resource is located.|String|
|Tags|The tags that are associated with the resource. Each entry has two properties: Key/Value. There may be 0 or more entries in this collection.|Collection\<KeyValuePair<String, String>>|
|Type|The type of the resource. This is what is used in policy evaluation.|String|
{{< /table >}}

## Policy Template
This template will give you a quick head start on generating a policy for an AwsInternetGateway:

{{< code lang="tf" >}}
resource "AwsInternetGateway"

validate {

}
{{< /code >}}
## Nested Types
### Attachment
Describes the attachment of a VPC to an internet gateway or an egress-only internet gateway.

#### Properties
{{< table style="table-striped" >}}
|Name|Description|Type|
|----------|----------|----------|
|State|The current state of the attachment.|String|
|VpcId|The ID of the VPC.|String|
{{< /table >}}

