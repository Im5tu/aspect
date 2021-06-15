+++
title = "AwsEc2Instance_Tagging.policy"
weight = 6
+++

## Commands

{{< table style="table-striped" >}}
|Command|Description|
|------|------|
|`aspect policy view BuiltIn\AWS\AwsEc2Instance_Tagging.policy`|View the contents of the policy|
|`aspect policy validate BuiltIn\AWS\AwsEc2Instance_Tagging.policy`|Validate the policy|
|`aspect run BuiltIn\AWS\AwsEc2Instance_Tagging.policy`|Validate the policy|
{{< /table >}}

## Policy Definition
{{< code lang="tf" >}}
resource "AwsEc2Instance"

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

