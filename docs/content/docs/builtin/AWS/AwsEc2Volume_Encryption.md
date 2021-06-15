+++
title = "AwsEc2Volume_Encryption.policy"
weight = 10
+++

## Commands

{{< table style="table-striped" >}}
|Command|Description|
|------|------|
|`aspect policy view BuiltIn\AWS\AwsEc2Volume_Encryption.policy`|View the contents of the policy|
|`aspect policy validate BuiltIn\AWS\AwsEc2Volume_Encryption.policy`|Validate the policy|
|`aspect run BuiltIn\AWS\AwsEc2Volume_Encryption.policy`|Validate the policy|
{{< /table >}}

## Policy Definition
{{< code lang="tf" >}}
resource "AwsEc2Volume"

exclude {
    hasKey(input.Tags, "AspectIgnore")
}

validate {
    input.Encrypted == true
}

{{< /code >}}

