+++
title = "AwsReservedInstance"
description = "Reserved Instances (RI) provide a significant discount (up to 72%) compared to On-Demand pricing for EC2 Instances and provide a capacity reservation when used in a specific Availability Zone."
weight = 12
+++

Reserved Instances (RI) provide a significant discount (up to 72%) compared to On-Demand pricing for EC2 Instances and provide a capacity reservation when used in a specific Availability Zone.

## Properties
{{< table style="table-striped" >}}
|Name|Description|Type|
|----------|----------|----------|
|Account|The account that is associated with the resource.|[AwsAccount](/docs/aws/resources/awsaccount/)|
|AvailabilityZone|The Availability Zone in which the Reserved Instance can be used.|String|
|CloudId|The cloud specific unique identifier for this resource, eg: AWS Arn.|String|
|CurrencyCode|The currency of the Reserved Instance. It's specified using ISO 4217 standard currency codes.|String|
|Expires|The time when the Reserved Instance expires.|DateTime|
|FixedPrice|The purchase price of the Reserved Instance.|Single|
|Id|The ID of the Reserved Instance.|String|
|InstanceCount|The number of reservations purchased.|Number|
|InstanceTenancy|The tenancy of the instance.|String|
|InstanceType|The instance type on which the Reserved Instance can be used.|String|
|Name|The name of the resource.|String|
|OfferingClass|The offering class of the Reserved Instance.|String|
|Region|The region in which this resource is located.|String|
|State|The state of the Reserved Instance purchase.|String|
|Tags|The tags that are associated with the resource. Each entry has two properties: Key/Value. There may be 0 or more entries in this collection.|Collection\<KeyValuePair<String, String>>|
|Type|The type of the resource. This is what is used in policy evaluation.|String|
|UsagePrice|The usage price of the Reserved Instance, per hour.|Single|
{{< /table >}}

## Policy Template
This template will give you a quick head start on generating a policy for an AwsReservedInstance:

{{< code lang="tf" >}}
resource "AwsReservedInstance"

validate {

}
{{< /code >}}
