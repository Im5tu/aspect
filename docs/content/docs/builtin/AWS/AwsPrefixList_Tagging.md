+++
title = "AwsPrefixList_Tagging.policy"
weight = 15
+++

## Commands

{{< table style="table-striped" >}}
|Command|Description|
|------|------|
|`aspect policy view BuiltIn\AWS\AwsPrefixList_Tagging.policy`|View the contents of the policy|
|`aspect policy validate BuiltIn\AWS\AwsPrefixList_Tagging.policy`|Validate the policy|
|`aspect run BuiltIn\AWS\AwsPrefixList_Tagging.policy`|Validate the policy|
{{< /table >}}

## Policy Definition
{{< code lang="tf" >}}
resource "AwsPrefixList"

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

