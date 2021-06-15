+++
title = "AwsEc2Instance_SourceDestinationChecks.policy"
weight = 5
+++

## Commands

{{< table style="table-striped" >}}
|Command|Description|
|------|------|
|`aspect policy view BuiltIn\AWS\AwsEc2Instance_SourceDestinationChecks.policy`|View the contents of the policy|
|`aspect policy validate BuiltIn\AWS\AwsEc2Instance_SourceDestinationChecks.policy`|Validate the policy|
|`aspect run BuiltIn\AWS\AwsEc2Instance_SourceDestinationChecks.policy`|Validate the policy|
{{< /table >}}

## Policy Definition
{{< code lang="tf" >}}
resource "AwsEc2Instance"

exclude {
    hasKey(input.Tags, "AspectIgnore")
}

validate {
    input.NetworkInterfaces[_].SourceDestCheck == true
}

{{< /code >}}

