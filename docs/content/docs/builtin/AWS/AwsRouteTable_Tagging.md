+++
title = "AwsRouteTable_Tagging.policy"
weight = 17
+++

## Commands

{{< table style="table-striped" >}}
|Command|Description|
|------|------|
|`aspect policy view BuiltIn\AWS\AwsRouteTable_Tagging.policy`|View the contents of the policy|
|`aspect policy validate BuiltIn\AWS\AwsRouteTable_Tagging.policy`|Validate the policy|
|`aspect run BuiltIn\AWS\AwsRouteTable_Tagging.policy`|Validate the policy|
{{< /table >}}

## Policy Definition
{{< code lang="tf" >}}
resource "AwsRouteTable"

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

