+++
title = "AwsAccount"
description = "The AWS account resource describes the details of the account that a specified resource belongs to."
weight = 1
+++

The AWS account resource describes the details of the account that a specified resource belongs to.

{{< alert style="warning" >}} **Note:** _You will not be able to write a policy directly against this type._ {{< /alert >}}

## Properties
{{< table style="table-striped" >}}
|Name|Description|Type|
|----------|----------|----------|
|Id|The account identifier specific to the cloud provider.|[AwsAccountIdentifier](#awsaccountidentifier)|
|Type|The type of the account, eg: AWS/Azure/GCP.|String|
{{< /table >}}

## Policy Template
This template will give you a quick head start on generating a policy for an AwsAccount:

{{< code lang="tf" >}}
resource "AwsAccount"

validate {

}
{{< /code >}}
## Nested Types
### AwsAccountIdentifier


#### Properties
{{< table style="table-striped" >}}
|Name|Description|Type|
|----------|----------|----------|
|Id|The 12 digit numeric identifer of the account.|String|
|Name|The friendly name of the account if it's available. Usually only supported by organisational accounts.|String|
{{< /table >}}

