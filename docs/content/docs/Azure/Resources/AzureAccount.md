+++
title = "AzureAccount"
description = "Coming soon!"
weight = 1
+++



**Note:** _You will not be able to write a policy directly against this type._

## Properties
|Name|Description|Type|
|----------|----------|----------|
|Id|The account identifier specific to the cloud provider|[AzureAccountIdentifier](#azureaccountidentifier)|
|Type|The type of the account, eg: AWS/Azure/GCP|String|

## Policy Template
This template will give you a quick head start on generating a policy for an AzureAccount:

```
resource "AzureAccount"

validate {

}
```
## Nested Types
### AzureAccountIdentifier


#### Properties
|Name|Description|Type|
|----------|----------|----------|

