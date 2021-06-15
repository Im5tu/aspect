+++
title = "AwsTransitGateway_Tagging.policy"
weight = 21
+++

## Commands

{{< table style="table-striped" >}}
|Command|Description|
|------|------|
|`aspect policy view BuiltIn\AWS\AwsTransitGateway_Tagging.policy`|View the contents of the policy|
|`aspect policy validate BuiltIn\AWS\AwsTransitGateway_Tagging.policy`|Validate the policy|
|`aspect run BuiltIn\AWS\AwsTransitGateway_Tagging.policy`|Validate the policy|
{{< /table >}}

## Policy Definition
{{< code lang="tf" >}}
resource "AwsTransitGateway"

exclude {
    hasKey(input.Tags, "AspectIgnore")
}

validate {
    hasKey(input.Tags, "Name")
    hasKey(input.Tags, "Product")
    hasKey(input.Tags, "Product-Group")
    hasKey(input.Tags, "Environment")
}

{{< /code >}}

