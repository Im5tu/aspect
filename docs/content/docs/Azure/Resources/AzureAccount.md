+++
title = "AzureAccount"
description = "Coming soon!"
weight = 1
+++



{{< alert style="warning" >}} **Note:** _You will not be able to write a policy directly against this type._ {{< /alert >}}

## Properties
{{< table style="table-striped" >}}
|Name|Description|Type|
|----------|----------|----------|
|Id|The account identifier specific to the cloud provider.|[AzureAccountIdentifier](#azureaccountidentifier)|
|Type|The type of the account, eg: AWS/Azure/GCP.|String|
{{< /table >}}

## Policy Template
This template will give you a quick head start on generating a policy for an AzureAccount:

{{< code lang="tf" >}}
resource "AzureAccount"

validate {

}
{{< /code >}}
## Nested Types
### AzureAccountIdentifier


#### Properties
{{< table style="table-striped" >}}
|Name|Description|Type|
|----------|----------|----------|
{{< /table >}}

