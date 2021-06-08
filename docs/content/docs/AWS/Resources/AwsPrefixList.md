+++
title = "AwsPrefixList"
description = "A prefix list is a set of one or more CIDR blocks. You can use prefix lists to make it easier to configure and maintain your security groups and route tables."
weight = 11
+++

A prefix list is a set of one or more CIDR blocks. You can use prefix lists to make it easier to configure and maintain your security groups and route tables.

## Properties
{{< table style="table-striped" >}}
|Name|Description|Type|
|----------|----------|----------|
|Account|The account that is associated with the resource.|[AwsAccount](/docs/aws/resources/awsaccount/)|
|AddressFamily|The IP address version.|String|
|Arn|The Amazon Resource Names (ARN) uniquely identifying an AWS resource.|String|
|Cidrs|Information about the prefix list entries. There may be 0 or more entries in this collection.|Collection\<String>|
|Id|The ID of the prefix list.|String|
|Name|The name of the resource.|String|
|OwnerId|The ID of the owner of the prefix list.|String|
|Region|The region in which this resource is located.|String|
|State|The state of the prefix list.|String|
|Tags|The tags that are associated with the resource. Each entry has two properties: Key/Value. There may be 0 or more entries in this collection.|Collection\<KeyValuePair<String, String>>|
|Type|The type of the resource. This is what is used in policy evaluation.|String|
{{< /table >}}

## Policy Template
This template will give you a quick head start on generating a policy for an AwsPrefixList:

{{< code lang="tf" >}}
resource "AwsPrefixList"

validate {

}
{{< /code >}}
