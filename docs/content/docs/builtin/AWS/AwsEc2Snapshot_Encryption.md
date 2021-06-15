+++
title = "AwsEc2Snapshot_Encryption.policy"
weight = 8
+++

## Commands

{{< table style="table-striped" >}}
|Command|Description|
|------|------|
|`aspect policy view BuiltIn\AWS\AwsEc2Snapshot_Encryption.policy`|View the contents of the policy|
|`aspect policy validate BuiltIn\AWS\AwsEc2Snapshot_Encryption.policy`|Validate the policy|
|`aspect run BuiltIn\AWS\AwsEc2Snapshot_Encryption.policy`|Validate the policy|
{{< /table >}}

## Policy Definition
{{< code lang="tf" >}}
resource "AwsEc2Snapshot"

exclude {
    hasKey(input.Tags, "AspectIgnore")
    input.Progress != "100%"
}

validate {
    input.Encrypted == true
}

{{< /code >}}

