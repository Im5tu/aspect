+++
title = "AwsEc2Instance_Monitoring.policy"
weight = 3
+++

## Commands

{{< table style="table-striped" >}}
|Command|Description|
|------|------|
|`aspect policy view BuiltIn\AWS\AwsEc2Instance_Monitoring.policy`|View the contents of the policy|
|`aspect policy validate BuiltIn\AWS\AwsEc2Instance_Monitoring.policy`|Validate the policy|
|`aspect run BuiltIn\AWS\AwsEc2Instance_Monitoring.policy`|Validate the policy|
{{< /table >}}

## Policy Definition
{{< code lang="tf" >}}
resource "AwsEc2Instance"

exclude {
    hasKey(input.Tags, "AspectIgnore")
    hasKey(input.Tags, "PubliclyAccessible")
}

validate {
    input.Monitoring == "enabled"
}

{{< /code >}}

